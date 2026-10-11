using Squora.Forms.Common.Enums;

namespace Squora.Forms.JsonSchema.Models;

public class StringSchemaItem : SchemaItem
{
    public int? MinimumLength { get; set; }
    public int? MaximumLength { get; set; }
    public string? Pattern { get; set; }
    public string? Format { get; set; }
    
    public StringSchemaItem(string path) : base(ComponentType.String, path)
    {
    }
}