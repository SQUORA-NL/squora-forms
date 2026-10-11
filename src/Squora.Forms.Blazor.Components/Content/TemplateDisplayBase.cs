using Squora.Forms.Core.Helpers;
using Squora.Forms.Blazor.Components;
using Squora.Forms.Blazor.ObjectHandlers;

namespace Squora.Forms.Blazor.Components.Content
{
    public class TemplateDisplayBase : DisplayFormComponent
    {
        public override string ValidationString => TokenReplacer.ReplaceTokens(InvalidMessage, this);

        public string StringValue { get { return Convert.ToString(GetValue() ?? ""); } }

        public String GetValue(string path)
        {
            if (Value == null || string.IsNullOrEmpty(path))
            {
                return string.Empty;
            }
          
                return Convert.ToString(DataObjectHelper.GetValue(GetInstancePath(path), Value) ?? "");
        }

        public string GetValue(string path, string format = null)
        {
            if (Value == null || string.IsNullOrEmpty(path))
            {
                return string.Empty;
            }

            object valueObject = DataObjectHelper.GetValue(GetInstancePath(path), Value);

            // Check if valueObject is not null and format string is provided
            if (valueObject != null && !string.IsNullOrEmpty(format))
            {
                // Check the type of valueObject and apply formatting accordingly
                if (valueObject is IFormattable)
                {
                    return ((IFormattable)valueObject).ToString(format, System.Globalization.CultureInfo.CurrentCulture);
                }
                else
                {
                    // If valueObject is not a formattable type, ignore the format string
                    return valueObject.ToString();
                }
            }

            // If no format is provided or valueObject is null, convert the value to a string normally
            return Convert.ToString(valueObject ?? "");
        }



    }
}