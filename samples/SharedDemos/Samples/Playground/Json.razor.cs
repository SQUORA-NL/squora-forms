using Squora.Forms.Core.Models;
using Squora.Forms.Core.Parser;
using Squora.Forms.Blazor.Components.Factories;
using Microsoft.AspNetCore.Components;
using Microsoft.JSInterop;
using MudBlazor;
using SharedDemos.Shared;
using System.Dynamic;

namespace SharedDemos.Samples.Playground
{
    public partial class Json : SampleBasePage
    {
        private string modelJson = "{\r\n  \"DynamicType\": {\r\n    \"ComponentType\": \"GroupLayout\"\r\n  },\r\n  \"Parameters\": {},\r\n  \"Elements\": [\r\n    {\r\n      \"DynamicType\": {\r\n        \"ComponentType\": \"String\"\r\n      },\r\n      \"Parameters\": {\r\n        \"Path\": \"$.FirstName\",\r\n        \"InvalidMessage\": \"Name should be at least {MinimumLength} and max {MaximumLength} long!\",\r\n        \"MinimumLength\": 7,\r\n        \"MaximumLength\": 14\r\n      },\r\n      \"Elements\": []\r\n    },\r\n    {\r\n      \"DynamicType\": {\r\n        \"ComponentType\": \"String\"\r\n      },\r\n      \"Parameters\": {\r\n        \"Path\": \"$.LastName\",\r\n        \"InvalidMessage\": \"Length min {MinimumLength} and max {MaximumLength} long!\",\r\n        \"MinimumLength\": 7,\r\n        \"MaximumLength\": 14\r\n      },\r\n      \"Elements\": []\r\n    }\r\n  ]\r\n}";
        private ExpandoObject data = new ExpandoObject();
        bool hasError = false;

        [Inject]
        ISnackbar? Snackbar { get; set; }
        private void Submit()
        {
            if (Snackbar != null)
            {
                Snackbar.Add("Form is Submitted", Severity.Normal, item => item.VisibleStateDuration = 1000);
            }
        }

        [Inject]
        private IJSRuntime JS { get; set; } = null!;

        [Inject]
        private UserSettingsService userSettingsService { get; set; }

        protected override void OnInitialized()
        {
            base.OnInitialized();
            if (!string.IsNullOrEmpty(userSettingsService.SampleJson))
            {
                modelJson = userSettingsService.SampleJson;
            }
        }

        protected override void OnParametersSet()
        {
            base.OnParametersSet();
            loadModel();
        }

        private void update()
        {
            StateHasChanged();
        }

        private  void loadModel()
        {
            var newmodel = new DynamicFormModelParser().Deserialize(modelJson);
            if(newmodel == null)
            {
             hasError = true;
            }
            else
            {
                hasError = false;
                model = newmodel;
            }
            StateHasChanged();
        }

        private async Task OpenInEditor()
        {
            await JS.InvokeVoidAsync("localStorage.setItem", "squora-forms-editor-model", modelJson);
            await JS.InvokeVoidAsync("open", "/editor/form", "_blank");
        }

        private DynamicFormModel model = FormFactory.GroupLayout();

    }
}
