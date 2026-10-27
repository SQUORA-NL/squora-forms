using Squora.Forms.Core.Models;
using Squora.Forms.Blazor.Components;
using Microsoft.AspNetCore.Components;
using System;
using System.Linq;

namespace Squora.Forms.Blazor.Services
{
    public class DragStateService
    {
        private bool _isDragging = false;
        private bool _isFromToolbox = false;
        private DynamicFormModel? _draggedElement = null;
        private FormComponentBase? _sourceContainer = null;
        private int _sourceIndex = -1;

        public event Action? StateChanged;

        /// <summary>Raised when the spot the pointer is over changes. Separate from <see cref="StateChanged"/> so only what shows the target has to redraw.</summary>
        public event Action? HoverChanged;

        private object? _hoverOwner;

        /// <summary>The layout a drop at the spot under the pointer would land in.</summary>
        public DynamicFormModel? HoverContainer { get; private set; }

        /// <summary>Says where a drop under the pointer would land, e.g. "above City".</summary>
        public string? HoverHint { get; private set; }

        /// <summary>The spot (<paramref name="owner"/>) under the pointer, and what dropping there does.</summary>
        public void SetHover(object owner, DynamicFormModel? container, string? hint)
        {
            if (_hoverOwner == owner && HoverContainer == container && HoverHint == hint) return;
            _hoverOwner = owner;
            HoverContainer = container;
            HoverHint = hint;
            HoverChanged?.Invoke();
        }

        /// <summary>The pointer left <paramref name="owner"/>. Ignored when it already moved on to another spot (enter of the next arrives before leave of the last).</summary>
        public void ClearHover(object owner)
        {
            if (_hoverOwner != owner) return;
            _hoverOwner = null;
            HoverContainer = null;
            HoverHint = null;
            HoverChanged?.Invoke();
        }

        public bool IsDragging => _isDragging;
        public bool IsFromToolbox => _isFromToolbox;
        public DynamicFormModel? DraggedElement => _draggedElement;
        public FormComponentBase? SourceContainer => _sourceContainer;
        public int SourceIndex => _sourceIndex;

        public void StartDrag(DynamicFormModel element, FormComponentBase sourceContainer, int sourceIndex)
        {
            _isDragging = true;
            _isFromToolbox = false;
            _draggedElement = element;
            _sourceContainer = sourceContainer;
            _sourceIndex = sourceIndex;
            StateChanged?.Invoke();
        }

        public void StartToolboxDrag(DynamicFormModel element)
        {
            _isDragging = true;
            _isFromToolbox = true;
            _draggedElement = element;
            _sourceContainer = null;
            _sourceIndex = -1;
            StateChanged?.Invoke();
        }

        public void EndDrag()
        {
            _hoverOwner = null;
            HoverContainer = null;
            HoverHint = null;
            _isDragging = false;
            _isFromToolbox = false;
            _draggedElement = null;
            _sourceContainer = null;
            _sourceIndex = -1;
            StateChanged?.Invoke();
        }

        public bool CanDrop(FormComponentBase targetContainer)
        {
            // Basic validation - must be dragging with valid elements
            if (!_isDragging || _draggedElement == null || targetContainer.FormModel?.Elements == null)
                return false;

            // For toolbox drags, we can always drop (no circular reference possible)
            if (_isFromToolbox)
                return true;

            // For existing element drags, check for circular references
            if (_sourceContainer == null)
                return false;

            // Prevent dropping an element into itself (circular reference); an empty layout has no children to be found in below
            if (_draggedElement == targetContainer.FormModel)
                return false;

            if (IsElementAncestorOfContainer(_draggedElement, targetContainer))
                return false;

            return true;
        }

        /// <summary>
        /// Checks if a dragged element would create a circular reference when dropped in a target container
        /// </summary>
        private bool IsElementAncestorOfContainer(DynamicFormModel draggedElement, FormComponentBase targetContainer)
        {
            // If the dragged element is a layout component (has children), check if the target container
            // is a descendant of the dragged element
            if (draggedElement.Elements != null && draggedElement.Elements.Any())
            {
                return IsContainerDescendantOfElement(targetContainer, draggedElement);
            }

            return false;
        }

