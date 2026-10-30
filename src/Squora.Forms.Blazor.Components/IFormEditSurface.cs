namespace Squora.Forms.Blazor.Components
{
    /// <summary>
    /// The extension point for a form editor. The runtime renders and validates forms and knows nothing about editing them; an editor
    /// (the Squora Forms Designer) registers one of these as a service, and the runtime then lets it draw the parts an editor needs.
    /// Without one registered, a form renders as it always did, whatever <see cref="FormComponentBase.EditMode"/> says.
    /// </summary>
    public interface IFormEditSurface
    {
        /// <summary>
        /// The component that draws the wrapper around every element of a form, in place of the plain one (it takes selection, drop targets,
        /// the handle to move an element by and so on). It derives from <see cref="ElementWrapperBase"/>, which keeps what every wrapper does:
        /// validation, the class and style of the element and its parameters.
        /// </summary>
        Type ElementWrapperType { get; }

        /// <summary>
        /// The component shown in a layout that has no elements yet, while editing (a place to drop one). It takes a
        /// <see cref="FormComponentBase"/> parameter named <c>Container</c>: the layout it stands in for.
        /// </summary>
        Type EmptyLayoutType { get; }
    }
}
