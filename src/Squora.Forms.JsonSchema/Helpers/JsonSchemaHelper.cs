using Squora.Forms.Common.Enums;
using Squora.Forms.JsonSchema.Constants;
using Squora.Forms.JsonSchema.Models;
using Newtonsoft.Json.Linq;
using Newtonsoft.Json.Schema;

namespace Squora.Forms.JsonSchema.Helpers;

public static class JsonSchemaHelper
{
    public  static ISchemaItem ExtractSchemaInfo(JToken token)
    {
        var type = token["type"].ToObject<JSchemaType>();
        switch (type)
        {
            case JSchemaType.Integer:
                return GetNumber(token, ComponentType.Int);
            case JSchemaType.Number:
                return GetNumber(token, ComponentType.Number);
            case JSchemaType.String:
                return GetString(token);
            case JSchemaType.Boolean:
                return GetBoolean(token);
            case JSchemaType.Array:
                return IsFile(token["items"]) ? GetFile(token, token["items"], multiple: true) : GetArray(token);
            case JSchemaType.Object:
                return IsFile(token) ? GetFile(token, token, multiple: false) : GetObject(token);
                default:
                    return new DefaultSchemaItem(token.Path);
        }
    }

    private static ISchemaItem GetNumber(JToken token, ComponentType type)
    {
        return new NumberSchemaItem(type, token.Path)
        {
            Minimum = token[JsonSchemaConstants.Minimum]?.ToObject<double>(),
            Maximum = token[JsonSchemaConstants.Maximum]?.ToObject<double>(),
            ExclusiveMinimum = token[JsonSchemaConstants.ExclusiveMinimum]?.ToObject<bool>(),
            ExclusiveMaximum = token[JsonSchemaConstants.ExclusiveMaximum]?.ToObject<bool>(),
            MultipleOf = token[JsonSchemaConstants.MultipleOf]?.ToObject<double>(),
        };
    }
    
    private static ISchemaItem GetString(JToken token)
    {
        return new StringSchemaItem(token.Path)
        {
            MaximumLength = token[JsonSchemaConstants.MaximumLength]?.ToObject<int>(),
            MinimumLength = token[JsonSchemaConstants.MinimumLength]?.ToObject<int>(),
            Pattern = token[JsonSchemaConstants.Pattern]?.ToObject<string>(),
            Format = token[JsonSchemaConstants.Format]?.ToObject<string>()
        };
    }
    
    private static bool IsFile(JToken? token)
    {
        return token is JObject && token["type"]?.ToString() == "object"
            && token[JsonSchemaConstants.Format]?.ToString() == JsonSchemaConstants.FileFormat;
    }

    // outer is the property itself (the array for several files), file is the schema of one file.
    private static ISchemaItem GetFile(JToken outer, JToken file, bool multiple)
    {
        var item = new FileSchemaItem(outer.Path)
        {
            Multiple = multiple,
            MinimumFiles = multiple ? outer[JsonSchemaConstants.MinimumItems]?.ToObject<int>() : null,
            Accept = file[JsonSchemaConstants.Accept]?.ToObject<string>()
        };
        if (multiple && outer[JsonSchemaConstants.MaximumItems] != null)
            item.MaximumFiles = outer[JsonSchemaConstants.MaximumItems].ToObject<int>();
        if (file[JsonSchemaConstants.MaximumFileSize] != null)
            item.MaximumFileSize = file[JsonSchemaConstants.MaximumFileSize].ToObject<long>();
        return item;
    }

    private static ISchemaItem GetBoolean(JToken token)
    {
        return new BoolSchemaItem(token.Path);
    }
    
    private static ISchemaItem GetObject(JToken token)
    {
        return new ObjectSchemaItem(token.Path)
        {
            RequiredElements = token[JsonSchemaConstants.Required]?.ToObject<List<string>>(),
            MinimumProperties = token[JsonSchemaConstants.MinimumProperties]?.ToObject<int>(),
            MaximumProperties = token[JsonSchemaConstants.MaximumProperties]?.ToObject<int>()
        };
    }
    
    private static ISchemaItem GetArray(JToken token)
    {
        return new ArraySchemaItem(token.Path)
        {
            UniqueItems = token[JsonSchemaConstants.UniqueItems]?.ToObject<bool>(),
            MinimumItems = token[JsonSchemaConstants.MinimumItems]?.ToObject<int>(),
            MaximumItems = token[JsonSchemaConstants.MaximumItems]?.ToObject<int>()
        };
    }
}