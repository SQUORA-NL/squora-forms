using Squora.Forms.Core.Helpers;
using Squora.Forms.Blazor.Components;
using Microsoft.AspNetCore.Components;

namespace Squora.Forms.Blazor.Components.Input
{
    public class IntComponentBase : InputFormComponent
    {
        [Parameter]
        public string Label { get; set; }

        [Parameter]
        public int Minimum { get; set; } = int.MinValue;

        [Parameter]
        public int Maximum { get; set; } = int.MaxValue;

        public override string ValidationString => TokenReplacer.ReplaceTokens(InvalidMessage, this);

        public int _numberValue = 0;
        public int NumberValue { get { return _numberValue; } set { _numberValue = value; ComponentLogic.UpdateValue(this, _numberValue); } }
        protected override void OnParametersSet()
        {
            base.OnParametersSet();
            try
            {
                _numberValue = Convert.ToInt32(GetValue() ?? 0);
            }
            catch (Exception)
            {
                _numberValue =0;
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