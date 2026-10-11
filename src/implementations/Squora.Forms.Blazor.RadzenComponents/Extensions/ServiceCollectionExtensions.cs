using Squora.Forms.Blazor.Extensions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Squora.Forms.Blazor.RadzenComponents.Extensions
{
    public static class ServiceCollectionExtensions
    {
        public static SquoraFormsConfigBuilder UseDefaultRadzenComponents(this SquoraFormsConfigBuilder builder)
        {
            var provider = new RadzenComponentProvider();
            builder.RegisterProvider(provider);
            builder.RegisterComponents(provider);
            return builder;
        }
    }
}
