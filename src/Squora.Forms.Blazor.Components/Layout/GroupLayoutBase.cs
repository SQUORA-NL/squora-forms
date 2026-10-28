using Squora.Forms.Core.Helpers;
using Microsoft.AspNetCore.Components;

namespace Squora.Forms.Blazor.Components.Layout
{
    public class GroupLayoutBase : LayoutFormComponent
    {
        [Parameter]
        public string Label { get; set; }

        public override string ValidationString => TokenReplacer.ReplaceTokens(InvalidMessage, this);
    }
}
