using System.Net;

namespace QueueManagement.SDK.Exceptions;

/// <summary>
/// Exception thrown when an API call fails.
/// </summary>
public class QueueManagementApiException : QueueManagementException
{
    /// <summary>
    /// Gets the raw response body from the API.
    /// </summary>
    public string? ResponseBody { get; }

    /// <summary>
    /// Gets the endpoint that was called.
    /// </summary>
    public string? Endpoint { get; }

    /// <summary>
    /// Gets the HTTP method used.
    /// </summary>
    public string? HttpMethod { get; }

    /// <summary>
    /// Initializes a new instance of the <see cref="QueueManagementApiException"/> class.
    /// </summary>
    public QueueManagementApiException(
        string message,
        HttpStatusCode statusCode,
        string? responseBody = null,
        string? errorCode = null,
        string? requestId = null,
        string? endpoint = null,
        string? httpMethod = null)
        : base(message, errorCode, statusCode, requestId)
    {
        ResponseBody = responseBody;
        Endpoint = endpoint;
        HttpMethod = httpMethod;
    }
}

/// <summary>
/// Exception thrown when authentication fails.
/// </summary>
public class QueueManagementAuthenticationException : QueueManagementException
{
    /// <summary>
    /// Initializes a new instance of the <see cref="QueueManagementAuthenticationException"/> class.
    /// </summary>
    public QueueManagementAuthenticationException(string message, string? requestId = null)
        : base(message, "AUTHENTICATION_FAILED", HttpStatusCode.Unauthorized, requestId)
    {
    }
}

/// <summary>
/// Exception thrown when authorization fails.
/// </summary>
public class QueueManagementAuthorizationException : QueueManagementException
{
    /// <summary>
    /// Initializes a new instance of the <see cref="QueueManagementAuthorizationException"/> class.
    /// </summary>
    public QueueManagementAuthorizationException(string message, string? requestId = null)
        : base(message, "AUTHORIZATION_FAILED", HttpStatusCode.Forbidden, requestId)
    {
    }
}

/// <summary>
/// Exception thrown when a resource is not found.
/// </summary>
public class QueueManagementNotFoundException : QueueManagementException
{
    /// <summary>
    /// Gets the type of resource that was not found.
    /// </summary>
    public string? ResourceType { get; }

    /// <summary>
    /// Gets the ID of the resource that was not found.
    /// </summary>
    public string? ResourceId { get; }

    /// <summary>
    /// Initializes a new instance of the <see cref="QueueManagementNotFoundException"/> class.
    /// </summary>
    public QueueManagementNotFoundException(
        string message,
        string? resourceType = null,
        string? resourceId = null,
        string? requestId = null)
        : base(message, "RESOURCE_NOT_FOUND", HttpStatusCode.NotFound, requestId)
    {
        ResourceType = resourceType;
        ResourceId = resourceId;
    }
}

/// <summary>
/// Exception thrown when rate limiting is encountered.
/// </summary>
public class QueueManagementRateLimitException : QueueManagementException
{
    /// <summary>
    /// Gets the time to wait before retrying.
    /// </summary>
    public TimeSpan? RetryAfter { get; }

    /// <summary>
    /// Gets the rate limit that was exceeded.
    /// </summary>
    public int? RateLimit { get; }

    /// <summary>
    /// Gets the remaining rate limit.
    /// </summary>
    public int? RateLimitRemaining { get; }

    /// <summary>
    /// Gets when the rate limit resets.
    /// </summary>
    public DateTimeOffset? RateLimitReset { get; }

    /// <summary>
    /// Initializes a new instance of the <see cref="QueueManagementRateLimitException"/> class.
    /// </summary>
    public QueueManagementRateLimitException(
        string message,
        TimeSpan? retryAfter = null,
        int? rateLimit = null,
        int? rateLimitRemaining = null,
        DateTimeOffset? rateLimitReset = null,
        string? requestId = null)
        : base(message, "RATE_LIMIT_EXCEEDED", HttpStatusCode.TooManyRequests, requestId)
    {
        RetryAfter = retryAfter;
        RateLimit = rateLimit;
        RateLimitRemaining = rateLimitRemaining;
        RateLimitReset = rateLimitReset;
    }
}

/// <summary>
/// Exception thrown when SignalR connection fails.
/// </summary>
public class QueueManagementSignalRException : QueueManagementException
{
    /// <summary>
    /// Initializes a new instance of the <see cref="QueueManagementSignalRException"/> class.
    /// </summary>
    public QueueManagementSignalRException(string message, Exception? innerException = null)
        : base(message, innerException ?? new Exception())
    {
        ErrorCode = "SIGNALR_ERROR";
    }
}