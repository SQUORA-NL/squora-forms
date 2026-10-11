using Squora.Forms.Core.Helpers;
using Squora.Forms.Blazor.Components;
using Squora.Forms.Blazor.ObjectHandlers;
using Microsoft.AspNetCore.Components;

namespace Squora.Forms.Blazor.Components.Content
{
    public class NumberDisplayBase: DisplayFormComponent
    {
        [Parameter]
        public string Label { get; set; }

       // public int _numberValue = 0;
        public int NumberValue { get { return CalculateCounter(); } }

        public override string ValidationString => TokenReplacer.ReplaceTokens(InvalidMessage, this);

        private int CalculateCounter()
        {
            ;
            return DataObjectHelper.GetCounterValue(GetInstancePath(Path), Value);
        }
    }
}