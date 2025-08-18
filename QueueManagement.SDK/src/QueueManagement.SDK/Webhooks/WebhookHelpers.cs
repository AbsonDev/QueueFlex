using System.Security.Cryptography;
using System.Text;
using System.Text.Json;
using System.Text.Json.Serialization;
using QueueManagement.SDK.Models.Webhooks;

namespace QueueManagement.SDK.Webhooks;

/// <summary>
/// Helper methods for working with webhooks.
/// </summary>
public static class WebhookHelpers
{
    private static readonly JsonSerializerOptions JsonOptions = new()
    {
        PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
        DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingNull,
        Converters = { new JsonStringEnumConverter(JsonNamingPolicy.CamelCase) }
    };

    /// <summary>
    /// Validates a webhook signature.
    /// </summary>
    /// <param name="payload">The webhook payload.</param>
    /// <param name="signature">The signature to validate.</param>
    /// <param name="secret">The webhook secret.</param>
    /// <returns>True if the signature is valid; otherwise, false.</returns>
    public static bool ValidateSignature(string payload, string signature, string secret)
    {
        if (string.IsNullOrWhiteSpace(payload) || string.IsNullOrWhiteSpace(signature) || string.IsNullOrWhiteSpace(secret))
        {
            return false;
        }

        try
        {
            using var hmac = new HMACSHA256(Encoding.UTF8.GetBytes(secret));
            var hash = hmac.ComputeHash(Encoding.UTF8.GetBytes(payload));
            var expectedSignature = Convert.ToBase64String(hash);
            
            // Use constant-time comparison to prevent timing attacks
            return CryptographicOperations.FixedTimeEquals(
                Encoding.UTF8.GetBytes(signature),
                Encoding.UTF8.GetBytes(expectedSignature));
        }
        catch
        {
            return false;
        }
    }

    /// <summary>
    /// Generates a webhook signature for a payload.
    /// </summary>
    /// <param name="payload">The webhook payload.</param>
    /// <param name="secret">The webhook secret.</param>
    /// <returns>The generated signature.</returns>
    public static string GenerateSignature(string payload, string secret)
    {
        if (string.IsNullOrWhiteSpace(payload) || string.IsNullOrWhiteSpace(secret))
        {
            throw new ArgumentException("Payload and secret must not be empty.");
        }

        using var hmac = new HMACSHA256(Encoding.UTF8.GetBytes(secret));
        var hash = hmac.ComputeHash(Encoding.UTF8.GetBytes(payload));
        return Convert.ToBase64String(hash);
    }

    /// <summary>
    /// Parses a webhook event from a JSON payload.
    /// </summary>
    /// <typeparam name="T">The type of event data.</typeparam>
    /// <param name="payload">The JSON payload.</param>
    /// <returns>The parsed webhook event.</returns>
    /// <exception cref="InvalidOperationException">Thrown when parsing fails.</exception>
    public static WebhookEvent<T> ParseEvent<T>(string payload) where T : class
    {
        if (string.IsNullOrWhiteSpace(payload))
        {
            throw new ArgumentException("Payload must not be empty.", nameof(payload));
        }

        try
        {
            var webhookEvent = JsonSerializer.Deserialize<WebhookEvent<T>>(payload, JsonOptions);
            if (webhookEvent == null)
            {
                throw new InvalidOperationException("Failed to parse webhook payload: deserialization returned null.");
            }

            return webhookEvent;
        }
        catch (JsonException ex)
        {
            throw new InvalidOperationException($"Failed to parse webhook payload: {ex.Message}", ex);
        }
    }

    /// <summary>
    /// Tries to parse a webhook event from a JSON payload.
    /// </summary>
    /// <typeparam name="T">The type of event data.</typeparam>
    /// <param name="payload">The JSON payload.</param>
    /// <param name="webhookEvent">The parsed webhook event, if successful.</param>
    /// <returns>True if parsing was successful; otherwise, false.</returns>
    public static bool TryParseEvent<T>(string payload, out WebhookEvent<T>? webhookEvent) where T : class
    {
        webhookEvent = null;

        if (string.IsNullOrWhiteSpace(payload))
        {
            return false;
        }

        try
        {
            webhookEvent = JsonSerializer.Deserialize<WebhookEvent<T>>(payload, JsonOptions);
            return webhookEvent != null;
        }
        catch
        {
            return false;
        }
    }

