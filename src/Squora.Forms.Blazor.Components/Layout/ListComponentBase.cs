using Squora.Forms.Core.Helpers;
using Squora.Forms.Core.Models;
using Squora.Forms.Core.Models.ParameterModels;
using Squora.Forms.Blazor.Components;
using Microsoft.AspNetCore.Components;
using System.Collections;

namespace Squora.Forms.Blazor.Components.Layout
{
    public class ListComponentBase :  LayoutFormComponent
    {

        [Parameter]
        public string Label { get; set; }

        public List<object> _listValue = new List<object>();
        public List<object> ListValue { get { return _listValue; } set { _listValue = value; ComponentLogic.UpdateValue(this,_listValue); } }

        public override string ValidationString => TokenReplacer.ReplaceTokens(InvalidMessage, this);

        protected override Task OnInitializedAsync()
        {

            return base.OnInitializedAsync();   
        }
        protected override void OnParametersSet()
        {
            base.OnParametersSet();
            var value = GetValue();

            // Check if the value is already a list.
            if (value is IEnumerable enumerable && !(value is string))
            {
                // Clear the current list of objects.
                _listValue.Clear();

                // Add items from the enumerable to the list of objects.
                foreach (var item in enumerable)
                {
                    _listValue.Add(item);

                }
            }
            else 
            {
                // If not a list, consider it as a single item.
            //    _listValue = new List<object> { value };
            }
        }

        public Dictionary<string, object> GetValidParametersFor(DynamicFormModel element, int index)
        {
            var parameters = GetValidParametersFor(element);

            if (parameters.ContainsKey(ParameterNames.IteratorPath))
            {
                parameters[ParameterNames.IteratorPath] = GetIteratorPath(GetInstancePath(Path), index);
            }
            return parameters;
        }

        private string GetIteratorPath(string listpath, int index)
        {
            return $"{listpath}[{index}]";
        }
    }
}
