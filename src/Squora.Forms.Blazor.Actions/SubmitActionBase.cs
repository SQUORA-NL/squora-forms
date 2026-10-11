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

            // An invalid form is not submitted: the components now show what is wrong. Without this the submit event fired
            // whatever was in the fields, so every consumer had to re-check the data itself.
            if (ValidationState is { AllValid: false })
            {
                return;
            }

            if (OnSubmitted.HasDelegate)
            {
                OnSubmitted.InvokeAsync(GetValue());
            }
        }
    }
}