    /// <summary>
    /// Serializes a webhook event to JSON.
    /// </summary>
    /// <typeparam name="T">The type of event data.</typeparam>
    /// <param name="webhookEvent">The webhook event to serialize.</param>
    /// <returns>The JSON representation of the webhook event.</returns>
    public static string SerializeEvent<T>(WebhookEvent<T> webhookEvent) where T : class
    {
        if (webhookEvent == null)
        {
            throw new ArgumentNullException(nameof(webhookEvent));
        }

        return JsonSerializer.Serialize(webhookEvent, JsonOptions);
    }

    /// <summary>
    /// Creates a webhook event.
    /// </summary>
    /// <typeparam name="T">The type of event data.</typeparam>
    /// <param name="eventType">The event type.</param>
    /// <param name="data">The event data.</param>
    /// <param name="webhookId">The webhook ID.</param>
    /// <param name="tenantId">The tenant ID (optional).</param>
    /// <returns>A new webhook event.</returns>
    public static WebhookEvent<T> CreateEvent<T>(string eventType, T data, Guid webhookId, string? tenantId = null) where T : class
    {
        return new WebhookEvent<T>
        {
            Id = Guid.NewGuid(),
            WebhookId = webhookId,
            Event = eventType,
            Data = data ?? throw new ArgumentNullException(nameof(data)),
            Timestamp = DateTime.UtcNow,
            TenantId = tenantId
        };
    }

    /// <summary>
    /// Validates that a webhook URL is valid and uses HTTPS.
    /// </summary>
    /// <param name="url">The URL to validate.</param>
    /// <returns>True if the URL is valid; otherwise, false.</returns>
    public static bool ValidateWebhookUrl(string url)
    {
        if (string.IsNullOrWhiteSpace(url))
        {
            return false;
        }

        if (!Uri.TryCreate(url, UriKind.Absolute, out var uri))
        {
            return false;
        }

        // Require HTTPS for security (can be disabled for local development)
        return uri.Scheme == Uri.UriSchemeHttps || 
               (uri.Scheme == Uri.UriSchemeHttp && (uri.Host == "localhost" || uri.Host == "127.0.0.1"));
    }

    /// <summary>
    /// Generates a secure random webhook secret.
    /// </summary>
    /// <param name="length">The length of the secret (default: 32 bytes).</param>
    /// <returns>A base64-encoded random secret.</returns>
    public static string GenerateSecret(int length = 32)
    {
        if (length <= 0)
        {
            throw new ArgumentException("Length must be greater than zero.", nameof(length));
        }

        var bytes = new byte[length];
        using var rng = RandomNumberGenerator.Create();
        rng.GetBytes(bytes);
        return Convert.ToBase64String(bytes);
    }

    /// <summary>
    /// Extracts the webhook signature from HTTP headers.
    /// </summary>
    /// <param name="headers">The HTTP headers.</param>
    /// <param name="headerName">The name of the signature header (default: "X-Webhook-Signature").</param>
    /// <returns>The signature value, or null if not found.</returns>
    public static string? ExtractSignatureFromHeaders(IDictionary<string, string> headers, string headerName = "X-Webhook-Signature")
    {
        if (headers == null)
        {
            return null;
        }

        // Try exact match first
        if (headers.TryGetValue(headerName, out var signature))
        {
            return signature;
        }

        // Try case-insensitive match
        var key = headers.Keys.FirstOrDefault(k => string.Equals(k, headerName, StringComparison.OrdinalIgnoreCase));
        return key != null ? headers[key] : null;
    }

    /// <summary>
    /// Calculates the retry delay for a failed webhook delivery.
    /// </summary>
    /// <param name="attemptNumber">The attempt number (1-based).</param>
    /// <param name="config">The retry configuration.</param>
    /// <returns>The delay before the next retry attempt.</returns>
    public static TimeSpan CalculateRetryDelay(int attemptNumber, WebhookRetryConfig config)
    {
        if (config == null)
        {
            throw new ArgumentNullException(nameof(config));
        }

        if (attemptNumber <= 0)
        {
            throw new ArgumentException("Attempt number must be greater than zero.", nameof(attemptNumber));
        }

        if (!config.UseExponentialBackoff)
        {
            return TimeSpan.FromSeconds(config.InitialDelaySeconds);
        }

        // Calculate exponential backoff with jitter
        var baseDelay = config.InitialDelaySeconds * Math.Pow(2, attemptNumber - 1);
        var maxDelay = Math.Min(baseDelay, config.MaxDelaySeconds);
        
        // Add jitter (up to 10% of the delay)
        var jitter = Random.Shared.NextDouble() * 0.1 * maxDelay;
        
        return TimeSpan.FromSeconds(maxDelay + jitter);
    }
}