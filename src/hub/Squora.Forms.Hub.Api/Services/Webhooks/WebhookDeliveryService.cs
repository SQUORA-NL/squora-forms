using System.Security.Cryptography;
using System.Text;
using System.Text.Json;
using Squora.Forms.Hub.Api.Models.Webhooks;
using MongoDB.Driver;

namespace Squora.Forms.Hub.Api.Services.Webhooks;

public class WebhookDeliveryService
{
    private readonly IMongoCollection<WebhookEndpoint> _endpoints;
    private readonly IMongoCollection<WebhookDelivery> _deliveries;
    private readonly HttpClient _httpClient;

    public WebhookDeliveryService(IMongoDatabase database, HttpClient httpClient)
    {
        _endpoints = database.GetCollection<WebhookEndpoint>("webhook_endpoints");
        _deliveries = database.GetCollection<WebhookDelivery>("webhook_deliveries");
        _httpClient = httpClient;
    }

    public async Task<WebhookEndpoint> RegisterEndpointAsync(string workspaceId, string url, List<string> events)
    {
        var secret = GenerateSecret();
        var endpoint = new WebhookEndpoint
        {
            WorkspaceId = workspaceId,
            Url = url,
            SubscribedEvents = events,
            SigningSecretHash = HashSecret(secret)
        };
        await _endpoints.InsertOneAsync(endpoint);
        // Return with plaintext secret (shown once)
        endpoint.SigningSecretHash = secret; // Temporarily hold plaintext for response
        return endpoint;
    }

    public async Task FireEventAsync(string workspaceId, string eventType, object payload)
    {
        var endpoints = await _endpoints.Find(
            e => e.WorkspaceId == workspaceId &&
                 e.Status == "Active" &&
                 e.SubscribedEvents.Contains(eventType))
            .ToListAsync();

        foreach (var endpoint in endpoints)
        {
            await DeliverAsync(endpoint, eventType, payload);
        }
    }

    private async Task DeliverAsync(WebhookEndpoint endpoint, string eventType, object payload)
    {
        var body = JsonSerializer.Serialize(payload);
        var delivery = new WebhookDelivery
        {
            EndpointId = endpoint.Id,
            EventType = eventType,
            WorkspaceId = endpoint.WorkspaceId
        };

        try
        {
            var request = new HttpRequestMessage(HttpMethod.Post, endpoint.Url)
            {
                Content = new StringContent(body, Encoding.UTF8, "application/json")
            };

            // HMAC signature
            var signature = ComputeHmac(body, endpoint.SigningSecretHash);
            request.Headers.Add("X-SquoraForms-Signature", $"sha256={signature}");
            request.Headers.Add("X-SquoraForms-Event", eventType);

            var response = await _httpClient.SendAsync(request);
            delivery.HttpStatusCode = (int)response.StatusCode;
            delivery.Status = response.IsSuccessStatusCode ? "delivered" : "failed";
            delivery.RespondedAt = DateTime.UtcNow;
        }
        catch (Exception ex)
        {
            delivery.Status = "failed";
            delivery.ErrorMessage = ex.Message;
        }

        await _deliveries.InsertOneAsync(delivery);
    }

    private static string GenerateSecret()
    {
        return Convert.ToBase64String(RandomNumberGenerator.GetBytes(32));
    }

    private static string HashSecret(string secret)
    {
        return Convert.ToHexStringLower(SHA256.HashData(Encoding.UTF8.GetBytes(secret)));
    }

    private static string ComputeHmac(string body, string secretHash)
    {
        var key = Encoding.UTF8.GetBytes(secretHash);
        var bodyBytes = Encoding.UTF8.GetBytes(body);
        var hmac = HMACSHA256.HashData(key, bodyBytes);
        return Convert.ToHexStringLower(hmac);
    }
}
