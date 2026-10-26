using Squora.Forms.Core.Helpers;
using Squora.Forms.Blazor.Components;
using Microsoft.AspNetCore.Components;

namespace Squora.Forms.Blazor.Components.Input
{
    public class DateTimeComponentBase : InputFormComponent
    {
        [Parameter]
        public string Label { get; set; }
        [Parameter]
        public DateTime? MinimumDateTime { get; set; } = DateTime.MinValue;
        [Parameter]
        public DateTime? MaximumDateTime { get; set; } = DateTime.MaxValue;

      


        public DateTime? _localDate = null;
        public DateTime? DateTimeValue { get { return _localDate; } set { _localDate = value; ComponentLogic.UpdateValue(this, _localDate); } }

        public override string ValidationString => TokenReplacer.ReplaceTokens(InvalidMessage, this);


        protected override void OnParametersSet()
        {


            base.OnParametersSet();
            var rawValue = GetValue();
            if (rawValue == null)
            {
                _localDate = null;
                return;
            }
            try
            {
                _localDate = Convert.ToDateTime(rawValue);
            }
            catch (Exception)
            {
                _localDate = null;
                IsValid = false;
            }
        }

        public override void Validate()
        {

            if (_localDate < MinimumDateTime || _localDate > MaximumDateTime)
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