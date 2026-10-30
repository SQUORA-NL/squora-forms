using Squora.Forms.Blazor.ObjectHandlers;
using Squora.Forms.Blazor.Utillities;
using Squora.Forms.Common.Enums;
using Microsoft.AspNetCore.Components;
using Newtonsoft.Json.Linq;
using Newtonsoft.Json.Schema;
using Newtonsoft.Json;
using Microsoft.AspNetCore.Authorization;
using Squora.Forms.Core.Models;
using Squora.Forms.Core.Models.ParameterModels;


namespace Squora.Forms.Blazor.Components
{
    public abstract class FormComponentBase : ComponentBase
    {
        public RuleEffect RuleEffect { get; private set; } = RuleEffect.SHOW;

        public bool IsDisabled => RuleEffect == RuleEffect.DISABLE;

        [Parameter]
        public ComponentsList? Components { get; set; } = new ComponentsList();

        [Parameter] public bool EditMode { get; set; } = false;

        [Parameter]
        public bool AllowElements { get; set; } = false;

        [Parameter]
        public string? TreePosition { get; set; }

        public string ComponentId => string.IsNullOrEmpty(TreePosition) ? "" : $"bd-{TreePosition}";
        public string ErrorMessageId => string.IsNullOrEmpty(ComponentId) ? "" : $"{ComponentId}-error";
        public bool IsInvalid => !IsValid;

        [Parameter]
        public string Path { get; set; }

        [Parameter]
        public object? Value { get; set; }

        [Parameter]
        public string IteratorPath { get; set; }

        [Parameter]
        public FormComponentBase? Parent { get; set; }

        [Parameter]
        public DynamicFormModel FormModel { get; set; } = new DynamicFormModel();

        [Parameter]
        public string? Style { get; set; }

        [Parameter]
        public string? Class { get; set; }

        /// <summary>The share of the row this component takes in a horizontal layout: "10%", "30" (also a percentage) or a fixed size such as "200px".</summary>
        [Parameter]
        public string? Width { get; set; }

        /// <summary>
        /// The inline style for the component's wrapper: its <see cref="Width"/> (only inside a horizontal layout, where the wrapper is the
        /// flex item) and its <see cref="Style"/>. A percentage may shrink, in proportion, to make room for the gaps between the columns, so
        /// 10% and 90% fill the row exactly. In a row where some columns have a width, the ones without take what is left.
        /// </summary>
        public string? ItemStyle
        {
            get
            {
                var inRow = Parent?.FormModel?.DynamicType.ComponentType == ComponentType.HorizontalLayout;
                var width = inRow ? WidthStyle(Width, Parent!.FormModel.Elements.Count) ?? (RowHasWidths() ? "flex: 1 1 0%; min-width: 0; " : null) : null;
                var style = string.IsNullOrWhiteSpace(Style) ? null : Style.Trim().TrimEnd(';') + ";";
                return width == null && style == null ? null : $"{width}{style}";
            }
        }

        private bool RowHasWidths() =>
            Parent!.FormModel.Elements.Any(e => e.Parameters.Entries.TryGetValue(ParameterNames.Width, out var w) && WidthStyle(w as string, 1) != null);

        // only a number with a unit is let through into the style attribute
        private static string? WidthStyle(string? width, int columns)
        {
            width = width?.Trim();
            if (string.IsNullOrEmpty(width) || !System.Text.RegularExpressions.Regex.IsMatch(width, @"^\d+(\.\d+)?(%|px|rem|em|vw|ch)?$")) return null;
            if (char.IsDigit(width[^1]) || width.EndsWith('%'))
            {
                // the share of the room that is left once the gaps between the columns are taken out (the HTML row wraps when it does not fit)
                var share = double.Parse(width.TrimEnd('%'), System.Globalization.CultureInfo.InvariantCulture) / 100;
                var gaps = Math.Max(columns - 1, 0);
                return string.Create(System.Globalization.CultureInfo.InvariantCulture,
                    $"flex: 0 1 calc((100% - {gaps} * var(--bd-gap, 16px)) * {share}); min-width: 0; ");
            }
            return $"flex: 0 0 {width}; min-width: 0; ";
        }

        [Parameter]
        public EventCallback<object?> OnSubmitted { get; set; }

        [Parameter]
        public EventCallback<object?> ValueChanged { get; set; }

        [Parameter]
        public string? InvalidMessage { get; set; }