        /// <summary>
        /// Recursively checks if a container is a descendant of a given element
        /// </summary>
        private bool IsContainerDescendantOfElement(FormComponentBase container, DynamicFormModel element)
        {
            if (element.Elements == null) return false;

            // Check if any of the element's children match the container's FormModel
            foreach (var child in element.Elements)
            {
                if (child == container.FormModel)
                    return true;

                // Recursively check child elements
                if (IsContainerDescendantOfElement(container, child))
                    return true;
            }

            return false;
        }

        public bool ExecuteDrop(FormComponentBase targetContainer, int targetIndex)
        {
            if (!CanDrop(targetContainer) || _draggedElement == null)
                return false;

            try
            {
                // Validate target index bounds
                if (targetIndex < 0 || targetIndex > targetContainer.FormModel.Elements.Count)
                    return false;

                // Handle toolbox drops (adding new elements)
                if (_isFromToolbox)
                {
                    return ExecuteToolboxDrop(targetContainer, targetIndex);
                }

                // Handle existing element drops (moving elements)
                if (_sourceContainer == null)
                    return false;

                return ExecuteElementMove(targetContainer, targetIndex);
            }
            catch (Exception)
            {
                // If any exception occurs, the operation failed
                return false;
            }
        }

        private bool ExecuteToolboxDrop(FormComponentBase targetContainer, int targetIndex)
        {
            if (_draggedElement == null)
                return false;

            var targetElements = targetContainer.FormModel.Elements;
            var originalTargetCount = targetElements.Count;

            // Add new element to target
            if (targetIndex >= targetElements.Count)
            {
                targetElements.Add(_draggedElement);
            }
            else
            {
                targetElements.Insert(targetIndex, _draggedElement);
            }

            // Validate the operation was successful
            if (targetElements.Count != originalTargetCount + 1)
                return false;

            // Force re-render and notify of changes
            StateChanged?.Invoke();
            _ = targetContainer.ValueChanged.InvokeAsync(targetContainer.Value);

            return true;
        }

        private bool ExecuteElementMove(FormComponentBase targetContainer, int targetIndex)
        {
            if (_draggedElement == null || _sourceContainer == null)
                return false;

            // Validate source index bounds
            if (_sourceIndex < 0 || _sourceIndex >= _sourceContainer.FormModel.Elements.Count)
                return false;

            // Verify that the element still exists at the source index
            if (_sourceContainer.FormModel.Elements[_sourceIndex] != _draggedElement)
                return false;

            // Create a backup of the original state in case we need to rollback
            var sourceElements = _sourceContainer.FormModel.Elements;
            var targetElements = targetContainer.FormModel.Elements;
            var originalSourceCount = sourceElements.Count;
            var originalTargetCount = targetElements.Count;

            // Adjust target index for same container drops
            var adjustedTargetIndex = targetIndex;
            if (targetContainer == _sourceContainer && _sourceIndex < targetIndex)
            {
                adjustedTargetIndex--;
            }

            // Remove from source
            sourceElements.RemoveAt(_sourceIndex);

            // Add to target
            if (adjustedTargetIndex >= targetElements.Count)
            {
                targetElements.Add(_draggedElement);
            }
            else
            {
                targetElements.Insert(adjustedTargetIndex, _draggedElement);
            }

            // Validate the operation was successful
            bool isValid = true;
            if (targetContainer == _sourceContainer)
            {
                // Same container: element count should remain the same
                isValid = sourceElements.Count == originalSourceCount;
            }
            else
            {
                // Different containers: source should have one less, target should have one more
                isValid = sourceElements.Count == originalSourceCount - 1 && 
                         targetElements.Count == originalTargetCount + 1;
            }

            if (!isValid)
            {
                // Something went wrong, attempt to restore if possible
                return false;
            }

            // Force re-render of both containers through the StateChanged event
            StateChanged?.Invoke();

            // Notify both containers of changes
            _ = _sourceContainer.ValueChanged.InvokeAsync(_sourceContainer.Value);
            if (targetContainer != _sourceContainer)
            {
                _ = targetContainer.ValueChanged.InvokeAsync(targetContainer.Value);
            }

            return true;
        }
    }
}