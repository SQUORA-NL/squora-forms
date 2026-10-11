namespace Squora.Forms.Common.Extensions
{
    public static class JsonSchemaPathExtensions
    {
        public static string GetPropertyName(string jsonPath)
        {
            if (string.IsNullOrEmpty(jsonPath))
                return jsonPath;

            string[] parts = jsonPath.Split('.');
            return parts[^1]; // Take the last segment
        }
    }
}
