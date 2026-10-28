using Squora.Forms.Core.Helpers;
using Squora.Forms.Core.Models;
using Squora.Forms.Blazor.Services;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Web;
using Microsoft.JSInterop;

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
            selectionStateService.PropertiesRequested += OnPropertiesRequested;
        }

        protected override void OnParametersSet()
        {
            selectionStateService.RootModel = FormModel;
            base.OnParametersSet();
        }

        [Inject] private IJSRuntime JS { get; set; } = null!;

        private bool dragSupportLoaded;

        protected override async Task OnAfterRenderAsync(bool firstRender)
        {
            // also without the toolbox: the script makes a click on a component's widget select the component
            if (EditMode && !dragSupportLoaded)
            {
                dragSupportLoaded = true;
                try
                {
                    var module = await JS.InvokeAsync<IJSObjectReference>("import", "./_content/Squora.Forms.Blazor/squora-forms-editor.js");
                    await module.InvokeVoidAsync("init");
                }
                catch (Exception e) when (e is JSException or JSDisconnectedException or InvalidOperationException)
                {
                    // the script is a help for Firefox; without it (or without a browser yet) the editor works in the others
                }
            }
            await base.OnAfterRenderAsync(firstRender);
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

        // something asked to see the properties (the action bar of the selected element, a double click)
        private void OnPropertiesRequested()
        {
            if (isPaneCollapsed)
            {
                isPaneCollapsed = false;
                InvokeAsync(StateHasChanged);
            }
        }

        private void TogglePane() => isPaneCollapsed = !isPaneCollapsed;

        // Escape lets go of the selection. On the form and the components next to it, not in the property pane: Escape there belongs to what is being typed.
        private void OnCanvasKeyDown(KeyboardEventArgs e)
        {
            if (EditMode && e.Key == "Escape")
            {
                selectionStateService.ClearSelection();
            }
        }

        public void Dispose()
        {
            selectionStateService.ModelChanged -= OnModelChanged;
            selectionStateService.StateChanged -= OnSelectionChanged;
            selectionStateService.PropertiesRequested -= OnPropertiesRequested;
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
