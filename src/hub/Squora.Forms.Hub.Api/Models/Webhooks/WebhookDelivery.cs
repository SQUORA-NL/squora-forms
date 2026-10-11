using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace Squora.Forms.Hub.Api.Models.Webhooks;

public class WebhookDelivery
{
    [BsonId]
    [BsonRepresentation(BsonType.ObjectId)]
    public string Id { get; set; } = ObjectId.GenerateNewId().ToString();

    public string EndpointId { get; set; } = "";
    public string EventType { get; set; } = "";
    public string WorkspaceId { get; set; } = "";
    public int AttemptNumber { get; set; } = 1;
    public string Status { get; set; } = "pending"; // pending | delivered | failed | payload_too_large
    public int? HttpStatusCode { get; set; }
    public string? ErrorMessage { get; set; }
    public DateTime RequestedAt { get; set; } = DateTime.UtcNow;
    public DateTime? RespondedAt { get; set; }
    public string? CorrelationChain { get; set; }
}
