using Microsoft.Extensions.DependencyInjection;

namespace Squora.Forms.Blazor.Extensions
{
    public static class ServiceCollectionExtensions
    {
        /// <summary>
        /// Registers the component registry used to resolve form components.
        /// Pass a configuration action to pick a UI framework provider, e.g. <c>cfg.UseDefaultMudBlazorComponents()</c>.
        /// </summary>
        public static IServiceCollection AddSquoraForms(this IServiceCollection services,
            Action<SquoraFormsConfigBuilder>? configAction = null)
        {
            var builder = new SquoraFormsConfigBuilder();
            configAction?.Invoke(builder);
            builder.Build(services);
            return services;
        }
    }
}
