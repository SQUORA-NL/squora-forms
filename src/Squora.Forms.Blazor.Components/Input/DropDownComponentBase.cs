using Squora.Forms.Core.Helpers;
using Squora.Forms.Blazor.Components;
using Microsoft.AspNetCore.Components;

namespace Squora.Forms.Blazor.Components.Input
{
    public class DropDownComponentBase : InputFormComponent
    {
        [Parameter]
        public string Label { get; set; }

        [Parameter]
        public Dictionary<object, string> Options { get; set; } = new Dictionary<object, string>();

        private Dictionary<string, object> _mapping = new Dictionary<string, object>();
        private string _selectedValueId = string.Empty;

        public override string ValidationString => TokenReplacer.ReplaceTokens(InvalidMessage, this);

        public object SelectedValue
        {
            get => !string.IsNullOrEmpty(_selectedValueId) && _mapping.TryGetValue(_selectedValueId, out var value) ? value : null;
            set
            {
                _selectedValueId = value != null
                    ? _mapping.FirstOrDefault(x => x.Value != null && x.Value.Equals(value)).Key ?? string.Empty
                    : string.Empty;
                if (value != null)
                {
                    ComponentLogic.UpdateValue(this, value);
                }
            }
        }

        protected override void OnParametersSet()
        {
            base.OnParametersSet();
            InitializeMapping();
            object currentValue = null;
            try
            {
                currentValue = GetValue();
            }
            catch (Exception)
            {
                IsValid = false;
            }
            if (currentValue != null)
            {
                _selectedValueId = _mapping.FirstOrDefault(x => x.Value != null && x.Value.Equals(currentValue)).Key ?? string.Empty;
            }
        }

        private void InitializeMapping()
        {
            _mapping.Clear();
            foreach (var option in Options)
            {
                var uniqueId = GenerateUniqueId(option.Key);
                _mapping[uniqueId] = option.Key;
            }
        }

        private string GenerateUniqueId(object obj)
        {
            // Using hash code for simplicity; ensure this is unique enough for your use case
            return "unique_" + obj.GetHashCode();
        }

        public void HandleChange(ChangeEventArgs e)
        {
            _selectedValueId = e.Value?.ToString() ?? string.Empty;
            var selectedObject = !string.IsNullOrEmpty(_selectedValueId) && _mapping.TryGetValue(_selectedValueId, out var value) ? value : null;
            SelectedValue = selectedObject; // This will call UpdateValue
        }

        public string GetUniqueId(object key)
        {
            return _mapping.FirstOrDefault(x => x.Value.Equals(key)).Key;
        }
    }
}
