using Squora.Forms.UISchema.Enums;

namespace Squora.Forms.UISchema.Models;

public class GroupDescriptionItem : ILayoutDescriptionItem
{
    public LayoutType Type => LayoutType.Group;

    public List<ILayoutDescriptionItem> Elements { get; set; }
    
    /// <summary>
    /// Defines an additional string that is used to describe the elements of the group
    /// </summary>
    public string Label { get; set; }

    public RuleItem? Rule { get; set; }
}