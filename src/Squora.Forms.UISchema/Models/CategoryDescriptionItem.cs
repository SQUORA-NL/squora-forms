using Squora.Forms.UISchema.Enums;

namespace Squora.Forms.UISchema.Models;

public class CategoryDescriptionItem : ILayoutDescriptionItem
{
    public LayoutType Type => LayoutType.Category;
    /// <summary>
    /// Defines an additional string that is used to describe the elements of the group
    /// </summary>
    public string Label { get; set; }
    
    public IEnumerable<ILayoutDescriptionItem> Elements { get; set; }

    public RuleItem? Rule { get; set; }
}