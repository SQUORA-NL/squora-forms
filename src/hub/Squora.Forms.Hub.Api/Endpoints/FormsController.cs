using System.Text.Json;
using System.Text.RegularExpressions;
using Squora.Forms.Hub.Api.Models;
using Squora.Forms.Hub.Api.Services;
using Microsoft.AspNetCore.Mvc;

namespace Squora.Forms.Hub.Api.Endpoints;

[ApiController]
[Route("api/v1/workspaces/{workspaceId}/forms")]
public partial class FormsController : ControllerBase
{
    private readonly FormDefinitionService _formService;

    public FormsController(FormDefinitionService formService)
    {
        _formService = formService;
    }

    /// <summary>Create a new form definition.</summary>
    [HttpPost]
    public async Task<IActionResult> Create(string workspaceId, [FromBody] CreateFormRequest request)
    {
        if (!IsValidSlug(request.Slug))
            return UnprocessableEntity(new { error = "invalid_slug", message = "Slug must be lowercase, alphanumeric, hyphens only." });

        try
        {
            var form = await _formService.CreateAsync(workspaceId, request.Slug, request.DisplayName, request.OwnerId, request.Body);
            return CreatedAtAction(nameof(GetBySlug), new { workspaceId, slug = form.Slug }, new
            {
                id = form.Id,
                slug = form.Slug,
                createdAt = form.CreatedAt,
                version = 1
            });
        }
        catch (MongoDB.Driver.MongoWriteException ex) when (ex.WriteError.Category == MongoDB.Driver.ServerErrorCategory.DuplicateKey)
        {
            return Conflict(new { error = "slug_conflict", message = $"Slug '{request.Slug}' already exists in this workspace." });
        }
    }

    /// <summary>List all forms in a workspace.</summary>
    [HttpGet]
    public async Task<IActionResult> List(string workspaceId)
    {
        var forms = await _formService.ListAsync(workspaceId);
        return Ok(forms.Select(f => new { f.Id, f.Slug, f.DisplayName, f.LatestVersionNumber, f.CreatedAt, f.UpdatedAt }));
    }

    /// <summary>Get a form by slug, optionally with state filter.</summary>
    /// <remarks>
    /// Supports patterns:
    /// - GET /forms/{slug} → latest published version
    /// - GET /forms/{slug}:latest@{state} → latest version in that state
    /// - GET /forms/{slug}@v{number} → specific version number
    /// </remarks>
    [HttpGet("{slugSpec}")]
    public async Task<IActionResult> GetBySlug(string workspaceId, string slugSpec)
    {
        var (slug, versionNumber, state) = ParseSlugSpec(slugSpec);

        var form = await _formService.GetBySlugAsync(workspaceId, slug);
        if (form == null)
            return NotFound(new { error = "form_not_found", slug });

        FormVersion? version;
        if (versionNumber.HasValue)
        {
            version = await _formService.GetVersionAsync(form.Id, versionNumber.Value);
        }
        else
        {
            var targetState = state ?? VersionState.Published;
            version = await _formService.GetLatestVersionByStateAsync(form.Id, targetState);
        }

        if (version == null)
            return NotFound(new { error = "version_not_found", slug, state = state?.ToString() ?? "published" });

        var etag = $"\"{version.ContentHash}\"";
        if (Request.Headers.IfNoneMatch.ToString() == etag)
            return StatusCode(304);

        Response.Headers.ETag = etag;
        Response.Headers.CacheControl = version.State == VersionState.Published
            ? "public, max-age=86400, immutable"
            : "private, no-cache";

        return Ok(MongoDB.Bson.BsonTypeMapper.MapToDotNetValue(version.Body));
    }

