using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace Squora.Forms.Hub.Api.Models.Collaboration;

public class ReviewRequest
{
    [BsonId]
    [BsonRepresentation(BsonType.ObjectId)]
    public string Id { get; set; } = ObjectId.GenerateNewId().ToString();

    public string WorkspaceId { get; set; } = "";
    public string DefinitionId { get; set; } = "";
    public int DefinitionVersion { get; set; }
    public string RequestedByUserId { get; set; } = "";
    public List<string> AssignedReviewerIds { get; set; } = new();
    public string Status { get; set; } = "pending"; // pending | approved | changes-requested | published
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    public DateTime? ResolvedAt { get; set; }
    public List<ReviewEvent> AuditLog { get; set; } = new();
}

public class ReviewEvent
{
    public string Id { get; set; } = Guid.NewGuid().ToString();
    public string ActorUserId { get; set; } = "";
    public string Type { get; set; } = ""; // submitted | approved | changes-requested | reassigned | invalidated | published
    public string? Note { get; set; }
    public DateTime OccurredAt { get; set; } = DateTime.UtcNow;
}
