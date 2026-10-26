using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System.Dynamic;
using Squora.Forms.Core.Models.ParameterModels;

namespace Squora.Forms.Core.Parser
{
    public class ParameterListConverter : JsonConverter
    {
        public override bool CanConvert(Type objectType)
        {
            return objectType == typeof(ParameterList);
        }

        public override void WriteJson(JsonWriter writer, object value, JsonSerializer serializer)
        {
            var parameterList = value as ParameterList;
            if (parameterList != null && parameterList.Entries != null && parameterList.Entries.Count != 0)
            {
                var jObject = new JObject();
                foreach (var kvp in parameterList?.Entries)
                {
                    if (kvp.Key != null && kvp.Value != null)
                    {
                        jObject.Add(new JProperty(kvp.Key, JToken.FromObject(kvp.Value, serializer)));
                    }
                }
                jObject.WriteTo(writer);
            }
        }

        public override object ReadJson(JsonReader reader, Type objectType, object existingValue, JsonSerializer serializer)
        {
            var jObject = JObject.Load(reader);
            var parameters = new Dictionary<string, object>();

            foreach (var property in jObject.Properties())
            {
                if (property.Name == "Options")
                {
                    parameters.Add(property.Name, ConvertJTokenToObject(property.Value));
                }
                else if (property.Name == "DefaultValue")
                {
                    parameters.Add(property.Name, ConvertJTokenToExpando(property.Value));
                }
                else if (property.Value is JObject nested && nested.ContainsKey("DynamicType"))
                {
                    // a parameter that holds a form model of its own (like a group's HeaderModel) is written as a nested model
                    parameters.Add(property.Name, nested.ToObject<Models.DynamicFormModel>(serializer));
                }
                else
                {
                    parameters.Add(property.Name, property.Value.ToObject<object>(serializer));
                }
            }

            return new ParameterList(parameters);
        }


        public static Dictionary<object, string> ConvertJObjectToDictionary(JObject jObject)
        {
            var result = new Dictionary<object, string>();
            foreach (var property in jObject.Properties())
            {
                result[property.Name] = ConvertJTokenToObject(property.Value)?.ToString();
            }
            return result;
        }

        private static object ConvertJTokenToObject(JToken token)
        {
            switch (token.Type)
            {
                case JTokenType.Object:
                    return ConvertJObjectToDictionary(token as JObject);
                case JTokenType.Array:
                    var list = new List<object>();
                    foreach (var item in token.Children())
                    {
                        list.Add(ConvertJTokenToObject(item));
                    }
                    return list;
                default:
                    return token.ToObject<object>();
            }
        }

        public static ExpandoObject ConvertJTokenToExpando(JToken token)
        {
            if (token.Type == JTokenType.Object)
            {
                var expando = new ExpandoObject() as IDictionary<string, object>;
                foreach (var property in token.Children<JProperty>())
                {
                    expando.Add(property.Name, ConvertJTokenToExpandoOrValue(property.Value));
                }
                return expando as ExpandoObject;
            }
            else
            {
                return token.ToObject<ExpandoObject>();
            }
        }

        private static object ConvertJTokenToExpandoOrValue(JToken token)
        {
            switch (token.Type)
            {
                case JTokenType.Object:
                    return ConvertJTokenToExpando(token);

                case JTokenType.Array:
                    return token.Children().Select(ConvertJTokenToExpandoOrValue).ToList();

                case JTokenType.Integer:
                    return token.Value<long>();

                case JTokenType.Float:
                    return token.Value<double>();

                case JTokenType.String:
                    return token.Value<string>();

                case JTokenType.Boolean:
                    return token.Value<bool>();

                case JTokenType.Date:
                    return token.Value<DateTime>();

                case JTokenType.Null:
                    return null;

                case JTokenType.Undefined:
                    return null;

                // Add additional cases for other JTokenType if needed

                default:
                    return token.ToString();
            }
        }

    }
}