        public string GetInstancePath(string path)
        {
            if (path.StartsWith('@'))
            {
                return $"{IteratorPath}{path.Substring(1)}";
            }
            return path;
        }

        public bool IsValid { get; set; } = true;

        public abstract string ValidationString { get; }

        protected override void OnParametersSet()
        {
            if (FormModel?.Rules?.Count != 0)
            {
                EvaluateRules();
            }
            base.OnParametersSet();
        }

        public object GetValue()
        {
            if (Value == null)
            {
                return Value;
            }

            if (string.IsNullOrEmpty(Path))
            {
                return null;
            }

            return DataObjectHelper.GetValue(GetInstancePath(Path), Value);
        }

        public virtual void Validate()
        {
            // can be overriden in decendants
        }

        internal Dictionary<string, object> GetValidParameters()
        {
            return GetValidParametersFor(FormModel);
        }

        public Dictionary<string, object> GetValidParametersFor(DynamicFormModel formModel)
        {
            if (formModel == null) { return new Dictionary<string, object>(); }
            var componentType = GetComponentTypeFor(formModel);
            var validParameters = RegularObjectHandler.GetParameters(componentType, formModel.Parameters.Entries, IncludeDynamicOptionsInParameters(formModel.Parameters.Entries));
            // a parameter that is no longer passed keeps its last value on a component that is already there:
            // pass the class always, so removing it in the editor clears it
            validParameters[ParameterNames.Width] = formModel.Parameters.Entries.TryGetValue(ParameterNames.Width, out var width) ? width : null!;
            validParameters[ParameterNames.Class] = formModel.Parameters.Entries.TryGetValue(ParameterNames.Class, out var cssClass) ? cssClass : null!;
            validParameters.Add(ParameterNames.Value, Value);
            validParameters.Add(ParameterNames.ValueChanged, EventCallback.Factory.Create<object?>(this, ValueUpdate));
            validParameters.Add(ParameterNames.OnSubmitted, EventCallback.Factory.Create<object?>(this, Submitted));
            validParameters.Add(ParameterNames.Components, Components);
            validParameters.Add(ParameterNames.Parent, this);
            validParameters.Add(ParameterNames.EditMode, EditMode);
            validParameters.Add(ParameterNames.FormModel, formModel);
            validParameters.Add(ParameterNames.IteratorPath, IteratorPath);

            return validParameters;
        }

        public Dictionary<string, object> GetValidParametersForChild(DynamicFormModel formModel, int childIndex)
        {
            var validParameters = GetValidParametersFor(formModel);
            var childPosition = string.IsNullOrEmpty(TreePosition)
                ? $"{childIndex + 1}"
                : $"{TreePosition}.{childIndex + 1}";
            validParameters[ParameterNames.TreePosition] = childPosition;
            return validParameters;
        }

        private void ValueUpdate(object value)
        {
            ValueChanged.InvokeAsync(Value);
        }

        private void Submitted(object value)
        {
            OnSubmitted.InvokeAsync(Value);
        }

        protected void EvaluateRules()
        {
            RuleEffect = RuleEvaluator.Resolve(FormModel.Rules, Value);
        }

        internal List<KeyValuePair<string, object>> IncludeDynamicOptionsInParameters(Dictionary<string, object> parameters)
        {
            return parameters == null ? new List<KeyValuePair<string, object>>() : parameters.ToList();
        }

        internal Type componentType => GetComponentType();
        internal Type GetComponentType()
        {
            if (FormModel == null)
            {
                return typeof(NotFoundComponent);
            }
            return GetComponentTypeFor(FormModel);
        }

        public Type GetComponentTypeFor(DynamicFormModel formModel)
        {
            if (!Components.ContainsKey(formModel.DynamicType))
            {
                return GetLessSpefificType(formModel);
            }
            return Components[formModel.DynamicType];
        }

        public Dictionary<string, object> validParameters => GetValidParameters();

        private Type GetLessSpefificType(DynamicFormModel formModel)
        {
            var newKey = new ComponentSelectionKey(formModel.DynamicType.ComponentType);
            if (!Components.ContainsKey(newKey))
            {
                return typeof(NotFoundComponent);
            }
            return Components[newKey];
        }
    

        protected override void OnAfterRender(bool firstRender)
        {
            EvaluateRules();
            base.OnAfterRender(firstRender);
        }
    }
}
