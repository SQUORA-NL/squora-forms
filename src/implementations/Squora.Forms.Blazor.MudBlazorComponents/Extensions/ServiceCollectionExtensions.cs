using Squora.Forms.Blazor.Extensions;

namespace Squora.Forms.Blazor.MudBlazorComponents.Extensions
{
    public static class ServiceCollectionExtensions
    {
        public static SquoraFormsConfigBuilder UseDefaultMudBlazorComponents(this SquoraFormsConfigBuilder builder)
        {
            var provider = new MudBlazorComponentProvider();
            builder.RegisterProvider(provider);
            builder.RegisterComponents(provider);
            return builder;
        }
    }
}
