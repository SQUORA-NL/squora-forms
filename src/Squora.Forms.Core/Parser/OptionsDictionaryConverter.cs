using Newtonsoft.Json.Linq;
using Newtonsoft.Json;

namespace Squora.Forms.Core.Parser;

public class OptionsDictionaryConverter : JsonConverter
{
    public override bool CanConvert(Type objectType)
    {
        // Check if the type is a Dictionary<object, string>
        return objectType == typeof(Dictionary<object, string>);
    }

    public override void WriteJson(JsonWriter writer, object value, JsonSerializer serializer)
    {
        var dictionary = (Dictionary<object, string>)value;
        var jObject = new JObject();
        foreach (var kvp in dictionary)
        {
            jObject.Add(kvp.Key.ToString(), JToken.FromObject(kvp.Value, serializer));
        }
        jObject.WriteTo(writer);
    }

    public override object ReadJson(JsonReader reader, Type objectType, object existingValue, JsonSerializer serializer)
    {
        JObject jObject = JObject.Load(reader);
        var dictionary = new Dictionary<object, string>();
        foreach (var property in jObject.Properties())
        {
            dictionary[property.Name] = property.Value.ToObject<string>();
        }
        return dictionary;
    }
}

