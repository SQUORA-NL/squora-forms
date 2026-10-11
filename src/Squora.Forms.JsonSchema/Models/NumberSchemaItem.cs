using Squora.Forms.Common.Enums;

namespace Squora.Forms.JsonSchema.Models;

public class NumberSchemaItem : SchemaItem
{
    public NumberSchemaItem(ComponentType type, string path) : base(type, path)
    {
    }
    public double? Minimum { get; set; }
    public double? Maximum { get; set; }
    public bool? ExclusiveMinimum { get; set; }
    public bool? ExclusiveMaximum { get; set; }
    public double? MultipleOf { get; set; }
}