using Squora.Forms.Core.Helpers;
using Squora.Forms.Blazor.Components;
using Microsoft.AspNetCore.Components;

namespace Squora.Forms.Blazor.Components.Content
{
    public class StringDisplayBase: DisplayFormComponent
    {
        [Parameter]
        public string Label { get; set; }

        public override string ValidationString => TokenReplacer.ReplaceTokens(InvalidMessage, this);


        public string StringValue { get { return Convert.ToString(GetValue() ?? ""); } }
    }
}