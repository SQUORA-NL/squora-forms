using Microsoft.AspNetCore.Components;
using SharedDemos.Shared;


namespace SharedDemos.Samples
{
    public class SampleBasePage : ComponentBase
    {
        [Inject]
        protected UserSettingsService UserSettingsService { get; set; }

        protected override void OnInitialized()
        {
            UserSettingsService.OnSettingsChanged += OnSettingsChangedHandler;
            base.OnInitialized();
        }

        public void Dispose()
        {
            UserSettingsService.OnSettingsChanged -= OnSettingsChangedHandler;
        }
        private async void OnSettingsChangedHandler()
        {
             await InvokeAsync(StateHasChanged);
        }
    }
}
