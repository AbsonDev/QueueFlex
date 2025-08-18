using Microsoft.Extensions.Logging;

namespace QueueManagement.SDK.Configuration;

/// <summary>
/// Configuration options for the QueueManagement SDK client.
/// </summary>
public class QueueManagementOptions
{
    /// <summary>
    /// Gets or sets the API key for authentication.
    /// </summary>
    public string ApiKey { get; set; } = string.Empty;

    /// <summary>
    /// Gets or sets the base URL of the QueueManagement API.
    /// Default: https://api.queuemanagement.io
    /// </summary>
    public string BaseUrl { get; set; } = "https://api.queuemanagement.io";

    /// <summary>
    /// Gets or sets the SignalR hub URL for real-time updates.
    /// If not set, will be derived from BaseUrl.
    /// </summary>
    public string? SignalRUrl { get; set; }

    /// <summary>
    /// Gets or sets the tenant ID for multi-tenant scenarios.
    /// </summary>
    public string? TenantId { get; set; }

    /// <summary>
    /// Gets or sets the HTTP timeout for API calls.
    /// Default: 30 seconds
    /// </summary>
    public TimeSpan Timeout { get; set; } = TimeSpan.FromSeconds(30);

    /// <summary>
    /// Gets or sets the maximum number of retry attempts for failed requests.
    /// Default: 3
    /// </summary>
    public int MaxRetries { get; set; } = 3;

    /// <summary>
    /// Gets or sets the base delay between retry attempts.
    /// Default: 1 second
    /// </summary>
    public TimeSpan RetryDelay { get; set; } = TimeSpan.FromSeconds(1);

    /// <summary>
    /// Gets or sets the maximum delay between retry attempts.
    /// Default: 30 seconds
    /// </summary>
    public TimeSpan MaxRetryDelay { get; set; } = TimeSpan.FromSeconds(30);

    /// <summary>
    /// Gets or sets whether to enable logging.
    /// Default: false
    /// </summary>
    public bool EnableLogging { get; set; } = false;

    /// <summary>
    /// Gets or sets the minimum log level.
    /// Default: Information
    /// </summary>
    public LogLevel LogLevel { get; set; } = LogLevel.Information;

    /// <summary>
    /// Gets or sets a custom User-Agent string for API requests.
    /// </summary>
    public string? UserAgent { get; set; }

    /// <summary>
    /// Gets or sets whether to automatically connect to SignalR on client initialization.
    /// Default: false
    /// </summary>
    public bool AutoConnectSignalR { get; set; } = false;

    /// <summary>
    /// Gets or sets whether to validate SSL certificates.
    /// Default: true (recommended for production)
    /// </summary>
    public bool ValidateSslCertificate { get; set; } = true;

    /// <summary>
    /// Validates the configuration options.
    /// </summary>
    /// <exception cref="ArgumentException">Thrown when configuration is invalid.</exception>
    public void Validate()
    {
        if (string.IsNullOrWhiteSpace(ApiKey))
        {
            throw new ArgumentException("API key is required.", nameof(ApiKey));
        }

        if (string.IsNullOrWhiteSpace(BaseUrl))
        {
            throw new ArgumentException("Base URL is required.", nameof(BaseUrl));
        }

        if (!Uri.TryCreate(BaseUrl, UriKind.Absolute, out var uri) || 
            (uri.Scheme != Uri.UriSchemeHttp && uri.Scheme != Uri.UriSchemeHttps))
        {
            throw new ArgumentException("Base URL must be a valid HTTP or HTTPS URL.", nameof(BaseUrl));
        }

        if (Timeout <= TimeSpan.Zero)
        {
            throw new ArgumentException("Timeout must be greater than zero.", nameof(Timeout));
        }

        if (MaxRetries < 0)
        {
            throw new ArgumentException("MaxRetries cannot be negative.", nameof(MaxRetries));
        }

        if (RetryDelay <= TimeSpan.Zero)
        {
            throw new ArgumentException("RetryDelay must be greater than zero.", nameof(RetryDelay));
        }

        if (MaxRetryDelay < RetryDelay)
        {
            throw new ArgumentException("MaxRetryDelay must be greater than or equal to RetryDelay.", nameof(MaxRetryDelay));
        }
    }

    /// <summary>
    /// Gets the effective SignalR URL.
    /// </summary>
    public string GetSignalRUrl()
    {
        if (!string.IsNullOrWhiteSpace(SignalRUrl))
        {
            return SignalRUrl;
        }

        var baseUri = new Uri(BaseUrl);
        var signalRPath = "/hubs/queue";
        return new Uri(baseUri, signalRPath).ToString();
    }
}