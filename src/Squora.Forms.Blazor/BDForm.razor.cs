using Squora.Forms.Core.Helpers;
using Squora.Forms.Core.Models;
using Squora.Forms.Blazor.Services;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Web;

namespace Squora.Forms.Blazor
{
    public partial class BDForm : Components.LayoutFormComponent
    {

        [Parameter] public bool ShowEditModeToggle { get; set; } = false;
        [Parameter] public bool ShowToolbox { get; set; } = true;
        [Parameter] public EventCallback<bool> EditModeChanged { get; set; }

        private bool isToolboxCollapsed = false;
        private DragStateService dragStateService = new DragStateService();
        private SelectionStateService selectionStateService = new SelectionStateService();
        private ValidationStateService validationStateService = new ValidationStateService();

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
