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
            if (FormModel.Rules == null || FormModel.Rules.Count == 0) { RuleEffect = RuleEffect.SHOW; return; }
            var result = RuleEffect.SHOW;
            foreach (var item in FormModel.Rules)
            {
                var ruleEvaluationResult = EvaluateRule(item);
                if(result == RuleEffect.SHOW && ruleEvaluationResult == RuleEffect.DISABLE) {  result = RuleEffect.DISABLE; }
                if(ruleEvaluationResult == RuleEffect.HIDE) { result = RuleEffect.HIDE; break; }
            }
            RuleEffect = result;
        }

        private RuleEffect EvaluateRule(DynamicFormModelRule rule)
        {
            var dataJson = JsonConvert.SerializeObject(Value);
            var jsonObject = JObject.Parse(dataJson);
            var value = jsonObject.SelectToken(JsonSchemaPathHelpers.ConvertSchemaPathToJsonPath(rule.Condition.Scope));
            if(value == null) { return RuleEffect.SHOW; }
            var isValid = value.IsValid(JSchema.Parse(rule.Condition.Schema.ToString()), out IList<string> errors);
            if (!isValid)
            {
                return rule.Effect;
            }
            else
            {
                return RuleEffect.SHOW;
            }
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
