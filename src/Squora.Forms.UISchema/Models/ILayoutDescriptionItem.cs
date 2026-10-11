using Squora.Forms.UISchema.Enums;

namespace Squora.Forms.UISchema.Models;

public interface ILayoutDescriptionItem
{
    public LayoutType Type { get; }

    public RuleItem? Rule { get; }
}