    /// <summary>Save a new version of a form definition.</summary>
    [HttpPut("{slug}")]
    public async Task<IActionResult> Update(string workspaceId, string slug, [FromBody] JsonElement body, [FromHeader(Name = "If-Match")] string? ifMatch)
    {
        if (string.IsNullOrEmpty(ifMatch))
            return StatusCode(428, new { error = "precondition_required", message = "If-Match header with ETag is required." });

        var form = await _formService.GetBySlugAsync(workspaceId, slug);
        if (form == null) return NotFound(new { error = "form_not_found", slug });

        // Check optimistic concurrency
        var latestVersion = await _formService.GetVersionAsync(form.Id, form.LatestVersionNumber);
        if (latestVersion != null)
        {
            var currentEtag = $"\"{latestVersion.ContentHash}\"";
            if (ifMatch != currentEtag)
                return StatusCode(412, new { error = "precondition_failed", currentETag = currentEtag, message = "Version conflict. Refresh and retry." });
        }

        var bodyJson = body.GetRawText();
        var version = await _formService.SaveVersionAsync(form.Id, workspaceId, bodyJson, "system"); // TODO: actor from auth

        return Ok(new
        {
            versionNumber = version.VersionNumber,
            etag = version.ContentHash,
            state = version.State.ToString()
        });
    }

    /// <summary>Change a version's state (draft → published, published → archived, etc.).</summary>
    [HttpPatch("{slug}/versions/{versionId}/state")]
    public async Task<IActionResult> ChangeState(string workspaceId, string slug, string versionId, [FromBody] ChangeStateRequest request)
    {
        if (!Enum.TryParse<VersionState>(request.State, true, out var newState))
            return UnprocessableEntity(new { error = "invalid_state", message = $"Unknown state: {request.State}. Valid: draft, published, archived, deprecated." });

        try
        {
            var version = await _formService.ChangeStateAsync(versionId, newState, request.ActorId ?? "system");
            return Ok(new { versionId = version.Id, state = version.State.ToString() });
        }
        catch (InvalidOperationException ex)
        {
            return UnprocessableEntity(new { error = "invalid_state_transition", message = ex.Message });
        }
    }

    /// <summary>Get version history for a form.</summary>
    [HttpGet("{slug}/versions")]
    public async Task<IActionResult> GetVersions(string workspaceId, string slug)
    {
        var form = await _formService.GetBySlugAsync(workspaceId, slug);
        if (form == null) return NotFound(new { error = "form_not_found", slug });

        var versions = await _formService.GetVersionHistoryAsync(form.Id);
        return Ok(versions.Select(v => new
        {
            v.Id, v.VersionNumber, v.SemanticTag, state = v.State.ToString(),
            v.ContentHash, v.CreatedBy, v.CreatedAt
        }));
    }

    /// <summary>Get JSON Schema for a form (AI-friendly endpoint).</summary>
    [HttpGet("{slug}/schema.json")]
    public async Task<IActionResult> GetSchema(string workspaceId, string slug)
    {
        var form = await _formService.GetBySlugAsync(workspaceId, slug);
        if (form == null) return NotFound(new { error = "form_not_found", slug });

        var version = await _formService.GetLatestVersionByStateAsync(form.Id, VersionState.Published);
        if (version == null) return NotFound(new { error = "no_published_version", slug });

        // Return the form body as a JSON Schema document
        return Ok(MongoDB.Bson.BsonTypeMapper.MapToDotNetValue(version.Body));
    }

    // --- Request/Response DTOs ---

    private static (string slug, int? versionNumber, VersionState? state) ParseSlugSpec(string spec)
    {
        // Pattern: slug:latest@state or slug@vN or just slug
        var stateMatch = Regex.Match(spec, @"^(.+?):latest@(.+)$");
        if (stateMatch.Success)
        {
            var slug = stateMatch.Groups[1].Value;
            if (Enum.TryParse<VersionState>(stateMatch.Groups[2].Value, true, out var state))
                return (slug, null, state);
        }

        var versionMatch = Regex.Match(spec, @"^(.+?)@v?(\d+)$");
        if (versionMatch.Success)
        {
            return (versionMatch.Groups[1].Value, int.Parse(versionMatch.Groups[2].Value), null);
        }

        return (spec, null, null); // Default: slug only → latest published
    }

    private static bool IsValidSlug(string slug) =>
        !string.IsNullOrEmpty(slug) && Regex.IsMatch(slug, @"^[a-z0-9][a-z0-9-]*$");
}

public class CreateFormRequest
{
    public string Slug { get; set; } = "";
    public string DisplayName { get; set; } = "";
    public string OwnerId { get; set; } = "";
    public string Body { get; set; } = "{}";
}

public class ChangeStateRequest
{
    public string State { get; set; } = "";
    public string? ActorId { get; set; }
}
