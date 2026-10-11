using Squora.Forms.UISchema.Enums;

namespace Squora.Forms.UISchema.Models;

/// <summary>
/// Vertical Layouts use the VerticalLayout type and arranges its elements in a vertical fashion, i.e. all elements are placed beneath each other.
/// </summary>
public class VerticalLayoutDescriptionItem : ILayoutDescriptionItem
{
    public LayoutType Type => LayoutType.VerticalLayout;

    public List<ILayoutDescriptionItem> Elements { get; set; }
    public RuleItem? Rule { get; set; }
}