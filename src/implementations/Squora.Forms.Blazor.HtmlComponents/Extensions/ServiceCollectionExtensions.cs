using Squora.Forms.Blazor.Extensions;

namespace Squora.Forms.Blazor.HtmlComponents.Extensions
{
    public static class ServiceCollectionExtensions
    {
        public static SquoraFormsConfigBuilder UseDefaultHTMLComponents(this SquoraFormsConfigBuilder builder)
        {
            var provider = new HtmlComponentsProvider();
            builder.RegisterProvider(provider);
            builder.RegisterComponents(provider);
            return builder;
        }
    }
}
