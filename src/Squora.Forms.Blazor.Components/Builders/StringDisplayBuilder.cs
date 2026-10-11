using Squora.Forms.Blazor.Components.Content;
using Squora.Forms.Common.Enums;
using Squora.Forms.Core.Models;
using Squora.Forms.Core.Models.ParameterModels;

namespace Squora.Forms.Blazor.Components.Builders
{
    public class StringDisplayBuilder : FormComponentBuilder<StringDisplayBase, StringDisplayBuilder>
    {
        public StringDisplayBuilder WithLabel(string label)
        {
            _component.Label = label;
            return this;
        }


        public new DynamicFormModel Build()
        {
            var model = base.Build();
            model.DynamicType = new ComponentSelectionKey(ComponentType.StringDisplay);
            model.Parameters.Add(ParameterNames.Label, _component.Label);
            return model;
        }
    }
}
