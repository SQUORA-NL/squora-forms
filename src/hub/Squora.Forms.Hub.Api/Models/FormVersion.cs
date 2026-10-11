using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace Squora.Forms.Hub.Api.Models;

/// <summary>
/// An immutable snapshot of a FormDefinition's JSON body.
/// Each save creates a new version; versions are never mutated.
/// </summary>
public class FormVersion
{
    [BsonId]
    [BsonRepresentation(BsonType.ObjectId)]
    public string Id { get; set; } = ObjectId.GenerateNewId().ToString();

    public string FormDefinitionId { get; set; } = "";
    public string WorkspaceId { get; set; } = "";
    public int VersionNumber { get; set; }
    public string? SemanticTag { get; set; }

    [BsonRepresentation(BsonType.String)]
    public VersionState State { get; set; } = VersionState.Draft;

    /// <summary>The form definition JSON body.</summary>
    public BsonDocument Body { get; set; } = new();

    public string ContentHash { get; set; } = "";
    public string CreatedBy { get; set; } = "";
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
}
