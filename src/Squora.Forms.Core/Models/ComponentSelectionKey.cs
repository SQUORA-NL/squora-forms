using Squora.Forms.Common.Enums;
using Newtonsoft.Json;

namespace Squora.Forms.Core.Models;

public class ComponentSelectionKey
{
    public ComponentType ComponentType { get; }

    public string TypeDefinitionName { get; }

    [JsonConstructor]
    public ComponentSelectionKey(ComponentType componentType, string? typeDefinitionName = default!)
    {
        ComponentType = componentType;
        TypeDefinitionName = typeDefinitionName ?? string.Empty;
    }

    public override bool Equals(object obj)
    {
        if (obj == null || GetType() != obj.GetType())
        {
            return false;
        }

        ComponentSelectionKey other = (ComponentSelectionKey)obj;
        return ComponentType == other.ComponentType && TypeDefinitionName == other.TypeDefinitionName;
    }

    public override int GetHashCode()
    {
        return (ComponentType, TypeDefinitionName).GetHashCode();
    }
}
