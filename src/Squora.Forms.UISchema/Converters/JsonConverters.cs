using Squora.Forms.UISchema.Constants;
using Squora.Forms.UISchema.Models;
using Newtonsoft.Json.Linq;

namespace Squora.Forms.UISchema.Converters
{
    internal class FormUISchemaConverters
    {

        public static ControlOptions ConvertControlOptions(JToken? jtoken)
        {
            if (jtoken == null) { return ControlOptions.Default(); }
            var result = new ControlOptions()
            .WithReadonly(GetControlProperty<bool>(UISchemaConstants.Readonly, jtoken))
            .WithShowSortButtons(GetControlProperty<bool>(UISchemaConstants.ShowSortButtons, jtoken))
            .WithElementLabelProperty(GetControlProperty<string>(UISchemaConstants.ElementLabelProp, jtoken) ?? string.Empty)
            .WithFormat(GetControlProperty<string>(UISchemaConstants.Format, jtoken) ?? string.Empty)
            ;
            return result;
        }

        public static (string Label, bool ShowLabel) ConvertToControlLabel(JToken? jToken)
        {
            if (jToken == null) return (string.Empty, false);

            if (jToken.Type == JTokenType.Boolean)
            {
                return (string.Empty, false);
            }

            return (jToken.ToObject<string>(), true);
        }

        private static TType? GetControlProperty<TType>(string name,JToken jtoken)
        {
            if (jtoken[name] == null) { return default; }
            return jtoken[name].ToObject<TType>();
        }
    }
}
