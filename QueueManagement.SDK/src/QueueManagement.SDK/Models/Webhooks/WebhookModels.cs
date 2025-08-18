using QueueManagement.SDK.Models.Common;

namespace QueueManagement.SDK.Models.Webhooks;

/// <summary>
/// Represents a webhook configuration.
/// </summary>
public class WebhookResponse
{
    /// <summary>
    /// Gets or sets the unique identifier of the webhook.
    /// </summary>
    public Guid Id { get; set; }

    /// <summary>
    /// Gets or sets the webhook name.
    /// </summary>
    public string Name { get; set; } = string.Empty;

    /// <summary>
    /// Gets or sets the webhook URL.
    /// </summary>
    public string Url { get; set; } = string.Empty;

    /// <summary>
    /// Gets or sets the webhook secret for signature validation.
    /// </summary>
    public string? Secret { get; set; }

    /// <summary>
    /// Gets or sets the list of events to subscribe to.
    /// </summary>
    public List<WebhookEventType> Events { get; set; } = new();

    /// <summary>
    /// Gets or sets whether the webhook is active.
    /// </summary>
    public bool IsActive { get; set; }

    /// <summary>
    /// Gets or sets custom headers to include in webhook requests.
    /// </summary>
    public Dictionary<string, string>? Headers { get; set; }

    /// <summary>
    /// Gets or sets the retry configuration.
    /// </summary>
    public WebhookRetryConfig? RetryConfig { get; set; }

    /// <summary>
    /// Gets or sets filter criteria for the webhook.
    /// </summary>
    public WebhookFilter? Filter { get; set; }

    /// <summary>
    /// Gets or sets when the webhook was created.
    /// </summary>
    public DateTime CreatedAt { get; set; }

    /// <summary>
    /// Gets or sets when the webhook was last updated.
    /// </summary>
    public DateTime UpdatedAt { get; set; }

    /// <summary>
    /// Gets or sets when the webhook was last triggered.
    /// </summary>
    public DateTime? LastTriggeredAt { get; set; }

    /// <summary>
    /// Gets or sets the last trigger status.
    /// </summary>
    public string? LastTriggerStatus { get; set; }

    /// <summary>
    /// Gets or sets the total number of successful deliveries.
    /// </summary>
    public int SuccessfulDeliveries { get; set; }

    /// <summary>
    /// Gets or sets the total number of failed deliveries.
    /// </summary>
    public int FailedDeliveries { get; set; }
}

/// <summary>
/// Request to create a new webhook.
/// </summary>
public class CreateWebhookRequest
{
    /// <summary>
    /// Gets or sets the webhook name.
    /// </summary>
    public string Name { get; set; } = string.Empty;

    /// <summary>
    /// Gets or sets the webhook URL.
    /// </summary>
    public string Url { get; set; } = string.Empty;

    /// <summary>
    /// Gets or sets the list of events to subscribe to.
    /// </summary>
    public List<WebhookEventType> Events { get; set; } = new();

    /// <summary>
    /// Gets or sets whether to generate a secret for signature validation (default: true).
    /// </summary>
    public bool GenerateSecret { get; set; } = true;

    /// <summary>
    /// Gets or sets custom headers to include in webhook requests (optional).
    /// </summary>
    public Dictionary<string, string>? Headers { get; set; }

    /// <summary>
    /// Gets or sets the retry configuration (optional).
    /// </summary>
    public WebhookRetryConfig? RetryConfig { get; set; }

    /// <summary>
    /// Gets or sets filter criteria for the webhook (optional).
    /// </summary>
    public WebhookFilter? Filter { get; set; }

    /// <summary>
    /// Gets or sets whether the webhook should be active immediately (default: true).
    /// </summary>
    public bool IsActive { get; set; } = true;
}

/// <summary>
/// Request to update an existing webhook.
/// </summary>
public class UpdateWebhookRequest
{
    /// <summary>
    /// Gets or sets the updated name (optional).
    /// </summary>
    public string? Name { get; set; }

    /// <summary>
    /// Gets or sets the updated URL (optional).
    /// </summary>
    public string? Url { get; set; }

    /// <summary>
    /// Gets or sets the updated events (optional).
    /// </summary>
    public List<WebhookEventType>? Events { get; set; }

    /// <summary>
    /// Gets or sets whether the webhook is active (optional).
    /// </summary>
    public bool? IsActive { get; set; }

    /// <summary>
    /// Gets or sets updated custom headers (optional).
    /// </summary>
    public Dictionary<string, string>? Headers { get; set; }

    /// <summary>
    /// Gets or sets the updated retry configuration (optional).
    /// </summary>
    public WebhookRetryConfig? RetryConfig { get; set; }

    /// <summary>
    /// Gets or sets updated filter criteria (optional).
    /// </summary>
    public WebhookFilter? Filter { get; set; }
}

