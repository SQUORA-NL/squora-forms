using Squora.Forms.Common.Enums;

namespace Squora.Forms.JsonSchema.Models;

public class DefaultSchemaItem : SchemaItem
{
    public DefaultSchemaItem(string path) : base(ComponentType.Default, path)
    {
    }
}