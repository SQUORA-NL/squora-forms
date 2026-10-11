using Microsoft.AspNetCore.Components;
using System.Reflection;

namespace SharedDemos.Shared.Components
{
    public partial class Documentation : ComponentBase
    {
        public string markdownContent { get; set; } = string.Empty;

        [Parameter]
        public string MarkDownFile { get; set; } = string.Empty;

        protected override async Task OnInitializedAsync()
        {
            markdownContent = await ReadEmbeddedResourceAsync($"SharedDemos.{MarkDownFile}");
        }

        protected override async Task OnParametersSetAsync()
        {
            if (!string.IsNullOrEmpty(MarkDownFile))
            {
                markdownContent = await ReadEmbeddedResourceAsync($"SharedDemos.{MarkDownFile}");
            }
        }

        internal Task<string> ReadEmbeddedResourceAsync(string resourcePath)
        {
            var assembly = Assembly.GetExecutingAssembly();
            using var stream = assembly.GetManifestResourceStream(resourcePath);
            if (stream == null)
            {
                throw new InvalidOperationException("Could not find the embedded resource '" + resourcePath + "'");
            }
            using var reader = new StreamReader(stream);
            return reader.ReadToEndAsync();
        }
    }
}
