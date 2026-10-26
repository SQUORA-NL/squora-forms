using Squora.Forms.Core.Helpers;
using Squora.Forms.Core.Models;
using Squora.Forms.Blazor.Services;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Web;

namespace Squora.Forms.Blazor
{
    public partial class BDForm : Components.LayoutFormComponent, IDisposable
    {

        [Parameter] public bool ShowEditModeToggle { get; set; } = false;
        [Parameter] public bool ShowToolbox { get; set; } = true;
        [Parameter] public EventCallback<bool> EditModeChanged { get; set; }

        private bool isToolboxCollapsed = false;
        private bool isPaneCollapsed = false;
        private DragStateService dragStateService = new DragStateService();
        private SelectionStateService selectionStateService = new SelectionStateService();
        private ValidationStateService validationStateService = new ValidationStateService();

        protected override void OnInitialized()
        {
            base.OnInitialized();
            // the editor changes the model in place, so the form is told to render it again
            selectionStateService.ModelChanged += OnModelChanged;
            selectionStateService.StateChanged += OnSelectionChanged;
        }

        private void OnModelChanged() => InvokeAsync(StateHasChanged);

        // selecting something is a reason to look at its properties
        private void OnSelectionChanged()
        {
            if (isPaneCollapsed && selectionStateService.SelectedElement != null)
            {
                isPaneCollapsed = false;
                InvokeAsync(StateHasChanged);
            }
        }

        private void TogglePane() => isPaneCollapsed = !isPaneCollapsed;

        public void Dispose()
        {
            selectionStateService.ModelChanged -= OnModelChanged;
            selectionStateService.StateChanged -= OnSelectionChanged;
        }

        public override string ValidationString => TokenReplacer.ReplaceTokens(InvalidMessage, this);

        private async Task OnEditModeChanged(bool newEditMode)
        {
            EditMode = newEditMode;
            
            // Clear selection when exiting edit mode
            if (!newEditMode)
            {
                selectionStateService.ClearSelection();
            }
            
            await EditModeChanged.InvokeAsync(newEditMode);
            StateHasChanged();
        }

        private void OnBackgroundClick(MouseEventArgs e)
        {
            if (EditMode)
            {
                selectionStateService.ClearSelection();
            }
        }

        private void ToggleToolbox()
        {
            isToolboxCollapsed = !isToolboxCollapsed;
            StateHasChanged();
        }
    }
}
