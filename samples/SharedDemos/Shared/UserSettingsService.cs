using Squora.Forms.Core.Models;
using Squora.Forms.Blazor.HtmlComponents.Extensions;

namespace SharedDemos.Shared
{
    public class UserSettingsService
    {
        public string SampleJson { get; set; } = string.Empty;

        public ComponentsList Components { get; private set; } = new HtmlComponentsProvider().GetComponents();

        public Boolean EditMode { get; set; } = false;

        public event Action OnSettingsChanged;

        public void UpdateComponents(ComponentsList components)
        {
            Components = components;
            OnSettingsChanged?.Invoke();
            
        }
    }
}
