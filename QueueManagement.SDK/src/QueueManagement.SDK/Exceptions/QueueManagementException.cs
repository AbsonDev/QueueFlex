using System.Net;

namespace QueueManagement.SDK.Exceptions;

/// <summary>
/// Base exception for QueueManagement SDK errors.
/// </summary>
public class QueueManagementException : Exception
{
    /// <summary>
    /// Gets the error code returned by the API.
    /// </summary>
    public string? ErrorCode { get; }

    /// <summary>
    /// Gets the HTTP status code of the failed request.
    /// </summary>
    public HttpStatusCode? StatusCode { get; }

    /// <summary>
    /// Gets the request ID for tracing purposes.
    /// </summary>
    public string? RequestId { get; }

    /// <summary>
    /// Gets the validation errors if this is a validation exception.
    /// </summary>
    public IReadOnlyList<ValidationError>? ValidationErrors { get; }

    /// <summary>
    /// Initializes a new instance of the <see cref="QueueManagementException"/> class.
    /// </summary>
    public QueueManagementException(string message) 
        : base(message)
    {
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="QueueManagementException"/> class.
    /// </summary>
    public QueueManagementException(string message, Exception innerException) 
        : base(message, innerException)
    {
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="QueueManagementException"/> class.
    /// </summary>
    public QueueManagementException(
        string message, 
        string? errorCode = null, 
        HttpStatusCode? statusCode = null,
        string? requestId = null) 
        : base(message)
    {
        ErrorCode = errorCode;
        StatusCode = statusCode;
        RequestId = requestId;
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="QueueManagementException"/> class with validation errors.
    /// </summary>
    public QueueManagementException(
        string message, 
        IEnumerable<ValidationError> validationErrors,
        string? requestId = null) 
        : base(message)
    {
        ValidationErrors = validationErrors?.ToList().AsReadOnly();
        StatusCode = HttpStatusCode.BadRequest;
        ErrorCode = "VALIDATION_ERROR";
        RequestId = requestId;
    }

    /// <summary>
    /// Gets whether this exception represents a client error (4xx status code).
    /// </summary>
    public bool IsClientError => StatusCode.HasValue && (int)StatusCode.Value >= 400 && (int)StatusCode.Value < 500;

    /// <summary>
    /// Gets whether this exception represents a server error (5xx status code).
    /// </summary>
    public bool IsServerError => StatusCode.HasValue && (int)StatusCode.Value >= 500;

    /// <summary>
    /// Gets whether this exception represents a transient error that can be retried.
    /// </summary>
    public bool IsTransient => IsServerError || 
        StatusCode == HttpStatusCode.RequestTimeout ||
        StatusCode == HttpStatusCode.TooManyRequests;
}

/// <summary>
/// Represents a validation error.
/// </summary>
public class ValidationError
{
    /// <summary>
    /// Gets or sets the field that failed validation.
    /// </summary>
    public string Field { get; set; } = string.Empty;

    /// <summary>
    /// Gets or sets the validation error message.
    /// </summary>
    public string Message { get; set; } = string.Empty;

    /// <summary>
    /// Gets or sets the validation error code.
    /// </summary>
    public string? Code { get; set; }

    /// <summary>
    /// Gets or sets the attempted value that failed validation.
    /// </summary>
    public object? AttemptedValue { get; set; }
}