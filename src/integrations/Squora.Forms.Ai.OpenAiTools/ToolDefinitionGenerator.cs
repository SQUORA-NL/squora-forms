using System.Security.Cryptography;
using System.Text;
using System.Text.Json;

namespace Squora.Forms.Ai.OpenAiTools;

public static class ToolDefinitionGenerator
{
    public static IReadOnlyList<ToolDefinition> Generate(IEnumerable<FormDescriptor> forms)
    {
        return forms.Select(Generate).ToList();
    }

    public static ToolDefinition Generate(FormDescriptor form)
    {
        return new ToolDefinition
        {
            FormId = form.Slug,
            Name = SanitizeName(form.Slug),
            Description = $"Fill the '{form.DisplayName}' form.",
            InputSchema = form.Schema,
            SchemaVersion = ComputeFingerprint(form.Schema)
        };
    }

    private static string SanitizeName(string slug)
    {
        return slug.Replace(".", "_").Replace(" ", "_");
    }

    private static string ComputeFingerprint(JsonElement schema)
    {
        var bytes = SHA256.HashData(Encoding.UTF8.GetBytes(schema.GetRawText()));
        return Convert.ToHexStringLower(bytes);
    }
}

public class FormDescriptor
{
    public string Slug { get; set; } = "";
    public string DisplayName { get; set; } = "";
    public JsonElement Schema { get; set; }
}
