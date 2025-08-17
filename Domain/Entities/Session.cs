using System.ComponentModel.DataAnnotations;
using QueueManagement.Domain.Enums;

namespace QueueManagement.Domain.Entities;

/// <summary>
/// Represents a service session for a ticket
/// </summary>
public class Session : BaseEntity
{
    /// <summary>
    /// Ticket this session is for
    /// </summary>
    [Required]
    public Guid TicketId { get; set; }

    /// <summary>
    /// User handling this session
    /// </summary>
    [Required]
    public Guid UserId { get; set; }

    /// <summary>
    /// Resource used for this session (optional)
    /// </summary>
    public Guid? ResourceId { get; set; }

    /// <summary>
    /// Current status of the session
    /// </summary>
    [Required]
    public SessionStatus Status { get; set; }

    /// <summary>
    /// Date and time when the session started
    /// </summary>
    [Required]
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
    [Range(1, 5)]
    public int? CustomerRating { get; set; }

    /// <summary>
    /// Customer feedback
    /// </summary>
    [MaxLength(1000)]
    public string? CustomerFeedback { get; set; }

    /// <summary>
    /// Internal notes about the session
    /// </summary>
    [MaxLength(1000)]
    public string? InternalNotes { get; set; }

    // Navigation properties
    /// <summary>
    /// Ticket this session is for
    /// </summary>
    public virtual Ticket Ticket { get; set; } = null!;

    /// <summary>
    /// User handling this session
    /// </summary>
    public virtual User User { get; set; } = null!;

    /// <summary>
    /// Resource used for this session
    /// </summary>
    public virtual Resource? Resource { get; set; }

    /// <summary>
    /// Constructor that sets default values
    /// </summary>
    public Session() : base()
    {
        Status = SessionStatus.InProgress;
        StartedAt = DateTime.UtcNow;
    }

    /// <summary>
    /// Creates a new session
    /// </summary>
    public Session(Guid ticketId, Guid userId, Guid tenantId, string createdBy, Guid? resourceId = null) : this()
    {
        TicketId = ticketId;
        UserId = userId;
        TenantId = tenantId;
        ResourceId = resourceId;
        SetCreated(createdBy);
    }

    /// <summary>
    /// Pauses the session
    /// </summary>
    public void Pause(string updatedBy)
    {
        if (Status != SessionStatus.InProgress)
            throw new InvalidOperationException("Only in-progress sessions can be paused");

        Status = SessionStatus.Paused;
        PausedAt = DateTime.UtcNow;
        SetUpdated(updatedBy);
    }

    /// <summary>
    /// Resumes the session
    /// </summary>
    public void Resume(string updatedBy)
    {
        if (Status != SessionStatus.Paused)
            throw new InvalidOperationException("Only paused sessions can be resumed");

        Status = SessionStatus.InProgress;
        
        if (PausedAt.HasValue)
        {
            var pauseDuration = DateTime.UtcNow - PausedAt.Value;
            PausedDuration = (PausedDuration ?? TimeSpan.Zero) + pauseDuration;
        }
        
        PausedAt = null;
        SetUpdated(updatedBy);
    }

    /// <summary>
    /// Completes the session
    /// </summary>
    public void Complete(string updatedBy)
    {
        if (Status != SessionStatus.InProgress && Status != SessionStatus.Paused)
            throw new InvalidOperationException("Only in-progress or paused sessions can be completed");

        Status = SessionStatus.Completed;
        CompletedAt = DateTime.UtcNow;
        SetUpdated(updatedBy);
    }

    /// <summary>
    /// Cancels the session
    /// </summary>
    public void Cancel(string updatedBy)
    {
        if (Status == SessionStatus.Completed)
            throw new InvalidOperationException("Completed sessions cannot be cancelled");

        Status = SessionStatus.Cancelled;
        SetUpdated(updatedBy);
    }

    /// <summary>
    /// Sets the customer rating
    /// </summary>
    public void SetCustomerRating(int rating, string? feedback, string updatedBy)
    {
        if (rating < 1 || rating > 5)
            throw new ArgumentOutOfRangeException(nameof(rating), "Rating must be between 1 and 5");

        CustomerRating = rating;
        CustomerFeedback = feedback;
        SetUpdated(updatedBy);
    }

    /// <summary>
    /// Adds internal notes
    /// </summary>
    public void AddInternalNotes(string notes, string updatedBy)
    {
        InternalNotes = notes ?? throw new ArgumentNullException(nameof(notes));
        SetUpdated(updatedBy);
    }

    /// <summary>
    /// Gets the total session duration
    /// </summary>
    public TimeSpan GetTotalDuration()
    {
        var endTime = CompletedAt ?? DateTime.UtcNow;
        var totalDuration = endTime - StartedAt;
        
        if (PausedDuration.HasValue)
            totalDuration -= PausedDuration.Value;
            
        return totalDuration;
    }

    /// <summary>
    /// Gets the active session duration (excluding pauses)
    /// </summary>
    public TimeSpan GetActiveDuration()
    {
        var endTime = CompletedAt ?? DateTime.UtcNow;
        var activeDuration = endTime - StartedAt;
        
        if (PausedDuration.HasValue)
            activeDuration -= PausedDuration.Value;
            
        return activeDuration;
    }
}