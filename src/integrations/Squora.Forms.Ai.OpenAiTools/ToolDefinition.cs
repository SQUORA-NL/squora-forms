using System.Text.Json;
using System.Text.Json.Nodes;

namespace Squora.Forms.Ai.OpenAiTools;

/// <summary>
/// Provider-agnostic intermediate representation of a form as an agent tool.
/// Serialises to OpenAI or Anthropic wire format.
/// </summary>
public class ToolDefinition
{
    public string FormId { get; set; } = "";
    public string Name { get; set; } = "";
    public string Description { get; set; } = "";
    public JsonElement InputSchema { get; set; }
    public string SchemaVersion { get; set; } = ""; // SHA-256 fingerprint

    /// <summary>
    /// Serialize to OpenAI function-calling format:
    /// { "type": "function", "function": { "name": "...", "description": "...", "parameters": { ...schema } } }
    /// </summary>
    public JsonObject ToOpenAiJson()
    {
        var func = new JsonObject
        {
            ["name"] = Name,
            ["description"] = Description,
            ["parameters"] = JsonNode.Parse(InputSchema.GetRawText())
        };
        return new JsonObject
        {
            ["type"] = "function",
            ["function"] = func
        };
    }

    /// <summary>
    /// Serialize to Anthropic tool-use format:
    /// { "name": "...", "description": "...", "input_schema": { ...schema } }
    /// </summary>
    public JsonObject ToAnthropicJson()
    {
        return new JsonObject
        {
            ["name"] = Name,
            ["description"] = Description,
            ["input_schema"] = JsonNode.Parse(InputSchema.GetRawText())
        };
    }
}
