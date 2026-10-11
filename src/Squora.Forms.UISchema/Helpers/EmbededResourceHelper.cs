using System.Reflection;
using Newtonsoft.Json.Schema;

namespace Squora.Forms.UISchema.Helpers;

public static class EmbededResourceHelper
{
    private static string GetEmbeddedResource(string resourceName)
    {
        var assembly = Assembly.GetExecutingAssembly();

        using (Stream stream = assembly.GetManifestResourceStream(resourceName))
        using (StreamReader reader = new StreamReader(stream))
        {
            return reader.ReadToEnd();
        }
    }

    public static JSchema ReadJsonFormsSchema()
    {
        var assembly = Assembly.GetExecutingAssembly();
        string[] resourceNames = assembly.GetManifestResourceNames();
        string schemaResourceName = Array.Find(resourceNames, name => name.EndsWith("jsonFormsSchema.json"));
        var schema = GetEmbeddedResource(schemaResourceName);
        return JSchema.Parse(schema);
    }
}