using Squora.Forms.Core.Helpers;
using Squora.Forms.Blazor.Components;
using Squora.Forms.Blazor.Services;
using Microsoft.AspNetCore.Components;

namespace Squora.Forms.Blazor.Actions
{
    public class SubmitActionBase : InputFormComponent
    {
        [Parameter]
        public string Label { get; set; }

        [CascadingParameter]
        public ValidationStateService? ValidationState { get; set; }

        public override string ValidationString => TokenReplacer.ReplaceTokens(InvalidMessage, this);

        public void Submit()
        {
            // Trigger validation on all form components
            ValidationState?.RequestValidation();

            if (OnSubmitted.HasDelegate)
            {
                OnSubmitted.InvokeAsync(GetValue());
            }
        }
    }
}
