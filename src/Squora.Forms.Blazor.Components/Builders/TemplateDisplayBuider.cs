using Squora.Forms.Blazor.Components.Content;
using Squora.Forms.Common.Enums;
using Squora.Forms.Core.Models;

namespace Squora.Forms.Blazor.Components.Builders
{
    public class TemplateDisplayBuider : FormComponentBuilder<TemplateDisplayBase, TemplateDisplayBuider>
    {
        public TemplateDisplayBuider Path(string path)
        {
            _component.Path = path;
            return this;
        }

        public new DynamicFormModel Build()
        {
            var model = base.Build();
            model.DynamicType = new ComponentSelectionKey(ComponentType.TemplateContent,typeDefinitionName);
            return model;
        }
    }
}
