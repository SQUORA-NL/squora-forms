using Squora.Forms.Core.Models;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;

namespace Squora.Forms.Core.Parser
{
    public class DynamicFormModelParser
    {
        private JsonSerializerSettings settings = new JsonSerializerSettings
        {
            Converters = new List<JsonConverter> {
                new OptionsDictionaryConverter(),
                new ParameterListConverter(),
                new StringEnumConverter()
                },
            Formatting = Formatting.Indented,
            NullValueHandling = NullValueHandling.Ignore,
            ContractResolver = new SkipEmptyCollectionsContractResolver()
        };

        public string Serialize(DynamicFormModel dynamicFormModel)
        {
            return JsonConvert.SerializeObject(dynamicFormModel, settings);
        }

        public DynamicFormModel Deserialize(string jsonModel)
        {
            try
            {
                return JsonConvert.DeserializeObject<DynamicFormModel>(jsonModel, settings);
            }
            catch (Exception)
            {
                return null;
            }
        }
    }
}
