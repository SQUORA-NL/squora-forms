using Squora.Forms.Common.Enums;
using Squora.Forms.Blazor.Components.Input;
using Squora.Forms.Core.Models;
using Squora.Forms.Core.Models.ParameterModels;

namespace Squora.Forms.Blazor.Components.Builders
{
    public class BooleanComponentBuilder : FormComponentBuilder<BooleanComponentBase, BooleanComponentBuilder>
    {
        public BooleanComponentBuilder WithLabel(string label)
        {
            _component.Label = label;
            return this;
        }

        public BooleanComponentBuilder WithNeedsToBeChecked(bool needsToBeChecked)
        {
            _component.NeedsToBeChecked = needsToBeChecked;
            return this;
        }

        public new DynamicFormModel Build()
        {
            var model = base.Build();
            model.DynamicType = new ComponentSelectionKey(ComponentType.Boolean);
            model.Parameters.Add(ParameterNames.Label, _component.Label);
            model.Parameters.Add(ParameterNames.NeedsToBeChecked, _component.NeedsToBeChecked);
            return model;
        }
    }
}
