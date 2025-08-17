using System.ComponentModel.DataAnnotations;

namespace QueueManagement.Api.DTOs.Queues;

/// <summary>
/// Queue response DTO
/// </summary>
public class QueueDto
{
    /// <summary>
    /// Queue ID
    /// </summary>
    public Guid Id { get; set; }

    /// <summary>
    /// Queue name
    /// </summary>
    public string Name { get; set; } = string.Empty;

    /// <summary>
    /// Queue code
    /// </summary>
    public string Code { get; set; } = string.Empty;

    /// <summary>
    /// Display name shown to customers
    /// </summary>
    public string DisplayName { get; set; } = string.Empty;

    /// <summary>
    /// Maximum capacity
    /// </summary>
    public int MaxCapacity { get; set; }

    /// <summary>
    /// Current status
    /// </summary>
    public string Status { get; set; } = string.Empty;

    /// <summary>
    /// Whether the queue is active
    /// </summary>
    public bool IsActive { get; set; }

    /// <summary>
    /// Unit ID that owns this queue
    /// </summary>
    public Guid UnitId { get; set; }

    /// <summary>
    /// Unit name
    /// </summary>
    public string UnitName { get; set; } = string.Empty;

    /// <summary>
    /// Current number of tickets waiting
    /// </summary>
    public int CurrentTicketCount { get; set; }

    /// <summary>
    /// Whether the queue is at capacity
    /// </summary>
    public bool IsAtCapacity { get; set; }

    /// <summary>
    /// Whether the queue is accepting tickets
    /// </summary>
    public bool IsAcceptingTickets { get; set; }

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
/// Queue status DTO for real-time updates
/// </summary>
public class QueueStatusDto
{
    /// <summary>
    /// Queue ID
    /// </summary>
    public Guid Id { get; set; }

    /// <summary>
    /// Queue name
    /// </summary>
    public string Name { get; set; } = string.Empty;

    /// <summary>
    /// Current status
    /// </summary>
    public string Status { get; set; } = string.Empty;

    /// <summary>
    /// Whether the queue is active
    /// </summary>
    public bool IsActive { get; set; }

    /// <summary>
    /// Current number of tickets waiting
    /// </summary>
    public int CurrentTicketCount { get; set; }

    /// <summary>
    /// Maximum capacity
    /// </summary>
    public int MaxCapacity { get; set; }

    /// <summary>
    /// Whether the queue is at capacity
    /// </summary>
    public bool IsAtCapacity { get; set; }

    /// <summary>
    /// Whether the queue is accepting tickets
    /// </summary>
    public bool IsAcceptingTickets { get; set; }

    /// <summary>
    /// Last update timestamp
    /// </summary>
    public DateTime LastUpdated { get; set; }
}

/// <summary>
/// Create queue request DTO
/// </summary>
public class CreateQueueDto
{
    /// <summary>
    /// Queue name
    /// </summary>
    [Required]
    [MaxLength(200)]
    public string Name { get; set; } = string.Empty;

    /// <summary>
    /// Queue code
    /// </summary>
    [Required]
    [MaxLength(50)]
    [RegularExpression(@"^[A-Z0-9]+$", ErrorMessage = "Code can only contain uppercase letters and numbers")]
    public string Code { get; set; } = string.Empty;

    /// <summary>
    /// Display name shown to customers
    /// </summary>
    [Required]
    [MaxLength(200)]
    public string DisplayName { get; set; } = string.Empty;

    /// <summary>
    /// Maximum capacity
    /// </summary>
    [Required]
    [Range(1, 10000)]
    public int MaxCapacity { get; set; }

    /// <summary>
    /// Unit ID that will own this queue
    /// </summary>
    [Required]
    public Guid UnitId { get; set; }
}

/// <summary>
/// Update queue request DTO
/// </summary>
public class UpdateQueueDto
{
    /// <summary>
    /// Queue name
    /// </summary>
    [MaxLength(200)]
    public string? Name { get; set; }

    /// <summary>
    /// Queue code
    /// </summary>
    [MaxLength(50)]
    [RegularExpression(@"^[A-Z0-9]+$", ErrorMessage = "Code can only contain uppercase letters and numbers")]
    public string? Code { get; set; }

    /// <summary>
    /// Display name shown to customers
    /// </summary>
    [MaxLength(200)]
    public string? DisplayName { get; set; }

    /// <summary>
    /// Maximum capacity
    /// </summary>
    [Range(1, 10000)]
    public int? MaxCapacity { get; set; }

    /// <summary>
    /// Whether the queue is active
    /// </summary>
    public bool? IsActive { get; set; }
}

/// <summary>
/// Update queue status request DTO
/// </summary>
public class UpdateQueueStatusDto
{
    /// <summary>
    /// New status
    /// </summary>
    [Required]
    public string Status { get; set; } = string.Empty;

    /// <summary>
    /// Whether the queue is active
    /// </summary>
    public bool? IsActive { get; set; }
}