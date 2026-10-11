
using Microsoft.AspNetCore.Components;

namespace Squora.Forms.Blazor.Components
{
    public abstract class InputFormComponent : FormComponentBase
    {
        [Parameter] public bool Required { get; set; }

        public object? InputValue { get; set; }

        protected override void OnParametersSet()
        {
            base.OnParametersSet();
        }

     
    }
}
