using Squora.Forms.UISchema.Enums;

namespace Squora.Forms.UISchema.Models;

/// <summary>
/// Horizontal layouts use the HorizontalLayout type and arranges its contained elements in a horizontal fashion.
/// Each child occupies the same amount of space, i.e. for n children a child occupies 1/n space.
/// </summary>
public class HorizontalLayoutDescriptionItem : ILayoutDescriptionItem
{
    public LayoutType Type => LayoutType.HorizontalLayout;

    public List<ILayoutDescriptionItem> Elements { get; set; }
    public RuleItem? Rule { get; set; }
}