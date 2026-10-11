using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace Squora.Forms.Hub.Api.Models;

/// <summary>
/// Immutable audit entry recording a version state transition.
/// </summary>
public class StateChangeRecord
{
    [BsonId]
    [BsonRepresentation(BsonType.ObjectId)]
    public string Id { get; set; } = ObjectId.GenerateNewId().ToString();

    public string FormVersionId { get; set; } = "";
    public string WorkspaceId { get; set; } = "";
    public string ActorId { get; set; } = "";

    [BsonRepresentation(BsonType.String)]
    public VersionState FromState { get; set; }

    [BsonRepresentation(BsonType.String)]
    public VersionState ToState { get; set; }

    public DateTime Timestamp { get; set; } = DateTime.UtcNow;
}
