using Squora.Forms.Core.Helpers;
using Squora.Forms.Blazor.Components;
using Microsoft.AspNetCore.Components;

namespace Squora.Forms.Blazor.Components.Input
{
    public class BooleanComponentBase : InputFormComponent
    {
        [Parameter]
        public string Label { get; set; }

        [Parameter]
        public bool? NeedsToBeChecked { get; set; }

        public override string ValidationString => TokenReplacer.ReplaceTokens(InvalidMessage, this);

        public bool? _booleanValue = false;
        public bool? BooleanValue { get { return _booleanValue; } set { _booleanValue = value; ComponentLogic.UpdateValue(this,_booleanValue); } }
        protected override void OnParametersSet()
        {
            base.OnParametersSet();
            try
            {
                _booleanValue = Convert.ToBoolean(GetValue());
            }
            catch (Exception)
            {
                _booleanValue = false;
                IsValid = false;
            }
        }

      
        public override void Validate()
        {

            // Invalid only when the box must be checked and is not. (Without the parentheses this parsed as
            // (!x) ?? (false && needsToBeChecked), which made every unchecked box invalid.)
            if (!(_booleanValue ?? false) && (NeedsToBeChecked ?? false))
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