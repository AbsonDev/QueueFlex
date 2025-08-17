namespace QueueManagement.Api.DTOs.Common;

/// <summary>
/// Base response wrapper for all API responses
/// </summary>
/// <typeparam name="T">Type of the data payload</typeparam>
public class ApiResponse<T>
{
    /// <summary>
    /// Indicates if the request was successful
    /// </summary>
    public bool Success { get; set; } = true;

    /// <summary>
    /// The actual data payload
    /// </summary>
    public T? Data { get; set; }

    /// <summary>
    /// Error information if the request failed
    /// </summary>
    public ApiError? Error { get; set; }

    /// <summary>
    /// Metadata about the response (pagination, timestamp, etc.)
    /// </summary>
    public ApiMeta? Meta { get; set; }
}

/// <summary>
/// Error information for failed API requests
/// </summary>
public class ApiError
{
    /// <summary>
    /// Error code for programmatic handling
    /// </summary>
    public string Code { get; set; } = string.Empty;

    /// <summary>
    /// Human-readable error message
    /// </summary>
    public string Message { get; set; } = string.Empty;

    /// <summary>
    /// Detailed validation errors if applicable
    /// </summary>
    public List<ValidationError>? Details { get; set; }
}

/// <summary>
/// Validation error details
/// </summary>
public class ValidationError
{
    /// <summary>
    /// Field name that failed validation
    /// </summary>
    public string Field { get; set; } = string.Empty;

    /// <summary>
    /// Validation error message
    /// </summary>
    public string Message { get; set; } = string.Empty;
}

/// <summary>
/// Metadata about the API response
/// </summary>
public class ApiMeta
{
    /// <summary>
    /// Pagination information if applicable
    /// </summary>
    public PaginationMeta? Pagination { get; set; }

    /// <summary>
    /// Timestamp when the response was generated
    /// </summary>
    public DateTime Timestamp { get; set; } = DateTime.UtcNow;
}

/// <summary>
/// Pagination metadata for list responses
/// </summary>
public class PaginationMeta
{
    /// <summary>
    /// Current page number (1-based)
    /// </summary>
    public int Page { get; set; }

    /// <summary>
    /// Number of items per page
    /// </summary>
    public int PageSize { get; set; }

    /// <summary>
    /// Total number of items available
    /// </summary>
    public int TotalItems { get; set; }

    /// <summary>
    /// Total number of pages
    /// </summary>
    public int TotalPages { get; set; }

    /// <summary>
    /// Whether there is a next page
    /// </summary>
    public bool HasNext { get; set; }

    /// <summary>
    /// Whether there is a previous page
    /// </summary>
    public bool HasPrevious { get; set; }
}