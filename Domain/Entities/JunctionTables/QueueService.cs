using System.ComponentModel.DataAnnotations;

namespace QueueManagement.Domain.Entities.JunctionTables;

/// <summary>
/// Junction table for many-to-many relationship between Queue and Service
/// </summary>
public class QueueService : BaseEntity
{
    /// <summary>
    /// Queue identifier
    /// </summary>
    [Required]
    public Guid QueueId { get; set; }

    /// <summary>
    /// Service identifier
    /// </summary>
    [Required]
    public Guid ServiceId { get; set; }

    /// <summary>
    /// Priority of this service in the queue (lower number = higher priority)
    /// </summary>
    [Required]
    [Range(1, 100)]
    public int Priority { get; set; }

    /// <summary>
    /// Whether this service is active in the queue
    /// </summary>
    [Required]
    public bool IsActive { get; set; }

    // Navigation properties
    /// <summary>
    /// Queue that offers this service
    /// </summary>
    public virtual Queue Queue { get; set; } = null!;

    /// <summary>
    /// Service offered by this queue
    /// </summary>
    public virtual Service Service { get; set; } = null!;

    /// <summary>
    /// Constructor that sets default values
    /// </summary>
    public QueueService() : base()
    {
        IsActive = true;
        Priority = 50; // Default medium priority
    }

    /// <summary>
    /// Creates a new queue-service relationship
    /// </summary>
    public QueueService(Guid queueId, Guid serviceId, Guid tenantId, string createdBy, int priority = 50) : this()
    {
        QueueId = queueId;
        ServiceId = serviceId;
        TenantId = tenantId;
        Priority = priority;
        SetCreated(createdBy);
    }

    /// <summary>
    /// Updates the priority of this service in the queue
    /// </summary>
    public void UpdatePriority(int newPriority, string updatedBy)
    {
        if (newPriority < 1 || newPriority > 100)
            throw new ArgumentOutOfRangeException(nameof(newPriority), "Priority must be between 1 and 100");

        Priority = newPriority;
        SetUpdated(updatedBy);
    }

    /// <summary>
    /// Activates this service in the queue
    /// </summary>
    public void Activate(string updatedBy)
    {
        IsActive = true;
        SetUpdated(updatedBy);
    }

    /// <summary>
    /// Deactivates this service in the queue
    /// </summary>
    public void Deactivate(string updatedBy)
    {
        IsActive = false;
        SetUpdated(updatedBy);
    }
}