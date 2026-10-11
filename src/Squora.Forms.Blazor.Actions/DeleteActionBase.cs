using Squora.Forms.Core.Helpers;
using Squora.Forms.Blazor.Components;
using Squora.Forms.Blazor.ObjectHandlers;
using Microsoft.AspNetCore.Components;

namespace Squora.Forms.Blazor.Actions
{
    public class DeleteActionBase : InputFormComponent
    {
        [Parameter]
        public string Label { get; set; }

        public override string ValidationString => TokenReplacer.ReplaceTokens(InvalidMessage, this);

        public void DeleteItem()
        {
            DataObjectHelper.Remove(GetInstancePath(Path), Value);
            ValueChanged.InvokeAsync(null);
            // StateHasChanged();
        }
    }
}
