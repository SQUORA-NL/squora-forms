using Squora.Forms.Common.Enums;
using Squora.Forms.Blazor.Components.Input;
using Squora.Forms.Core.Models;
using Squora.Forms.Core.Models.ParameterModels;

namespace Squora.Forms.Blazor.Components.Builders
{
    public class DropDownComponentBuilder : FormComponentBuilder<DropDownComponentBase, DropDownComponentBuilder>
    {
        public DropDownComponentBuilder WithLabel(string label)
        {
            _component.Label = label;
            return this;
        }

        public DropDownComponentBuilder WithOptions(Dictionary<object, string> options)
        {
            _component.Options = options;
            return this;
        }

        public DropDownComponentBuilder WithSelectedValue(object selectedValue)
        {
            _component.SelectedValue = selectedValue;
            return this;
        }

        public new DynamicFormModel Build()
        {
            var model = base.Build();
            model.DynamicType = new ComponentSelectionKey(ComponentType.Dropdown);
            model.Parameters.Add(ParameterNames.Label, _component.Label);
            model.Parameters.Add(ParameterNames.Options, _component.Options);
            model.Parameters.Add(ParameterNames.SelectedValue, _component.SelectedValue);
            return model;
        }
    }
}
