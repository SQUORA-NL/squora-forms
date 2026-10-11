using Squora.Forms.Common.Enums;
using Squora.Forms.Blazor.Components.Layout;
using Squora.Forms.Core.Models;
using Squora.Forms.Core.Models.ParameterModels;

namespace Squora.Forms.Blazor.Components.Builders
{
    public class ListComponentBuilder : FormComponentBuilder<ListComponentBase, ListComponentBuilder>
    {
        public ListComponentBuilder WithLabel(string label)
        {
            _component.Label = label;
            return this;
        }

        public ListComponentBuilder WithListValue(List<object> listValue)
        {
            _component.ListValue = listValue;
            return this;
        }

        public new DynamicFormModel Build()
        {
            var model = base.Build();
            model.DynamicType = new ComponentSelectionKey(ComponentType.List);
            model.Parameters.Add(ParameterNames.Label, _component.Label);
            model.Parameters.Add(ParameterNames.ListValue, _component.ListValue);
            return model;
        }
    }

}
