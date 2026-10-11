using Squora.Forms.Core.Helpers;
using Squora.Forms.Blazor.Components;
using Microsoft.AspNetCore.Components;

namespace Squora.Forms.Blazor.Components.Input
{
    public class StringComponentBase : InputFormComponent
    {
        [Parameter]
        public string Label { get; set; }
        
        [Parameter]
        public long? MinimumLength { get; set; } 
        
        [Parameter]
        public long? MaximumLength { get; set; } 
        
        [Parameter]
        public string? Pattern { get; set; }

        [Parameter]
        public string? Format { get; set; }

        public string _localString = string.Empty;
        public string StringValue { get { return _localString; } set { _localString = value; ComponentLogic.UpdateValue(this, StringValue); } }

        public override string ValidationString => TokenReplacer.ReplaceTokens(InvalidMessage, this);

        protected override void OnParametersSet()
        {
            _localString = Convert.ToString(GetValue() ?? "");
        }

        public override void Validate()
        {
            
            if (_localString.Length < (MinimumLength ?? 0) || _localString.Length > (MaximumLength ?? long.MaxValue))
            {
                IsValid = false;

            }
            else
            {
                IsValid = true;
            }

            base.Validate();
        }

    }
}