using Squora.Forms.Core.Models;
using Squora.Forms.Blazor.Utillities;
using System;

namespace Squora.Forms.Blazor.Services
{
    public class SelectionStateService
    {
        private DynamicFormModel? _selectedElement = null;

        public event Action? StateChanged;

        /// <summary>Raised when the form model itself was edited (a property, an element removed, the whole form replaced), so the rendered form must refresh.</summary>
        public event Action? ModelChanged;

        /// <summary>Raised when something asks to see (and edit) the properties of the selected element: the pane opens, if it was closed.</summary>
        public event Action? PropertiesRequested;

        public void NotifyModelChanged() => ModelChanged?.Invoke();

        /// <summary>The form being edited. Set by the form, so the actions below can find an element's parent.</summary>
        public DynamicFormModel? RootModel { get; set; }

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

        /// <summary>Set when the properties were asked for, until the pane has moved the cursor to its first input.</summary>
        public bool FocusRequested { get; set; }

        /// <summary>Selects the element and opens its properties.</summary>
        public void EditProperties(DynamicFormModel element)
        {
            FocusRequested = true;
            SelectElement(element);
            PropertiesRequested?.Invoke();
        }

        public bool CanSelectParent => FormTree.FindParent(RootModel, _selectedElement) != null;

        public void SelectParent()
        {
            var parent = FormTree.FindParent(RootModel, _selectedElement);
            if (parent != null)
            {
                SelectElement(parent);
            }
        }

        /// <summary>Deletes the selected element (never the form itself).</summary>
        public bool DeleteSelected()
        {
            if (RootModel == null || _selectedElement == null || _selectedElement == RootModel) return false;

            var parent = FormTree.FindParent(RootModel, _selectedElement);
            if (parent == null) return false;

            parent.Elements.Remove(_selectedElement);
            _selectedElement = null;
            StateChanged?.Invoke();
            NotifyModelChanged();
            return true;
        }

        /// <summary>Copies the selected element, puts the copy after it and selects it.</summary>
        public bool DuplicateSelected()
        {
            if (RootModel == null || _selectedElement == null) return false;

            var copy = FormTree.Duplicate(RootModel, _selectedElement);
            if (copy == null) return false;

            NotifyModelChanged();
            SelectElement(copy);
            return true;
        }

        /// <summary>Adds a new element next to the selection (see <see cref="FormTree.Add"/>) and selects it.</summary>
        public void AddElement(DynamicFormModel element)
        {
            if (RootModel == null) return;

            FormTree.Add(RootModel, _selectedElement, element);
            NotifyModelChanged();
            SelectElement(element);
        }
    }
}
