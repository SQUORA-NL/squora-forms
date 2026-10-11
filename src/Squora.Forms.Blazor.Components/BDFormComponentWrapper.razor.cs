using Squora.Forms.Core;
using Squora.Forms.Blazor.Components;
using Squora.Forms.Blazor.Services;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Web;
using System.Diagnostics;

namespace Squora.Forms.Blazor.Components
{
    public partial class BDFormComponentWrapper : ComponentBase, IHandleEvent, IDisposable
    {
        private string hoverClass = string.Empty;
        [Parameter] public string Class { get; set; } 
        [Parameter] public RenderFragment ChildContent { get; set; }
        [Parameter] public FormComponentBase FormComponent { get; set; }
        [Parameter] public bool ShowValidation { get; set; } = true;
        [Parameter] public bool EditMode { get; set; } = false;

        [CascadingParameter]
        public SelectionStateService? SelectionState { get; set; }

        [CascadingParameter]
        public ValidationStateService? ValidationState { get; set; }

        public bool IsSelected => SelectionState?.IsSelected(FormComponent?.FormModel) ?? false;

        protected override void OnInitialized()
        {
            if (SelectionState != null)
            {
                SelectionState.StateChanged += StateHasChanged;
            }
            if (ValidationState != null)
            {
                ValidationState.ValidationRequested += OnValidationRequested;
            }
        }

        protected override void OnParametersSet()
        {
            base.OnParametersSet();
            EditMode = FormComponent?.EditMode ?? false; 
        }

        private void OnElementClick(MouseEventArgs e)
        {
            if (EditMode && SelectionState != null && FormComponent?.FormModel != null)
            {
                SelectionState.SelectElement(FormComponent.FormModel);
            }
        }

        // Suppress Blazor's default "re-render after every DOM event" behaviour.
        // Without this, @onclick on the wrapper div triggers StateHasChanged() after
        // every click on a child Syncfusion control (popup, calendar, dropdown),
        // which re-renders the component mid-open and collapses the popup.
        // Explicit StateHasChanged() calls in individual handlers still work as normal.
        Task IHandleEvent.HandleEventAsync(EventCallbackWorkItem callback, object? arg)
            => callback.InvokeAsync(arg);

        private void OnFocusOut()
        {
            // Validate on blur but do NOT call StateHasChanged here.
            // Syncfusion appends popup panels to <body> outside this wrapper div,
            // so moving focus into the popup fires onfocusout on the wrapper.
            // A StateHasChanged call here would re-render the child Syncfusion component
            // and collapse the open popup. Validation messages show on the next
            // natural re-render (value change, submit, or parent update).
            FormComponent?.Validate();
        }

        private void OnValidationRequested()
        {
            FormComponent?.Validate();
            StateHasChanged();
        }

        public void Dispose()
        {
            if (SelectionState != null)
            {
                SelectionState.StateChanged -= StateHasChanged;
            }
            if (ValidationState != null)
            {
                ValidationState.ValidationRequested -= OnValidationRequested;
            }
        }
    }
}