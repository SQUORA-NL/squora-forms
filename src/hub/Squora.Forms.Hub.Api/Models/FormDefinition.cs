using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace Squora.Forms.Hub.Api.Models;

/// <summary>
/// The core artifact. A form definition with a workspace-scoped slug.
/// </summary>
public class FormDefinition
{
    [BsonId]
    [BsonRepresentation(BsonType.ObjectId)]
    public string Id { get; set; } = ObjectId.GenerateNewId().ToString();

    public string WorkspaceId { get; set; } = "";
    public string Slug { get; set; } = "";
    public string DisplayName { get; set; } = "";
    public string OwnerId { get; set; } = "";
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;
    public bool IsDeleted { get; set; }
    public int LatestVersionNumber { get; set; }
}
