namespace QueueManagement.SDK.Models.Common;

/// <summary>
/// Represents API information.
/// </summary>
public class ApiInfo
{
    /// <summary>
    /// Gets or sets the API version.
    /// </summary>
    public string Version { get; set; } = string.Empty;

    /// <summary>
    /// Gets or sets the API environment.
    /// </summary>
    public string Environment { get; set; } = string.Empty;

    /// <summary>
    /// Gets or sets the server time.
    /// </summary>
    public DateTime ServerTime { get; set; }

    /// <summary>
    /// Gets or sets the server timezone.
    /// </summary>
    public string TimeZone { get; set; } = string.Empty;

    /// <summary>
    /// Gets or sets the API status.
    /// </summary>
    public string Status { get; set; } = string.Empty;

    /// <summary>
    /// Gets or sets available features.
    /// </summary>
    public List<string> Features { get; set; } = new();

    /// <summary>
    /// Gets or sets rate limit information.
    /// </summary>
    public RateLimitInfo? RateLimit { get; set; }
}

/// <summary>
/// Represents rate limit information.
/// </summary>
public class RateLimitInfo
{
    /// <summary>
    /// Gets or sets the rate limit.
    /// </summary>
    public int Limit { get; set; }

    /// <summary>
    /// Gets or sets the remaining requests.
    /// </summary>
    public int Remaining { get; set; }

    /// <summary>
    /// Gets or sets when the rate limit resets.
    /// </summary>
    public DateTimeOffset Reset { get; set; }
}

/// <summary>
/// Represents a health check response.
/// </summary>
public class HealthCheckResponse
{
    /// <summary>
    /// Gets or sets whether the service is healthy.
    /// </summary>
    public bool IsHealthy { get; set; }

    /// <summary>
    /// Gets or sets the status message.
    /// </summary>
    public string Status { get; set; } = string.Empty;

    /// <summary>
    /// Gets or sets individual service health checks.
    /// </summary>
    public Dictionary<string, ServiceHealth>? Services { get; set; }

    /// <summary>
    /// Gets or sets the response time in milliseconds.
    /// </summary>
    public long ResponseTimeMs { get; set; }

    /// <summary>
    /// Gets or sets the timestamp.
    /// </summary>
    public DateTime Timestamp { get; set; }
}

/// <summary>
/// Represents the health of an individual service.
/// </summary>
public class ServiceHealth
{
    /// <summary>
    /// Gets or sets whether the service is healthy.
    /// </summary>
    public bool IsHealthy { get; set; }

    /// <summary>
    /// Gets or sets the status message.
    /// </summary>
    public string Status { get; set; } = string.Empty;

    /// <summary>
    /// Gets or sets the response time in milliseconds.
    /// </summary>
    public long? ResponseTimeMs { get; set; }

    /// <summary>
    /// Gets or sets any error message.
    /// </summary>
    public string? Error { get; set; }
}

/// <summary>
/// Standard API response wrapper.
/// </summary>
/// <typeparam name="T">The type of data in the response.</typeparam>
public class ApiResponse<T> where T : class
{
    /// <summary>
    /// Gets or sets whether the request was successful.
    /// </summary>
    public bool Success { get; set; }

    /// <summary>
    /// Gets or sets the response data.
    /// </summary>
    public T? Data { get; set; }

    /// <summary>
    /// Gets or sets any error message.
    /// </summary>
    public string? Error { get; set; }

    /// <summary>
    /// Gets or sets the error code.
    /// </summary>
    public string? ErrorCode { get; set; }

    /// <summary>
    /// Gets or sets validation errors.
    /// </summary>
    public List<ValidationError>? ValidationErrors { get; set; }

    /// <summary>
    /// Gets or sets the request ID for tracing.
    /// </summary>
    public string? RequestId { get; set; }

    /// <summary>
    /// Gets or sets the timestamp.
    /// </summary>
    public DateTime Timestamp { get; set; }
}

/// <summary>
/// Represents a batch operation request.
/// </summary>
/// <typeparam name="T">The type of items in the batch.</typeparam>
public class BatchRequest<T> where T : class
{
    /// <summary>
    /// Gets or sets the items to process.
    /// </summary>
    public List<T> Items { get; set; } = new();

    /// <summary>
    /// Gets or sets whether to stop on first error.
    /// </summary>
    public bool StopOnError { get; set; } = false;

    /// <summary>
    /// Gets or sets whether to validate all items before processing.
    /// </summary>
    public bool ValidateBeforeProcess { get; set; } = true;
}

/// <summary>
/// Represents a batch operation response.
/// </summary>
/// <typeparam name="T">The type of results.</typeparam>
public class BatchResponse<T> where T : class
{
    /// <summary>
    /// Gets or sets the successful results.
    /// </summary>
    public List<BatchResult<T>> Successful { get; set; } = new();

    /// <summary>
    /// Gets or sets the failed results.
    /// </summary>
    public List<BatchResult<T>> Failed { get; set; } = new();

    /// <summary>
    /// Gets or sets the total number of items processed.
    /// </summary>
    public int TotalProcessed { get; set; }

    /// <summary>
    /// Gets or sets the number of successful items.
    /// </summary>
    public int SuccessCount => Successful.Count;

    /// <summary>
    /// Gets or sets the number of failed items.
    /// </summary>
    public int FailureCount => Failed.Count;

    /// <summary>
    /// Gets or sets the processing time in milliseconds.
    /// </summary>
    public long ProcessingTimeMs { get; set; }
}

/// <summary>
/// Represents a single result in a batch operation.
/// </summary>
/// <typeparam name="T">The type of the result.</typeparam>
public class BatchResult<T> where T : class
{
    /// <summary>
    /// Gets or sets the index of the item in the original batch.
    /// </summary>
    public int Index { get; set; }

    /// <summary>
    /// Gets or sets whether the operation was successful.
    /// </summary>
    public bool Success { get; set; }

    /// <summary>
    /// Gets or sets the result data.
    /// </summary>
    public T? Data { get; set; }

    /// <summary>
    /// Gets or sets any error message.
    /// </summary>
    public string? Error { get; set; }

    /// <summary>
    /// Gets or sets the error code.
    /// </summary>
    public string? ErrorCode { get; set; }
}