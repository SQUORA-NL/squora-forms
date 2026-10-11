using Squora.Forms.Core.Contracts;
using Squora.Forms.Core.Models;
using Squora.Forms.JsonSchema.Implementations;
using Squora.Forms.UISchema.Implementations;
using Microsoft.AspNetCore.Components;
using Newtonsoft.Json.Linq;


namespace Squora.Forms.Blazor
{
    public partial class SquoraForm : ComponentBase
    {
        [Parameter]
        public JObject Schema { get; set; }

        [Parameter]
        public JToken UISchema { get; set; }

        [Parameter]
        public object Data { get; set; }

        [Parameter]
        public EventCallback<object?> ValueChanged { get; set; }

        [Parameter]
        public ComponentsList? Components { get; set; }

        private DynamicFormModel model;

        protected override Task OnParametersSetAsync()
        {
            if (UISchema != null)
            {
                var creator = new DynamicFormModelCreator(new UISchemaParser(new JsonSchemaScopeProvider(new SchemaReader(), Schema)));
                model = creator.GenerateModels(UISchema).First();
            }
            return base.OnParametersSetAsync();
        }
    }
}
