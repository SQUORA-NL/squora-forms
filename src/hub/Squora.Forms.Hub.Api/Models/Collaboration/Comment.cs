using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace Squora.Forms.Hub.Api.Models.Collaboration;

public class Comment
{
    [BsonId]
    [BsonRepresentation(BsonType.ObjectId)]
    public string Id { get; set; } = ObjectId.GenerateNewId().ToString();

    public string WorkspaceId { get; set; } = "";
    public string DefinitionId { get; set; } = "";
    public string AnchorPath { get; set; } = ""; // JSONPointer, e.g. /properties/email
    public string? ParentCommentId { get; set; } // null for top-level threads
    public string AuthorUserId { get; set; } = "";
    public string Body { get; set; } = ""; // max 4000 chars
    public string Status { get; set; } = "open"; // open | resolved | orphaned
    public List<string> Mentions { get; set; } = new(); // user IDs from @mentions
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;
}
