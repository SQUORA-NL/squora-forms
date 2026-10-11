using Squora.Forms.Core.Models;
using Squora.Forms.Core.Parser;
using Microsoft.AspNetCore.Components;
using Newtonsoft.Json;

namespace SharedDemos.Shared.Components
{
    public partial class SampleDocumentation : Documentation
    {
        private string dataResult { get; set; } = string.Empty;
        private object refreshKey = new object();

        [Parameter]
        public bool ShowPlaygroundButton { get; set; } = true;

        [Inject]
        private NavigationManager navigationManager { get; set; }

        [Inject]
        private UserSettingsService userSettingsService { get; set; }   

        [Parameter]
        public RenderFragment ChildContent { get; set; }

      
        [Parameter]
        public Object Data { get; set; } = string.Empty;

        [Parameter]
        public DynamicFormModel FormModel { get; set; }

     

        protected override async Task OnParametersSetAsync()
        {
            await base.OnParametersSetAsync();    

            var settings = new JsonSerializerSettings
            {
                Formatting = Formatting.Indented, // Beautify the JSON output
                NullValueHandling = NullValueHandling.Ignore // Ignore null values
            };

            dataResult = $"````json\r\n{JsonConvert.SerializeObject(Data, settings)}\r\n````";

            refreshKey = new object(); // This creates a new key, triggering a re-render

        }

        private void GotoPlayground()
        {
            userSettingsService.SampleJson = new DynamicFormModelParser().Serialize(FormModel);
            navigationManager.NavigateTo("/samples/playground/json");
        }

    }
}
