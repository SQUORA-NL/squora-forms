using Squora.Forms.Common.Enums;

namespace Squora.Forms.JsonSchema.Models;

public interface ISchemaItem
{
    public ComponentType Type { get; }
    
    public Dictionary<string, object> ItemMetadata { get; }
}