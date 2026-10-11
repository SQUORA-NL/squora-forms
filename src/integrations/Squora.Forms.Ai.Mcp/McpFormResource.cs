using System.Text.Json;

namespace Squora.Forms.Ai.Mcp;

/// <summary>
/// Represents a Hub form exposed as an MCP resource.
/// URI pattern: squora-forms://forms/{tenantId}/{slug}:latest@published
/// </summary>
public class McpFormResource
{
    public string Uri { get; set; } = "";
    public string Name { get; set; } = "";
    public string Description { get; set; } = "";
    public string MimeType { get; set; } = "application/json";
    public JsonElement Schema { get; set; }
    public string SchemaVersion { get; set; } = ""; // SHA-256 fingerprint
}
