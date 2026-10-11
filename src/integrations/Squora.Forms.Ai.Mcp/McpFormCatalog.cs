using System.Text.Json;

namespace Squora.Forms.Ai.Mcp;

/// <summary>
/// Provides MCP resource listing and reading for Hub forms.
/// Targets MCP 1.0 protocol.
/// </summary>
public class McpFormCatalog
{
    private readonly IHubFormClient _hubClient;

    public McpFormCatalog(IHubFormClient hubClient)
    {
        _hubClient = hubClient;
    }

    /// <summary>
    /// List all published forms as MCP resources.
    /// Maps to MCP resources/list.
    /// </summary>
    public async Task<List<McpFormResource>> ListResourcesAsync(string tenantId)
    {
        var forms = await _hubClient.ListPublishedFormsAsync(tenantId);
        return forms.Select(f => new McpFormResource
        {
            Uri = $"squora-forms://forms/{tenantId}/{f.Slug}:latest@published",
            Name = f.DisplayName,
            Description = $"Form: {f.DisplayName} ({f.Slug})",
            Schema = f.Schema,
            SchemaVersion = f.SchemaVersion
        }).ToList();
    }

    /// <summary>
    /// Read a specific form's schema as an MCP resource.
    /// Maps to MCP resources/read.
    /// </summary>
    public async Task<McpFormResource?> ReadResourceAsync(string tenantId, string slug)
    {
        var form = await _hubClient.GetPublishedFormAsync(tenantId, slug);
        if (form == null) return null;

        return new McpFormResource
        {
            Uri = $"squora-forms://forms/{tenantId}/{slug}:latest@published",
            Name = form.DisplayName,
            Description = $"Form: {form.DisplayName} ({form.Slug})",
            Schema = form.Schema,
            SchemaVersion = form.SchemaVersion
        };
    }
}

/// <summary>
/// Interface for Hub form catalog operations. Implemented by the host.
/// </summary>
public interface IHubFormClient
{
    Task<List<FormDescriptor>> ListPublishedFormsAsync(string tenantId);
    Task<FormDescriptor?> GetPublishedFormAsync(string tenantId, string slug);
}

public class FormDescriptor
{
    public string Slug { get; set; } = "";
    public string DisplayName { get; set; } = "";
    public JsonElement Schema { get; set; }
    public string SchemaVersion { get; set; } = "";
}
