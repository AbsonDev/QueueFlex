namespace QueueManagement.Api.DTOs.Common;

/// <summary>
/// Pagination request DTO
/// </summary>
public class PaginationRequestDto
{
    /// <summary>
    /// Page number (1-based)
    /// </summary>
    public int Page { get; set; } = 1;

    /// <summary>
    /// Number of items per page
    /// </summary>
    public int PageSize { get; set; } = 20;

    /// <summary>
    /// Sort field
    /// </summary>
    public string? SortBy { get; set; }

    /// <summary>
    /// Sort direction (asc/desc)
    /// </summary>
    public string SortDirection { get; set; } = "asc";

    /// <summary>
    /// Search term
    /// </summary>
    public string? SearchTerm { get; set; }

    /// <summary>
    /// Filters to apply
    /// </summary>
    public Dictionary<string, string>? Filters { get; set; }
}

/// <summary>
/// Paginated response DTO
/// </summary>
/// <typeparam name="T">Type of the data items</typeparam>
public class PaginatedResponseDto<T>
{
    /// <summary>
    /// The data items for the current page
    /// </summary>
    public List<T> Items { get; set; } = new();

    /// <summary>
    /// Pagination metadata
    /// </summary>
    public PaginationMeta Meta { get; set; } = new();
}

/// <summary>
/// Base filter DTO
/// </summary>
public class BaseFilterDto
{
    /// <summary>
    /// Search term
    /// </summary>
    public string? SearchTerm { get; set; }

    /// <summary>
    /// Whether to include deleted items
    /// </summary>
    public bool IncludeDeleted { get; set; } = false;

    /// <summary>
    /// Date range filter - start date
    /// </summary>
    public DateTime? StartDate { get; set; }

    /// <summary>
    /// Date range filter - end date
    /// </summary>
    public DateTime? EndDate { get; set; }
}

/// <summary>
/// Unit filter DTO
/// </summary>
public class UnitFilterDto : BaseFilterDto
{
    /// <summary>
    /// Filter by status
    /// </summary>
    public string? Status { get; set; }

    /// <summary>
    /// Filter by city
    /// </summary>
    public string? City { get; set; }

    /// <summary>
    /// Filter by state
    /// </summary>
    public string? State { get; set; }
}

/// <summary>
/// Queue filter DTO
/// </summary>
public class QueueFilterDto : BaseFilterDto
{
    /// <summary>
    /// Filter by unit ID
    /// </summary>
    public Guid? UnitId { get; set; }

    /// <summary>
    /// Filter by status
    /// </summary>
    public string? Status { get; set; }

    /// <summary>
    /// Filter by active status
    /// </summary>
    public bool? IsActive { get; set; }
}

/// <summary>
/// Ticket filter DTO
/// </summary>
public class TicketFilterDto : BaseFilterDto
{
    /// <summary>
    /// Filter by queue ID
    /// </summary>
    public Guid? QueueId { get; set; }

    /// <summary>
    /// Filter by service ID
    /// </summary>
    public Guid? ServiceId { get; set; }

    /// <summary>
    /// Filter by status
    /// </summary>
    public string? Status { get; set; }

    /// <summary>
    /// Filter by priority
    /// </summary>
    public string? Priority { get; set; }

    /// <summary>
    /// Filter by unit ID
    /// </summary>
    public Guid? UnitId { get; set; }
}

/// <summary>
/// Session filter DTO
/// </summary>
public class SessionFilterDto : BaseFilterDto
{
    /// <summary>
    /// Filter by user ID
    /// </summary>
    public Guid? UserId { get; set; }

    /// <summary>
    /// Filter by ticket ID
    /// </summary>
    public Guid? TicketId { get; set; }

    /// <summary>
    /// Filter by status
    /// </summary>
    public string? Status { get; set; }

    /// <summary>
    /// Filter by resource ID
    /// </summary>
    public Guid? ResourceId { get; set; }
}

/// <summary>
/// Service filter DTO
/// </summary>
public class ServiceFilterDto : BaseFilterDto
{
    /// <summary>
    /// Filter by active status
    /// </summary>
    public bool? IsActive { get; set; }

    /// <summary>
    /// Filter by resource requirement
    /// </summary>
    public bool? RequiresResource { get; set; }
}

/// <summary>
/// User filter DTO
/// </summary>
public class UserFilterDto : BaseFilterDto
{
    /// <summary>
    /// Filter by role
    /// </summary>
    public string? Role { get; set; }

    /// <summary>
    /// Filter by status
    /// </summary>
    public string? Status { get; set; }

    /// <summary>
    /// Filter by unit ID
    /// </summary>
    public Guid? UnitId { get; set; }
}