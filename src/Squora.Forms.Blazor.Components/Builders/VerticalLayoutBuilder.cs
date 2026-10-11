using Squora.Forms.Common.Enums;
using Squora.Forms.Blazor.Components.Layout;
using Squora.Forms.Core.Models;

namespace Squora.Forms.Blazor.Components.Builders
{
    public class VerticalLayoutBuilder : FormComponentBuilder<VerticalLayoutBase, VerticalLayoutBuilder>
    {
        public new  DynamicFormModel Build()
        {
            var model = base.Build();
            model.DynamicType = new ComponentSelectionKey(ComponentType.VerticalLayout);
            return model;
        }
    }
}