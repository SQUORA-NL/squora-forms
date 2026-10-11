using Squora.Forms.Core.Helpers;
using Squora.Forms.Blazor.Components;
using Squora.Forms.Blazor.Services;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Web;
using System;

namespace Squora.Forms.Blazor.Components.Layout
{
    public class HorizontalLayoutBase : LayoutFormComponent, IDisposable
    {
        [CascadingParameter]
        protected DragStateService? DragState { get; set; }

        public override string ValidationString => TokenReplacer.ReplaceTokens(InvalidMessage, this);

        // Local drag state for UI
        protected int dragOverIndex = -1;

        // Properties to access global drag state
        protected bool isDragging => DragState?.IsDragging ?? false;
        protected bool isDraggedFromThisContainer => DragState?.SourceContainer == this;
        protected int draggedIndex => isDraggedFromThisContainer ? DragState?.SourceIndex ?? -1 : -1;

        protected override void OnInitialized()
        {
            base.OnInitialized();
            if (DragState != null)
            {
                DragState.StateChanged += OnDragStateChanged;
            }
        }

        public virtual void Dispose()
        {
            if (DragState != null)
            {
                DragState.StateChanged -= OnDragStateChanged;
            }
        }

        protected void OnDragStateChanged()
        {
            StateHasChanged();
        }

        protected void OnDragStart(DragEventArgs e, int index)
        {
            if (!EditMode || DragState == null) return;
            
            var elementToDrag = FormModel.Elements[index];
            DragState.StartDrag(elementToDrag, this, index);
        }

        protected void OnDragEnd(DragEventArgs e)
        {
            if (DragState == null) return;
            
            dragOverIndex = -1;
            DragState.EndDrag();
        }

        protected void OnDragOver(DragEventArgs e, int index)
        {
            if (!EditMode || !isDragging || DragState == null) return;
            
            dragOverIndex = index;
            StateHasChanged();
        }

        protected void OnDragLeave(DragEventArgs e)
        {
            dragOverIndex = -1;
            StateHasChanged();
        }

        protected void OnDrop(DragEventArgs e, int dropIndex)
        {
            if (!EditMode || !isDragging || DragState == null) return;

            try
            {
                // Use the global drag state service to handle the drop
                if (DragState.ExecuteDrop(this, dropIndex))
                {
                    // Drop was successful
                }
            }
            catch
            {
                // If anything goes wrong, the drag state service will handle cleanup
            }
            finally
            {
                dragOverIndex = -1;
                StateHasChanged();
            }
        }
    }
}