/// <summary>
/// Webhook retry configuration.
/// </summary>
public class WebhookRetryConfig
{
    /// <summary>
    /// Gets or sets the maximum number of retry attempts.
    /// </summary>
    public int MaxRetries { get; set; } = 3;

    /// <summary>
    /// Gets or sets the initial retry delay in seconds.
    /// </summary>
    public int InitialDelaySeconds { get; set; } = 10;

    /// <summary>
    /// Gets or sets the maximum retry delay in seconds.
    /// </summary>
    public int MaxDelaySeconds { get; set; } = 3600;

    /// <summary>
    /// Gets or sets whether to use exponential backoff.
    /// </summary>
    public bool UseExponentialBackoff { get; set; } = true;
}

/// <summary>
/// Webhook filter criteria.
/// </summary>
public class WebhookFilter
{
    /// <summary>
    /// Gets or sets the list of unit IDs to filter by.
    /// </summary>
    public List<Guid>? UnitIds { get; set; }

    /// <summary>
    /// Gets or sets the list of queue IDs to filter by.
    /// </summary>
    public List<Guid>? QueueIds { get; set; }

    /// <summary>
    /// Gets or sets the list of service IDs to filter by.
    /// </summary>
    public List<Guid>? ServiceIds { get; set; }

    /// <summary>
    /// Gets or sets the minimum priority level to include.
    /// </summary>
    public Priority? MinPriority { get; set; }

    /// <summary>
    /// Gets or sets custom filter expressions.
    /// </summary>
    public Dictionary<string, object>? CustomFilters { get; set; }
}

/// <summary>
/// Represents a webhook event payload.
/// </summary>
/// <typeparam name="T">The type of event data.</typeparam>
public class WebhookEvent<T> where T : class
{
    /// <summary>
    /// Gets or sets the event ID.
    /// </summary>
    public Guid Id { get; set; }

    /// <summary>
    /// Gets or sets the webhook ID.
    /// </summary>
    public Guid WebhookId { get; set; }

    /// <summary>
    /// Gets or sets the event type.
    /// </summary>
    public string Event { get; set; } = string.Empty;

    /// <summary>
    /// Gets or sets the event data.
    /// </summary>
    public T Data { get; set; } = default!;

    /// <summary>
    /// Gets or sets when the event occurred.
    /// </summary>
    public DateTime Timestamp { get; set; }

    /// <summary>
    /// Gets or sets the tenant ID if applicable.
    /// </summary>
    public string? TenantId { get; set; }

    /// <summary>
    /// Gets or sets additional metadata.
    /// </summary>
    public Dictionary<string, object>? Metadata { get; set; }
}

/// <summary>
/// Response from testing a webhook.
/// </summary>
public class WebhookTestResponse
{
    /// <summary>
    /// Gets or sets whether the test was successful.
    /// </summary>
    public bool Success { get; set; }

    /// <summary>
    /// Gets or sets the HTTP status code received.
    /// </summary>
    public int StatusCode { get; set; }

    /// <summary>
    /// Gets or sets the response body.
    /// </summary>
    public string? ResponseBody { get; set; }

    /// <summary>
    /// Gets or sets the response time in milliseconds.
    /// </summary>
    public long ResponseTimeMs { get; set; }

    /// <summary>
    /// Gets or sets any error message.
    /// </summary>
    public string? Error { get; set; }

    /// <summary>
    /// Gets or sets the test payload that was sent.
    /// </summary>
    public object? TestPayload { get; set; }
}

/// <summary>
/// Webhook delivery attempt record.
/// </summary>
public class WebhookDeliveryAttempt
{
    /// <summary>
    /// Gets or sets the attempt ID.
    /// </summary>
    public Guid Id { get; set; }

    /// <summary>
    /// Gets or sets the webhook ID.
    /// </summary>
    public Guid WebhookId { get; set; }

    /// <summary>
    /// Gets or sets the event ID.
    /// </summary>
    public Guid EventId { get; set; }

    /// <summary>
    /// Gets or sets the attempt number.
    /// </summary>
    public int AttemptNumber { get; set; }

    /// <summary>
    /// Gets or sets whether the delivery was successful.
    /// </summary>
    public bool Success { get; set; }

    /// <summary>
    /// Gets or sets the HTTP status code.
    /// </summary>
    public int? StatusCode { get; set; }

    /// <summary>
    /// Gets or sets the response body.
    /// </summary>
    public string? ResponseBody { get; set; }

    /// <summary>
    /// Gets or sets any error message.
    /// </summary>
    public string? Error { get; set; }

    /// <summary>
    /// Gets or sets the response time in milliseconds.
    /// </summary>
    public long ResponseTimeMs { get; set; }

    /// <summary>
    /// Gets or sets when the attempt was made.
    /// </summary>
    public DateTime AttemptedAt { get; set; }

    /// <summary>
    /// Gets or sets when the next retry will be attempted.
    /// </summary>
    public DateTime? NextRetryAt { get; set; }
}