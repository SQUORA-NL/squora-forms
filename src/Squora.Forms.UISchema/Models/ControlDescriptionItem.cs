using Squora.Forms.UISchema.Enums;

namespace Squora.Forms.UISchema.Models;

public class ControlDescriptionItem : ILayoutDescriptionItem
{
    public string Scope { get; set; } 
    public string Label { get; set; }
    public bool ShowLabel { get; set; } = true;
    public ControlOptions? Options { get; set; }
    public LayoutType Type => LayoutType.Control;

    public Dictionary<string, object>? ScopeMetadata { get; set; }
    
    public RuleItem? Rule { get; set; }
}