using System.Security.Cryptography;
using System.Text;
using System.Text.Json;
using Squora.Forms.Hub.Api.Models;
using MongoDB.Bson;
using MongoDB.Driver;

namespace Squora.Forms.Hub.Api.Services;

public class FormDefinitionService
{
    private readonly IMongoCollection<FormDefinition> _forms;
    private readonly IMongoCollection<FormVersion> _versions;
    private readonly IMongoCollection<StateChangeRecord> _stateChanges;

    public FormDefinitionService(IMongoDatabase database)
    {
        _forms = database.GetCollection<FormDefinition>("form_definitions");
        _versions = database.GetCollection<FormVersion>("form_versions");
        _stateChanges = database.GetCollection<StateChangeRecord>("state_changes");

        // Ensure indexes
        _forms.Indexes.CreateOne(new CreateIndexModel<FormDefinition>(
            Builders<FormDefinition>.IndexKeys.Ascending(f => f.WorkspaceId).Ascending(f => f.Slug),
            new CreateIndexOptions { Unique = true }));

        _versions.Indexes.CreateOne(new CreateIndexModel<FormVersion>(
            Builders<FormVersion>.IndexKeys.Ascending(v => v.FormDefinitionId).Ascending(v => v.VersionNumber)));
    }

    // --- CRUD ---

    public async Task<FormDefinition> CreateAsync(string workspaceId, string slug, string displayName, string ownerId, string bodyJson)
    {
        var form = new FormDefinition
        {
            WorkspaceId = workspaceId,
            Slug = slug.ToLowerInvariant(),
            DisplayName = displayName,
            OwnerId = ownerId,
            LatestVersionNumber = 1
        };
        await _forms.InsertOneAsync(form);

        var version = new FormVersion
        {
            FormDefinitionId = form.Id,
            WorkspaceId = workspaceId,
            VersionNumber = 1,
            State = VersionState.Draft,
            Body = BsonDocument.Parse(bodyJson),
            ContentHash = ComputeHash(bodyJson),
            CreatedBy = ownerId
        };
        await _versions.InsertOneAsync(version);

        return form;
    }

    public async Task<FormDefinition?> GetBySlugAsync(string workspaceId, string slug)
    {
        return await _forms.Find(f => f.WorkspaceId == workspaceId && f.Slug == slug && !f.IsDeleted)
            .FirstOrDefaultAsync();
    }

    public async Task<List<FormDefinition>> ListAsync(string workspaceId)
    {
        return await _forms.Find(f => f.WorkspaceId == workspaceId && !f.IsDeleted)
            .ToListAsync();
    }

    // --- Versioning ---

    public async Task<FormVersion> SaveVersionAsync(string formId, string workspaceId, string bodyJson, string createdBy)
    {
        var form = await _forms.Find(f => f.Id == formId).FirstOrDefaultAsync()
            ?? throw new InvalidOperationException($"Form {formId} not found.");

        var newVersionNumber = form.LatestVersionNumber + 1;
        var version = new FormVersion
        {
            FormDefinitionId = formId,
            WorkspaceId = workspaceId,
            VersionNumber = newVersionNumber,
            State = VersionState.Draft,
            Body = BsonDocument.Parse(bodyJson),
            ContentHash = ComputeHash(bodyJson),
            CreatedBy = createdBy
        };
        await _versions.InsertOneAsync(version);

        await _forms.UpdateOneAsync(
            f => f.Id == formId,
            Builders<FormDefinition>.Update
                .Set(f => f.LatestVersionNumber, newVersionNumber)
                .Set(f => f.UpdatedAt, DateTime.UtcNow));

        return version;
    }

    public async Task<FormVersion?> GetVersionAsync(string formId, int versionNumber)
    {
        return await _versions.Find(v => v.FormDefinitionId == formId && v.VersionNumber == versionNumber)
            .FirstOrDefaultAsync();
    }

    public async Task<FormVersion?> GetLatestVersionByStateAsync(string formId, VersionState state)
    {
        return await _versions.Find(v => v.FormDefinitionId == formId && v.State == state)
            .SortByDescending(v => v.VersionNumber)
            .FirstOrDefaultAsync();
    }

    public async Task<List<FormVersion>> GetVersionHistoryAsync(string formId)
    {
        return await _versions.Find(v => v.FormDefinitionId == formId)
            .SortByDescending(v => v.VersionNumber)
            .ToListAsync();
    }

    // --- State transitions ---

    public async Task<FormVersion> ChangeStateAsync(string versionId, VersionState newState, string actorId)
    {
        var version = await _versions.Find(v => v.Id == versionId).FirstOrDefaultAsync()
            ?? throw new InvalidOperationException($"Version {versionId} not found.");

        ValidateStateTransition(version.State, newState);

        var oldState = version.State;
        await _versions.UpdateOneAsync(
            v => v.Id == versionId,
            Builders<FormVersion>.Update.Set(v => v.State, newState));

        var record = new StateChangeRecord
        {
            FormVersionId = versionId,
            WorkspaceId = version.WorkspaceId,
            ActorId = actorId,
            FromState = oldState,
            ToState = newState
        };
        await _stateChanges.InsertOneAsync(record);

        version.State = newState;
        return version;
    }

    private static void ValidateStateTransition(VersionState from, VersionState to)
    {
        var valid = (from, to) switch
        {
            (VersionState.Draft, VersionState.Published) => true,
            (VersionState.Published, VersionState.Archived) => true,
            (VersionState.Published, VersionState.Deprecated) => true,
            (VersionState.Archived, VersionState.Draft) => true, // Re-open
            _ => false
        };

        if (!valid)
            throw new InvalidOperationException($"Invalid state transition: {from} → {to}.");
    }

    // --- Tags ---

    public async Task TagVersionAsync(string versionId, string semanticTag)
    {
        await _versions.UpdateOneAsync(
            v => v.Id == versionId,
            Builders<FormVersion>.Update.Set(v => v.SemanticTag, semanticTag));
    }

    // --- Helpers ---

    private static string ComputeHash(string content)
    {
        var bytes = SHA256.HashData(Encoding.UTF8.GetBytes(content));
        return Convert.ToHexStringLower(bytes);
    }
}
