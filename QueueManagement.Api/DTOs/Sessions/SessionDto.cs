using System.ComponentModel.DataAnnotations;

namespace QueueManagement.Api.DTOs.Sessions;

/// <summary>
/// Session response DTO
/// </summary>
public class SessionDto
{
    /// <summary>
    /// Session ID
    /// </summary>
    public Guid Id { get; set; }

    /// <summary>
    /// Current status
    /// </summary>
    public string Status { get; set; } = string.Empty;

    /// <summary>
    /// Date and time when the session started
    /// </summary>
    public DateTime StartedAt { get; set; }

    /// <summary>
    /// Date and time when the session was paused
    /// </summary>
    public DateTime? PausedAt { get; set; }

    /// <summary>
    /// Total duration the session was paused
    /// </summary>
    public TimeSpan? PausedDuration { get; set; }

    /// <summary>
    /// Date and time when the session was completed
    /// </summary>
    public DateTime? CompletedAt { get; set; }

    /// <summary>
    /// Customer rating (1-5)
    /// </summary>
    public int? CustomerRating { get; set; }

    /// <summary>
    /// Customer feedback
    /// </summary>
    public string? CustomerFeedback { get; set; }

    /// <summary>
    /// Internal notes about the session
    /// </summary>
    public string? InternalNotes { get; set; }

    /// <summary>
    /// Ticket ID
    /// </summary>
    public Guid TicketId { get; set; }

    /// <summary>
    /// Ticket number
    /// </summary>
    public string TicketNumber { get; set; } = string.Empty;

    /// <summary>
    /// User ID handling this session
    /// </summary>
    public Guid UserId { get; set; }

    /// <summary>
    /// User name
    /// </summary>
    public string UserName { get; set; } = string.Empty;

    /// <summary>
    /// Resource ID if applicable
    /// </summary>
    public Guid? ResourceId { get; set; }

    /// <summary>
    /// Resource name if applicable
    /// </summary>
    public string? ResourceName { get; set; }

    /// <summary>
    /// Total session duration
    /// </summary>
    public TimeSpan TotalDuration { get; set; }

    /// <summary>
    /// Active session duration (excluding pauses)
    /// </summary>
    public TimeSpan ActiveDuration { get; set; }

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
/// Create session request DTO
/// </summary>
public class CreateSessionDto
{
    /// <summary>
    /// Ticket ID
    /// </summary>
    [Required]
    public Guid TicketId { get; set; }

    /// <summary>
    /// User ID handling this session
    /// </summary>
    [Required]
    public Guid UserId { get; set; }

    /// <summary>
    /// Resource ID if applicable
    /// </summary>
    public Guid? ResourceId { get; set; }

    /// <summary>
    /// Initial notes about the session
    /// </summary>
    [MaxLength(1000)]
    public string? InitialNotes { get; set; }
}

/// <summary>
/// Complete session request DTO
/// </summary>
public class CompleteSessionDto
{
    /// <summary>
    /// Customer rating (1-5)
    /// </summary>
    [Range(1, 5)]
    public int? CustomerRating { get; set; }

    /// <summary>
    /// Customer feedback
    /// </summary>
    [MaxLength(1000)]
    public string? CustomerFeedback { get; set; }

    /// <summary>
    /// Completion notes
    /// </summary>
    [MaxLength(1000)]
    public string? CompletionNotes { get; set; }

    /// <summary>
    /// Internal notes
    /// </summary>
    [MaxLength(1000)]
    public string? InternalNotes { get; set; }
}

/// <summary>
/// Pause session request DTO
/// </summary>
public class PauseSessionDto
{
    /// <summary>
    /// Reason for pausing
    /// </summary>
    [Required]
    [MaxLength(500)]
    public string Reason { get; set; } = string.Empty;

    /// <summary>
    /// Additional notes
    /// </summary>
    [MaxLength(1000)]
    public string? Notes { get; set; }
}

/// <summary>
/// Resume session request DTO
/// </summary>
public class ResumeSessionDto
{
    /// <summary>
    /// Notes about resuming
    /// </summary>
    [MaxLength(1000)]
    public string? Notes { get; set; }
}