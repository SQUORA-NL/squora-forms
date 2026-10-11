using Squora.Forms.Blazor.Components.Input;
using Squora.Forms.Common.Enums;
using Squora.Forms.Core.Models;
using Squora.Forms.Core.Models.ParameterModels;

namespace Squora.Forms.Blazor.Components.Builders
{
    public class StringComponentBuilder : FormComponentBuilder<StringComponentBase, StringComponentBuilder>
    {
        public StringComponentBuilder WithLabel(string label)
        {
            _component.Label = label;
            return this;
        }
        public StringComponentBuilder WithMinimumLength(long? minimumLength)
        {
            _component.MinimumLength = minimumLength;
            return this;
        }

        public StringComponentBuilder WithFormat(string format)
        {
            _component.Format = format;
            return this;
        }

        public StringComponentBuilder WithMaximumLength(long? maximumLength)
        {
            _component.MaximumLength = maximumLength;
            return this;
        }

        public StringComponentBuilder WithPattern(string? pattern)
        {
            _component.Pattern = pattern;
            return this;
        }

        public new DynamicFormModel Build()
        {
            var model = base.Build();
            model.DynamicType = new ComponentSelectionKey(ComponentType.String);
            model.Parameters.Add(ParameterNames.Label, _component.Label);
            model.Parameters.Add(ParameterNames.MinimumLength, _component.MinimumLength);
            model.Parameters.Add(ParameterNames.MaximumLength, _component.MaximumLength);
            model.Parameters.Add(ParameterNames.Pattern, _component.Pattern);
            return model;
        }
    }
}
