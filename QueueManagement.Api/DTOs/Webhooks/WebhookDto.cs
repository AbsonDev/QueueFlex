using System.ComponentModel.DataAnnotations;

namespace QueueManagement.Api.DTOs.Webhooks;

/// <summary>
/// Webhook response DTO
/// </summary>
public class WebhookDto
{
    /// <summary>
    /// Webhook ID
    /// </summary>
    public Guid Id { get; set; }

    /// <summary>
    /// Webhook name
    /// </summary>
    public string Name { get; set; } = string.Empty;

    /// <summary>
    /// Webhook URL
    /// </summary>
    public string Url { get; set; } = string.Empty;

    /// <summary>
    /// Events that trigger this webhook
    /// </summary>
    public List<string> Events { get; set; } = new();

    /// <summary>
    /// Whether this webhook is active
    /// </summary>
    public bool IsActive { get; set; }

    /// <summary>
    /// Number of retry attempts for failed webhook calls
    /// </summary>
    public int RetryCount { get; set; }

    /// <summary>
    /// Date and time when this webhook was last triggered
    /// </summary>
    public DateTime? LastTriggeredAt { get; set; }

    /// <summary>
    /// Creation date
    /// </summary>
    public DateTime CreatedAt { get; set; }

    /// <summary>
    /// Last update date
    /// </summary>
    public DateTime UpdatedAt { get; set; }
}

/// <summary>
/// Create webhook request DTO
/// </summary>
public class CreateWebhookDto
{
    /// <summary>
    /// Webhook name
    /// </summary>
    [Required]
    [MaxLength(200)]
    public string Name { get; set; } = string.Empty;

    /// <summary>
    /// Webhook URL
    /// </summary>
    [Required]
    [MaxLength(500)]
    [Url]
    public string Url { get; set; } = string.Empty;

    /// <summary>
    /// Events that trigger this webhook
    /// </summary>
    [Required]
    [MinLength(1)]
    public List<string> Events { get; set; } = new();

    /// <summary>
    /// Number of retry attempts for failed webhook calls
    /// </summary>
    [Range(0, 10)]
    public int RetryCount { get; set; } = 3;
}

/// <summary>
/// Update webhook request DTO
/// </summary>
public class UpdateWebhookDto
{
    /// <summary>
    /// Webhook name
    /// </summary>
    [MaxLength(200)]
    public string? Name { get; set; }

    /// <summary>
    /// Webhook URL
    /// </summary>
    [MaxLength(500)]
    [Url]
    public string? Url { get; set; }

    /// <summary>
    /// Events that trigger this webhook
    /// </summary>
    [MinLength(1)]
    public List<string>? Events { get; set; }

    /// <summary>
    /// Whether this webhook is active
    /// </summary>
    public bool? IsActive { get; set; }

    /// <summary>
    /// Number of retry attempts for failed webhook calls
    /// </summary>
    [Range(0, 10)]
    public int? RetryCount { get; set; }
}

/// <summary>
/// Test webhook request DTO
/// </summary>
public class TestWebhookDto
{
    /// <summary>
    /// Event type to test
    /// </summary>
    [Required]
    public string EventType { get; set; } = string.Empty;

    /// <summary>
    /// Test payload (optional)
    /// </summary>
    public object? TestPayload { get; set; }
}

/// <summary>
/// Webhook test response DTO
/// </summary>
public class WebhookTestResponseDto
{
    /// <summary>
    /// Whether the test was successful
    /// </summary>
    public bool Success { get; set; }

    /// <summary>
    /// HTTP status code received
    /// </summary>
    public int StatusCode { get; set; }

    /// <summary>
    /// Response message
    /// </summary>
    public string Message { get; set; } = string.Empty;

    /// <summary>
    /// Response time in milliseconds
    /// </summary>
    public long ResponseTimeMs { get; set; }

    /// <summary>
    /// Response headers received
    /// </summary>
    public Dictionary<string, string> ResponseHeaders { get; set; } = new();

    /// <summary>
    /// Response body received
    /// </summary>
    public string? ResponseBody { get; set; }

    /// <summary>
    /// Error details if the test failed
    /// </summary>
    public string? ErrorDetails { get; set; }
}