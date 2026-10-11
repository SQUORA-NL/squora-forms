using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace Squora.Forms.Hub.Api.Models.Webhooks;

public class WebhookEndpoint
{
    [BsonId]
    [BsonRepresentation(BsonType.ObjectId)]
    public string Id { get; set; } = ObjectId.GenerateNewId().ToString();

    public string WorkspaceId { get; set; } = "";
    public string Url { get; set; } = "";
    public List<string> SubscribedEvents { get; set; } = new(); // form.published, form.schema_changed, form.validation_failed
    public string SigningSecretHash { get; set; } = ""; // stored hashed
    public string Status { get; set; } = "Active"; // Active | Suspended
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;
    public int ConsecutiveFailureHours { get; set; }
}
