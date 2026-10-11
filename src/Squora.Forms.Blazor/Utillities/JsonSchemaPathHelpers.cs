namespace Squora.Forms.Blazor.Utillities
{
    internal class JsonSchemaPathHelpers
    {

        internal static string ConvertSchemaPathToJsonPath(string schemaPath)
        {
            if(schemaPath.StartsWith('$') || schemaPath.StartsWith('@')) { return schemaPath; }
            // Remove the root symbol #
            string jsonPath = schemaPath.TrimStart('#');

            // Replace /properties/ with .
            jsonPath = jsonPath.Replace("/properties", "").Replace("/", ".");

            // Add the root symbol $ at the beginning
            jsonPath = "$" + jsonPath;

            return jsonPath;
        }
    }
}
