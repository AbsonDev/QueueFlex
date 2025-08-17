using System.ComponentModel.DataAnnotations;

namespace QueueManagement.Api.DTOs.Tickets;

/// <summary>
/// Ticket response DTO
/// </summary>
public class TicketDto
{
    /// <summary>
    /// Ticket ID
    /// </summary>
    public Guid Id { get; set; }

    /// <summary>
    /// Ticket number
    /// </summary>
    public string Number { get; set; } = string.Empty;

    /// <summary>
    /// Current status
    /// </summary>
    public string Status { get; set; } = string.Empty;

    /// <summary>
    /// Priority level
    /// </summary>
    public string Priority { get; set; } = string.Empty;

    /// <summary>
    /// Date and time when the ticket was issued
    /// </summary>
    public DateTime IssuedAt { get; set; }

    /// <summary>
    /// Date and time when the ticket was called
    /// </summary>
    public DateTime? CalledAt { get; set; }

    /// <summary>
    /// Date and time when the service started
    /// </summary>
    public DateTime? StartedAt { get; set; }

    /// <summary>
    /// Date and time when the service was completed
    /// </summary>
    public DateTime? CompletedAt { get; set; }

    /// <summary>
    /// Customer name
    /// </summary>
    public string? CustomerName { get; set; }

    /// <summary>
    /// Customer document
    /// </summary>
    public string? CustomerDocument { get; set; }

    /// <summary>
    /// Customer phone number
    /// </summary>
    public string? CustomerPhone { get; set; }

    /// <summary>
    /// Additional notes
    /// </summary>
    public string? Notes { get; set; }

    /// <summary>
    /// Completion notes
    /// </summary>
    public string? CompletionNotes { get; set; }

    /// <summary>
    /// Queue ID
    /// </summary>
    public Guid QueueId { get; set; }

    /// <summary>
    /// Queue name
    /// </summary>
    public string QueueName { get; set; } = string.Empty;

    /// <summary>
    /// Service ID
    /// </summary>
    public Guid ServiceId { get; set; }

    /// <summary>
    /// Service name
    /// </summary>
    public string ServiceName { get; set; } = string.Empty;

    /// <summary>
    /// Unit ID
    /// </summary>
    public Guid UnitId { get; set; }

    /// <summary>
    /// Unit name
    /// </summary>
    public string UnitName { get; set; } = string.Empty;

    /// <summary>
    /// Total waiting time
    /// </summary>
    public TimeSpan? WaitingTime { get; set; }

    /// <summary>
    /// Total service time
    /// </summary>
    public TimeSpan? ServiceTime { get; set; }

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
/// Ticket status DTO for public display
/// </summary>
public class TicketStatusDto
{
    /// <summary>
    /// Ticket number
    /// </summary>
    public string Number { get; set; } = string.Empty;

    /// <summary>
    /// Current status
    /// </summary>
    public string Status { get; set; } = string.Empty;

    /// <summary>
    /// Priority level
    /// </summary>
    public string Priority { get; set; } = string.Empty;

    /// <summary>
    /// Queue name
    /// </summary>
    public string QueueName { get; set; } = string.Empty;

    /// <summary>
    /// Service name
    /// </summary>
    public string ServiceName { get; set; } = string.Empty;

    /// <summary>
    /// Date and time when the ticket was issued
    /// </summary>
    public DateTime IssuedAt { get; set; }

    /// <summary>
    /// Date and time when the ticket was called
    /// </summary>
    public DateTime? CalledAt { get; set; }

    /// <summary>
    /// Estimated waiting time in minutes
    /// </summary>
    public int? EstimatedWaitingMinutes { get; set; }
}

/// <summary>
/// Create ticket request DTO
/// </summary>
public class CreateTicketDto
{
    /// <summary>
    /// Queue ID
    /// </summary>
    [Required]
    public Guid QueueId { get; set; }

    /// <summary>
    /// Service ID
    /// </summary>
    [Required]
    public Guid ServiceId { get; set; }

    /// <summary>
    /// Customer name
    /// </summary>
    [MaxLength(200)]
    public string? CustomerName { get; set; }

    /// <summary>
    /// Customer document
    /// </summary>
    [MaxLength(20)]
    public string? CustomerDocument { get; set; }

    /// <summary>
    /// Customer phone number
    /// </summary>
    [MaxLength(20)]
    public string? CustomerPhone { get; set; }

    /// <summary>
    /// Priority level
    /// </summary>
    public string Priority { get; set; } = "Normal";

    /// <summary>
    /// Additional notes
    /// </summary>
    [MaxLength(1000)]
    public string? Notes { get; set; }
}

/// <summary>
/// Update ticket request DTO
/// </summary>
public class UpdateTicketDto
{
    /// <summary>
    /// Customer name
    /// </summary>
    [MaxLength(200)]
    public string? CustomerName { get; set; }

    /// <summary>
    /// Customer document
    /// </summary>
    [MaxLength(20)]
    public string? CustomerDocument { get; set; }

    /// <summary>
    /// Customer phone number
    /// </summary>
    [MaxLength(20)]
    public string? CustomerPhone { get; set; }

    /// <summary>
    /// Priority level
    /// </summary>
    public string? Priority { get; set; }

    /// <summary>
    /// Additional notes
    /// </summary>
    [MaxLength(1000)]
    public string? Notes { get; set; }
}

/// <summary>
/// Call ticket request DTO
/// </summary>
public class CallTicketDto
{
    /// <summary>
    /// User ID calling the ticket
    /// </summary>
    [Required]
    public Guid UserId { get; set; }

    /// <summary>
    /// Resource ID if applicable
    /// </summary>
    public Guid? ResourceId { get; set; }

    /// <summary>
    /// Notes about the call
    /// </summary>
    [MaxLength(500)]
    public string? Notes { get; set; }
}

/// <summary>
/// Transfer ticket request DTO
/// </summary>
public class TransferTicketDto
{
    /// <summary>
    /// New queue ID
    /// </summary>
    [Required]
    public Guid NewQueueId { get; set; }

    /// <summary>
    /// New service ID
    /// </summary>
    [Required]
    public Guid NewServiceId { get; set; }

    /// <summary>
    /// Reason for transfer
    /// </summary>
    [Required]
    [MaxLength(500)]
    public string Reason { get; set; } = string.Empty;

    /// <summary>
    /// User ID performing the transfer
    /// </summary>
    [Required]
    public Guid UserId { get; set; }
}