using QueueManagement.SDK.Models.Common;
using QueueManagement.SDK.Models.Webhooks;

namespace QueueManagement.SDK.Services;

/// <summary>
/// Client for managing webhooks.
/// </summary>
public interface IWebhooksClient
{
    /// <summary>
    /// Creates a new webhook.
    /// </summary>
    Task<WebhookResponse> CreateAsync(CreateWebhookRequest request, CancellationToken cancellationToken = default);

    /// <summary>
    /// Gets a webhook by ID.
    /// </summary>
    Task<WebhookResponse> GetAsync(Guid webhookId, CancellationToken cancellationToken = default);

    /// <summary>
    /// Updates an existing webhook.
    /// </summary>
    Task<WebhookResponse> UpdateAsync(Guid webhookId, UpdateWebhookRequest request, CancellationToken cancellationToken = default);

    /// <summary>
    /// Deletes a webhook.
    /// </summary>
    Task DeleteAsync(Guid webhookId, CancellationToken cancellationToken = default);

    /// <summary>
    /// Gets all webhooks.
    /// </summary>
    Task<List<WebhookResponse>> GetAllAsync(CancellationToken cancellationToken = default);

    /// <summary>
    /// Gets paginated webhooks.
    /// </summary>
    Task<PagedResult<WebhookResponse>> GetPagedAsync(PagedRequest request, CancellationToken cancellationToken = default);

    /// <summary>
    /// Tests a webhook.
    /// </summary>
    Task<WebhookTestResponse> TestAsync(Guid webhookId, CancellationToken cancellationToken = default);

    /// <summary>
    /// Regenerates the secret for a webhook.
    /// </summary>
    Task<WebhookResponse> RegenerateSecretAsync(Guid webhookId, CancellationToken cancellationToken = default);

    /// <summary>
    /// Activates a webhook.
    /// </summary>
    Task<WebhookResponse> ActivateAsync(Guid webhookId, CancellationToken cancellationToken = default);

    /// <summary>
    /// Deactivates a webhook.
    /// </summary>
    Task<WebhookResponse> DeactivateAsync(Guid webhookId, CancellationToken cancellationToken = default);

    /// <summary>
    /// Gets delivery attempts for a webhook.
    /// </summary>
    Task<List<WebhookDeliveryAttempt>> GetDeliveryAttemptsAsync(Guid webhookId, int? limit = null, CancellationToken cancellationToken = default);

    /// <summary>
    /// Retries a failed webhook delivery.
    /// </summary>
    Task<WebhookDeliveryAttempt> RetryDeliveryAsync(Guid webhookId, Guid attemptId, CancellationToken cancellationToken = default);

    /// <summary>
    /// Gets webhooks by event type.
    /// </summary>
    Task<List<WebhookResponse>> GetByEventTypeAsync(WebhookEventType eventType, CancellationToken cancellationToken = default);

    /// <summary>
    /// Validates a webhook signature.
    /// </summary>
    bool ValidateSignature(string payload, string signature, string secret);

    /// <summary>
    /// Parses a webhook event payload.
    /// </summary>
    WebhookEvent<T> ParseEvent<T>(string payload) where T : class;
}