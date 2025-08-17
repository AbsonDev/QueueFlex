using System.ComponentModel.DataAnnotations;
using QueueManagement.Domain.Enums;

namespace QueueManagement.Domain.Entities.JunctionTables;

/// <summary>
/// Represents the history of status changes for a ticket
/// </summary>
public class TicketStatusHistory : BaseEntity
{
    /// <summary>
    /// Ticket this history entry belongs to
    /// </summary>
    [Required]
    public Guid TicketId { get; set; }

    /// <summary>
    /// Previous status of the ticket
    /// </summary>
    [Required]
    public TicketStatus FromStatus { get; set; }

    /// <summary>
    /// New status of the ticket
    /// </summary>
    [Required]
    public TicketStatus ToStatus { get; set; }

    /// <summary>
    /// Reason for the status change
    /// </summary>
    [MaxLength(500)]
    public string? Reason { get; set; }

    /// <summary>
    /// User who made the status change (optional)
    /// </summary>
    public Guid? UserId { get; set; }

    // Navigation properties
    /// <summary>
    /// Ticket this history entry belongs to
    /// </summary>
    public virtual Ticket Ticket { get; set; } = null!;

    /// <summary>
    /// User who made the status change
    /// </summary>
    public virtual User? User { get; set; }

    /// <summary>
    /// Constructor that sets default values
    /// </summary>
    public TicketStatusHistory() : base()
    {
    }

    /// <summary>
    /// Creates a new status history entry
    /// </summary>
    public TicketStatusHistory(Guid ticketId, TicketStatus fromStatus, TicketStatus toStatus, Guid tenantId, 
        string createdBy, string? reason = null, Guid? userId = null) : this()
    {
        TicketId = ticketId;
        FromStatus = fromStatus;
        ToStatus = toStatus;
        Reason = reason;
        UserId = userId;
        TenantId = tenantId;
        SetCreated(createdBy);
    }

    /// <summary>
    /// Gets a human-readable description of the status change
    /// </summary>
    public string GetStatusChangeDescription()
    {
        var description = $"Status changed from {FromStatus} to {ToStatus}";
        
        if (!string.IsNullOrEmpty(Reason))
            description += $" - Reason: {Reason}";
            
        if (UserId.HasValue)
            description += $" by user {UserId}";
            
        return description;
    }
}