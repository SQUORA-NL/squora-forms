using System.Reflection;
using Squora.Forms.Common.Enums;

namespace Squora.Forms.JsonSchema.Models;

public abstract class SchemaItem : ISchemaItem
{
    public ComponentType Type { get; }
    
    public string Path { get; }
    
    public string PropertyName { get; }
    
    public Dictionary<string, object> ItemMetadata => GetPublicPropertiesAsDictionary(this);
    
    protected SchemaItem(ComponentType type, string path)
    {
        Type = type;
        Path = ("$." + path).Replace(".properties",""); //todo fix this!
        PropertyName = GetPropertyName(Path);
    }
    
    private Dictionary<string, object> GetPublicPropertiesAsDictionary(object obj)
    {
        var dictionary = new Dictionary<string, object>();
        var properties = obj.GetType().GetProperties(BindingFlags.Public | BindingFlags.Instance);

        foreach (var property in properties)
        {
            if (property.Name == nameof(ItemMetadata)) // Prevent infinite recursion
                continue;

            dictionary.Add(property.Name, property.GetValue(obj));
        }

        return dictionary;
    }
    
    static string GetPropertyName(string jsonPath)
    {
        if (string.IsNullOrEmpty(jsonPath))
            return jsonPath;

        string[] parts = jsonPath.Split('.');
        return parts[^1]; // Take the last segment
    }
}