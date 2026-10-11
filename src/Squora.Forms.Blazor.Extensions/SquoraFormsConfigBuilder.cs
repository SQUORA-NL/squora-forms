using Squora.Forms.Common.Enums;
using Squora.Forms.Core.Contracts;
using Squora.Forms.Core.Models;
using Microsoft.Extensions.DependencyInjection;

namespace Squora.Forms.Blazor.Extensions
{
    public class SquoraFormsConfigBuilder
    {
        private readonly ComponentsList _components = new();

        private readonly List<IComponentProvider> _providers = new();


        public SquoraFormsConfigBuilder RegisterProvider(IComponentProvider componentProvider)
        {
            _providers.Add(componentProvider);
            return this;
        }

        public SquoraFormsConfigBuilder RegisterComponents(IComponentProvider componentProvider)
        {
            RegisterComponents(componentProvider.GetComponents());
            return this;
        }

        public SquoraFormsConfigBuilder RegisterComponents(ComponentsList components)
        {
            foreach (var item in components)
            {
                RegisterComponent(item.Key, item.Value);
            }
            return this;
        }

        public SquoraFormsConfigBuilder RegisterComponent(ComponentSelectionKey key, Type formComponentType)
        {
            _components[key] = formComponentType;
            return this;
        }

        public SquoraFormsConfigBuilder RegisterComponent(ComponentType componentType, string typeDefinitionName, Type formComponentType)
        {
            RegisterComponent(new ComponentSelectionKey(componentType, typeDefinitionName), formComponentType);
            return this;
        }

        public SquoraFormsConfigBuilder RegisterComponent(ComponentType componentType, Type formComponentType)
        {
            RegisterComponent(new ComponentSelectionKey(componentType), formComponentType);
            return this;
        }
        public void Build(IServiceCollection services)
        {
            // Register the ComponentsList as a singleton
            services.AddSingleton(_components);

            foreach (var provider in _providers)
            {
                services.AddScoped(provider.GetType());
            }
        }
    }
}
