using Squora.Forms.Core.Helpers;
using Squora.Forms.Blazor.Components;
using Microsoft.AspNetCore.Components;

namespace Squora.Forms.Blazor.Components.Input
{
    public class NumberComponentBase : InputFormComponent
    {
        [Parameter]
        public string Label { get; set; }


        [Parameter]
        public double Minimum { get; set; } = double.MinValue;

        [Parameter]
        public double Maximum { get; set; } = double.MaxValue;

        public override string ValidationString => TokenReplacer.ReplaceTokens(InvalidMessage, this);

        public double _numberValue = 0;
        public double NumberValue { get { return _numberValue; } set { _numberValue = value; ComponentLogic.UpdateValue(this, _numberValue); } }
        protected override void OnParametersSet()
        {
            try
            {
                _numberValue = Convert.ToDouble(GetValue());
            }
            catch (Exception)
            {
                _numberValue = 0;
                IsValid = false;
            }
        }

        public override void Validate()
        {

            if (_numberValue < Minimum || _numberValue > Maximum)
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