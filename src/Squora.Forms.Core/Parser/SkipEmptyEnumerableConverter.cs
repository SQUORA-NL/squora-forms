using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System.Collections;

public class SkipEmptyEnumerableConverter : JsonConverter
{
    public override bool CanConvert(Type objectType)
    {
        return typeof(IEnumerable).IsAssignableFrom(objectType);
    }

    public override bool CanWrite
    {
        get { return true; }
    }

    public override void WriteJson(JsonWriter writer, object value, JsonSerializer serializer)
    {
        var enumerable = value as IEnumerable;
        if (enumerable != null && enumerable.Cast<object>().Any())
        {
            JToken t = JToken.FromObject(value, serializer);
            t.WriteTo(writer);
        }
        else
        {
            writer.WriteNull();
        }
    }

    public override object ReadJson(JsonReader reader, Type objectType, object existingValue, JsonSerializer serializer)
    {
        JToken token = JToken.Load(reader);
        if (token.Type == JTokenType.Array)
        {
            var elementType = objectType.IsArray ? objectType.GetElementType() : objectType.GetGenericArguments()[0];
            var array = token.ToObject(Array.CreateInstance(elementType, 0).GetType(), serializer);
            return typeof(List<>).MakeGenericType(elementType).GetConstructor(new[] { array.GetType() }).Invoke(new[] { array });
        }
        return existingValue ?? Activator.CreateInstance(objectType);
    }

    public override bool CanRead
    {
        get { return true; }
    }
}
