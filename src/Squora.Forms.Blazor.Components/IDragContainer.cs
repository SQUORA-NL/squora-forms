using Squora.Forms.Core.Models;

namespace Squora.Forms.Blazor.Components
{
    /// <summary>
    /// A layout whose children can be moved around, or dropped into, while the form is in edit mode.
    /// The edit chrome (drag handle, drop targets) is drawn by <see cref="BDEditChrome"/> on top of the
    /// children, so a layout renders its children exactly the same in edit mode as in view mode.
    /// </summary>
    public interface IDragContainer
    {
        bool IsHorizontal { get; }

        /// <summary>The layout itself, so the editor can show which one a drop would land in. Optional: without it only the spot is shown.</summary>
        DynamicFormModel? ContainerModel => null;

        int IndexOf(DynamicFormModel child);

        void BeginDrag(int index);

        void EndDrag();

        void Drop(int index);
    }
}
