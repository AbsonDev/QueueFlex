using System.ComponentModel.DataAnnotations;
using QueueManagement.Domain.Enums;

namespace QueueManagement.Domain.Entities;

/// <summary>
/// Represents a queue for customer service
/// </summary>
public class Queue : BaseEntity
{
    /// <summary>
    /// Unit that owns this queue
    /// </summary>
    [Required]
    public Guid UnitId { get; set; }

    /// <summary>
    /// Queue name
    /// </summary>
    [Required]
    [MaxLength(200)]
    public string Name { get; set; } = string.Empty;

    /// <summary>
    /// Unique code for the queue within the unit
    /// </summary>
    [Required]
    [MaxLength(50)]
    public string Code { get; set; } = string.Empty;

    /// <summary>
    /// Display name shown to customers
    /// </summary>
    [Required]
    [MaxLength(200)]
    public string DisplayName { get; set; } = string.Empty;

    /// <summary>
    /// Maximum capacity of the queue
    /// </summary>
    [Required]
    [Range(1, 10000)]
    public int MaxCapacity { get; set; }

    /// <summary>
    /// Current status of the queue
    /// </summary>
    [Required]
    public QueueStatus Status { get; set; }

    /// <summary>
    /// Whether this queue is active
    /// </summary>
    [Required]
    public bool IsActive { get; set; }

    // Navigation properties
    /// <summary>
    /// Unit that owns this queue
    /// </summary>
    public virtual Unit Unit { get; set; } = null!;

    /// <summary>
    /// Services offered by this queue
    /// </summary>
    public virtual ICollection<QueueService> QueueServices { get; set; } = new List<QueueService>();

    /// <summary>
    /// Tickets in this queue
    /// </summary>
    public virtual ICollection<Ticket> Tickets { get; set; } = new List<Ticket>();

    /// <summary>
    /// Constructor that sets default values
    /// </summary>
    public Queue() : base()
    {
        IsActive = true;
        Status = QueueStatus.Open;
        MaxCapacity = 100;
    }

    /// <summary>
    /// Creates a new queue
    /// </summary>
    public Queue(string name, string code, string displayName, Guid unitId, Guid tenantId, string createdBy) : this()
    {
        Name = name ?? throw new ArgumentNullException(nameof(name));
        Code = code ?? throw new ArgumentNullException(nameof(code));
        DisplayName = displayName ?? throw new ArgumentNullException(nameof(displayName));
        UnitId = unitId;
        TenantId = tenantId;
        SetCreated(createdBy);
    }

    /// <summary>
    /// Updates the queue status
    /// </summary>
    public void UpdateStatus(QueueStatus newStatus, string updatedBy)
    {
        Status = newStatus;
        SetUpdated(updatedBy);
    }

    /// <summary>
    /// Updates the queue capacity
    /// </summary>
    public void UpdateCapacity(int newCapacity, string updatedBy)
    {
        if (newCapacity < 1 || newCapacity > 10000)
            throw new ArgumentOutOfRangeException(nameof(newCapacity), "Capacity must be between 1 and 10000");

        MaxCapacity = newCapacity;
        SetUpdated(updatedBy);
    }

    /// <summary>
    /// Activates this queue
    /// </summary>
    public void Activate(string updatedBy)
    {
        IsActive = true;
        SetUpdated(updatedBy);
    }

    /// <summary>
    /// Deactivates this queue
    /// </summary>
    public void Deactivate(string updatedBy)
    {
        IsActive = false;
        SetUpdated(updatedBy);
    }

    /// <summary>
    /// Checks if the queue is currently accepting tickets
    /// </summary>
    public bool IsAcceptingTickets => IsActive && Status == QueueStatus.Open;

    /// <summary>
    /// Gets the current number of tickets in the queue
    /// </summary>
    public int CurrentTicketCount => Tickets.Count(t => t.Status == TicketStatus.Waiting);

    /// <summary>
    /// Checks if the queue has reached its maximum capacity
    /// </summary>
    public bool IsAtCapacity => CurrentTicketCount >= MaxCapacity;
}