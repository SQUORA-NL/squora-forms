using Squora.Forms.Common.Enums;
using Squora.Forms.Blazor.Components.Input;
using Squora.Forms.Core.Models;
using Squora.Forms.Core.Models.ParameterModels;

namespace Squora.Forms.Blazor.Components.Builders
{
    public class IntComponentBuilder : FormComponentBuilder<IntComponentBase, IntComponentBuilder>
    {
        public IntComponentBuilder WithLabel(string label)
        {
            _component.Label = label;
            return this;
        }

        public IntComponentBuilder WithMinimum(int minimum)
        {
            _component.Minimum = minimum;
            return this;
        }

        public IntComponentBuilder WithMaximum(int maximum)
        {
            _component.Maximum = maximum;
            return this;
        }

        public new DynamicFormModel Build()
        {
            var model = base.Build();
            model.DynamicType = new ComponentSelectionKey(ComponentType.Int);
            model.Parameters.Add(ParameterNames.Label, _component.Label);
            model.Parameters.Add(ParameterNames.Minimum, _component.Minimum);
            model.Parameters.Add(ParameterNames.Maximum, _component.Maximum);
            return model;
        }
    }

}
