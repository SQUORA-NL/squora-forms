using Microsoft.AspNetCore.Components;
using Squora.Forms.Blazor.Services;

namespace Squora.Forms.Blazor.Components
{
    /// <summary>
    /// What every wrapper around an element of a form does: it takes the element's class, style and settings, registers it for validation,
    /// validates it when it loses focus and shows its message. <see cref="PlainElementWrapper"/> is the wrapper of a plain form; an editor
    /// brings its own, derived from this one (see <see cref="IFormEditSurface"/>).
    /// </summary>
    public abstract class ElementWrapperBase : ComponentBase, IHandleEvent, IDisposable
    {
        [Parameter] public string? Class { get; set; }
        [Parameter] public RenderFragment? ChildContent { get; set; }
        [Parameter] public FormComponentBase? FormComponent { get; set; }
        [Parameter] public bool ShowValidation { get; set; } = true;

        /// <summary>Taken from the form component: is the form being edited.</summary>
        public bool EditMode { get; protected set; }

        [CascadingParameter]
        public ValidationStateService? ValidationState { get; set; }

        /// <summary>The classes of the wrapper's element: the one of its place in the layout, and the one set on the element itself.</summary>
        protected virtual string ElementClasses =>
            $"{Class} {FormComponent?.Class} {(FormComponent?.IsDisabled == true ? "bd-disabled" : "")}".Trim();

        protected override void OnInitialized()
        {
            if (ValidationState != null)
            {
                ValidationState.ValidationRequested += OnValidationRequested;
                if (FormComponent != null)
                {
                    ValidationState.Register(FormComponent);
                }
            }
        }

        protected override void OnParametersSet()
        {
            base.OnParametersSet();
            EditMode = FormComponent?.EditMode ?? false;
        }

        // Suppress Blazor's default "re-render after every DOM event" behaviour.
        // Without this, an @onclick on the wrapper div triggers StateHasChanged() after
        // every click on a child Syncfusion control (popup, calendar, dropdown),
        // which re-renders the component mid-open and collapses the popup.
        // Explicit StateHasChanged() calls in individual handlers still work as normal.
        Task IHandleEvent.HandleEventAsync(EventCallbackWorkItem callback, object? arg)
            => callback.InvokeAsync(arg);

        protected void OnFocusOut()
        {
            // Validate on blur but do NOT call StateHasChanged here.
            // Syncfusion appends popup panels to <body> outside this wrapper div,
            // so moving focus into the popup fires onfocusout on the wrapper div.
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

        public virtual void Dispose()
        {
            if (ValidationState != null)
            {
                ValidationState.ValidationRequested -= OnValidationRequested;
                if (FormComponent != null)
                {
                    ValidationState.Unregister(FormComponent);
                }
            }
        }
    }
}
