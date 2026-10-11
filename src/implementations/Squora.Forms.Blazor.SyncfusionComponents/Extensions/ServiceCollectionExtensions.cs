using Squora.Forms.Blazor.Extensions;

namespace Squora.Forms.Blazor.SyncfusionComponents.Extensions
{
    public static class ServiceCollectionExtensions
    {
        public static SquoraFormsConfigBuilder UseDefaultSyncfusionComponents(this SquoraFormsConfigBuilder builder)
        {
            var provider = new SyncfusionComponentProvider();
            builder.RegisterProvider(provider);
            builder.RegisterComponents(provider);
            return builder;
        }
    }
}
