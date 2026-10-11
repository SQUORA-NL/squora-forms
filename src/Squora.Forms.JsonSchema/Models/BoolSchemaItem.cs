using Squora.Forms.Common.Enums;

namespace Squora.Forms.JsonSchema.Models;

public class BoolSchemaItem : SchemaItem
{
    public BoolSchemaItem(string path) : base(ComponentType.Boolean, path)
    {
    }
}