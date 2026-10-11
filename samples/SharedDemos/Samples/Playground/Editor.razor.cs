using Squora.Forms.Core.Models;
using Squora.Forms.Blazor.Components.Factories;
using Microsoft.AspNetCore.Components;
using Microsoft.JSInterop;
using Newtonsoft.Json;
using System.Dynamic;

namespace SharedDemos.Samples.Playground
{
    public partial class Editor : SampleBasePage
    {
        [Inject] private IJSRuntime JS { get; set; } = null!;

        private bool EditModeIndicator = true;
        private void update()
        {
            StateHasChanged();
        }

        private async Task OpenInEditor()
        {
            var json = JsonConvert.SerializeObject(model, Formatting.Indented);
            await JS.InvokeVoidAsync("localStorage.setItem", "squora-forms-editor-model", json);
            await JS.InvokeVoidAsync("open", "/editor/form", "_blank");
        }

        public ExpandoObject data = new ExpandoObject();

        public DynamicFormModel model = FormFactory.VerticalLayout(
            FormFactory.GroupLayout(group =>
                group
                   .WithParameter("Label", "Personal Information")
                   .WithElements(
                        FormFactory.StringComponent("First Name", "$.firstName"),
                        FormFactory.StringComponent("Last Name", "$.lastName"),
                        FormFactory.StringComponent("Email", "$.email")
                   )
                ),
            FormFactory.GroupLayout(group =>
                group
                   .WithParameter("Label", "Address Information")
                   .WithElements(
                        FormFactory.StringComponent("Street", "$.address.street"),
                        FormFactory.StringComponent("City", "$.address.city"),
                        FormFactory.StringComponent("ZIP Code", "$.address.zip")
                   )
                ),
            FormFactory.HorizontalLayout(
                FormFactory.BoolComponent("Subscribe to Newsletter", "$.subscribe"),
                FormFactory.BoolComponent("Accept Terms", "$.acceptTerms")
            )
        );
    }
}
