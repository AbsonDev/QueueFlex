using System.ComponentModel.DataAnnotations;
using QueueManagement.Domain.Enums;

namespace QueueManagement.Domain.Entities;

/// <summary>
/// Represents a customer ticket in a queue
/// </summary>
public class Ticket : BaseEntity
{
    /// <summary>
    /// Ticket number (e.g., A001, B047)
    /// </summary>
    [Required]
    [MaxLength(20)]
    public string Number { get; set; } = string.Empty;

    /// <summary>
    /// Queue this ticket belongs to
    /// </summary>
    [Required]
    public Guid QueueId { get; set; }

    /// <summary>
    /// Service requested by this ticket
    /// </summary>
    [Required]
    public Guid ServiceId { get; set; }

    /// <summary>
    /// Current status of the ticket
    /// </summary>
    [Required]
    public TicketStatus Status { get; set; }

    /// <summary>
    /// Priority level of the ticket
    /// </summary>
    [Required]
    public Priority Priority { get; set; }

    /// <summary>
    /// Date and time when the ticket was issued
    /// </summary>
    [Required]
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
    /// Customer name (optional)
    /// </summary>
    [MaxLength(200)]
    public string? CustomerName { get; set; }

    /// <summary>
    /// Customer document (CPF, CNPJ, etc.)
    /// </summary>
    [MaxLength(20)]
    public string? CustomerDocument { get; set; }

    /// <summary>
    /// Customer phone number
    /// </summary>
    [MaxLength(20)]
    public string? CustomerPhone { get; set; }

    /// <summary>
    /// Additional notes about the ticket
    /// </summary>
    [MaxLength(1000)]
    public string? Notes { get; set; }

    /// <summary>
    /// Notes about the completion of the service
    /// </summary>
    [MaxLength(1000)]
    public string? CompletionNotes { get; set; }

    // Navigation properties
    /// <summary>
    /// Queue this ticket belongs to
    /// </summary>
    public virtual Queue Queue { get; set; } = null!;

    /// <summary>
    /// Service requested by this ticket
    /// </summary>
    public virtual Service Service { get; set; } = null!;

    /// <summary>
    /// Sessions for this ticket
    /// </summary>
    public virtual ICollection<Session> Sessions { get; set; } = new List<Session>();

    /// <summary>
    /// Status change history for this ticket
    /// </summary>
    public virtual ICollection<TicketStatusHistory> StatusHistory { get; set; } = new List<TicketStatusHistory>();

    /// <summary>
    /// Constructor that sets default values
    /// </summary>
    public Ticket() : base()
    {
        Status = TicketStatus.Waiting;
        Priority = Priority.Normal;
        IssuedAt = DateTime.UtcNow;
    }

    /// <summary>
    /// Creates a new ticket
    /// </summary>
    public Ticket(string number, Guid queueId, Guid serviceId, Guid tenantId, string createdBy, 
        Priority priority = Priority.Normal) : this()
    {
        Number = number ?? throw new ArgumentNullException(nameof(number));
        QueueId = queueId;
        ServiceId = serviceId;
        TenantId = tenantId;
        Priority = priority;
        SetCreated(createdBy);
    }

    /// <summary>
    /// Calls the ticket for service
    /// </summary>
    public void Call(string updatedBy)
    {
        if (Status != TicketStatus.Waiting)
            throw new InvalidOperationException("Only waiting tickets can be called");

        Status = TicketStatus.Called;
        CalledAt = DateTime.UtcNow;
        SetUpdated(updatedBy);
    }

    /// <summary>
    /// Starts the service for this ticket
    /// </summary>
    public void Start(string updatedBy)
    {
        if (Status != TicketStatus.Called)
            throw new InvalidOperationException("Only called tickets can be started");

        Status = TicketStatus.InProgress;
        StartedAt = DateTime.UtcNow;
        SetUpdated(updatedBy);
    }

    /// <summary>
    /// Completes the service for this ticket
    /// </summary>
    public void Complete(string updatedBy, string? completionNotes = null)
    {
        if (Status != TicketStatus.InProgress)
            throw new InvalidOperationException("Only in-progress tickets can be completed");

        Status = TicketStatus.Completed;
        CompletedAt = DateTime.UtcNow;
        CompletionNotes = completionNotes;
        SetUpdated(updatedBy);
    }

    /// <summary>
    /// Cancels the ticket
    /// </summary>
    public void Cancel(string updatedBy, string? reason = null)
    {
        if (Status == TicketStatus.Completed)
            throw new InvalidOperationException("Completed tickets cannot be cancelled");

        Status = TicketStatus.Cancelled;
        SetUpdated(updatedBy);
    }

    /// <summary>
    /// Marks the ticket as no-show
    /// </summary>
    public void MarkAsNoShow(string updatedBy)
    {
        if (Status != TicketStatus.Called)
            throw new InvalidOperationException("Only called tickets can be marked as no-show");

        Status = TicketStatus.NoShow;
        SetUpdated(updatedBy);
    }

    /// <summary>
    /// Updates the ticket priority
    /// </summary>
    public void UpdatePriority(Priority newPriority, string updatedBy)
    {
        Priority = newPriority;
        SetUpdated(updatedBy);
    }

    /// <summary>
    /// Gets the total waiting time
    /// </summary>
    public TimeSpan? GetWaitingTime()
    {
        if (CalledAt.HasValue)
            return CalledAt.Value - IssuedAt;
        else if (StartedAt.HasValue)
            return StartedAt.Value - IssuedAt;
        else if (CompletedAt.HasValue)
            return CompletedAt.Value - IssuedAt;
        else
            return DateTime.UtcNow - IssuedAt;
    }

    /// <summary>
    /// Gets the total service time
    /// </summary>
    public TimeSpan? GetServiceTime()
    {
        if (!StartedAt.HasValue)
            return null;

        var endTime = CompletedAt ?? DateTime.UtcNow;
        return endTime - StartedAt.Value;
    }
}