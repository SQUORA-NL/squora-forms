using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace Squora.Forms.Hub.Api.Models.Collaboration;

/// <summary>
/// Immutable append-only event record for the activity feed.
/// Stored but not surfaced via UI in v1 (deferred).
/// </summary>
public class ActivityEvent
{
    [BsonId]
    [BsonRepresentation(BsonType.ObjectId)]
    public string Id { get; set; } = ObjectId.GenerateNewId().ToString();

    public string WorkspaceId { get; set; } = "";
    public string? DefinitionId { get; set; }
    public string ActorUserId { get; set; } = "";
    public string Type { get; set; } = ""; // definition.saved, comment.added, review.approved, etc.
    public BsonDocument? Payload { get; set; }
    public DateTime OccurredAt { get; set; } = DateTime.UtcNow;
}
