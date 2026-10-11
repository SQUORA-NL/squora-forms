using Squora.Forms.Common.Enums;
using Squora.Forms.Blazor.Components.Layout;
using Squora.Forms.Core.Models;
using Squora.Forms.Core.Models.ParameterModels;

namespace Squora.Forms.Blazor.Components.Builders
{
    public class GroupLayoutBuilder : FormComponentBuilder<GroupLayoutBase, GroupLayoutBuilder>
    {
        public GroupLayoutBuilder WithLabel(string label)
        {
            _component.Label = label;
            return this;
        }

        public new DynamicFormModel Build()
        {
            var model = base.Build();
            model.DynamicType = new ComponentSelectionKey(ComponentType.GroupLayout);
            model.Parameters.Add(ParameterNames.Label, _component.Label);
            return model;
        }
    }
}