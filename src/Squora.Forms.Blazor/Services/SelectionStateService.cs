using Squora.Forms.Core.Models;
using System;

namespace Squora.Forms.Blazor.Services
{
    public class SelectionStateService
    {
        private DynamicFormModel? _selectedElement = null;

        public event Action? StateChanged;

        public DynamicFormModel? SelectedElement => _selectedElement;

        public void SelectElement(DynamicFormModel? element)
        {
            if (_selectedElement != element)
            {
                _selectedElement = element;
                StateChanged?.Invoke();
            }
        }

        public void ClearSelection()
        {
            if (_selectedElement != null)
            {
                _selectedElement = null;
                StateChanged?.Invoke();
            }
        }

        public bool IsSelected(DynamicFormModel element)
        {
            return _selectedElement == element;
        }
    }
}