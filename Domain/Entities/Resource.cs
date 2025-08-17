using System.ComponentModel.DataAnnotations;
using QueueManagement.Domain.Enums;

namespace QueueManagement.Domain.Entities;

/// <summary>
/// Represents a physical resource (counter, room, equipment, etc.)
/// </summary>
public class Resource : BaseEntity
{
    /// <summary>
    /// Unit that owns this resource
    /// </summary>
    [Required]
    public Guid UnitId { get; set; }

    /// <summary>
    /// Resource name
    /// </summary>
    [Required]
    [MaxLength(200)]
    public string Name { get; set; } = string.Empty;

    /// <summary>
    /// Unique code for the resource within the unit
    /// </summary>
    [Required]
    [MaxLength(50)]
    public string Code { get; set; } = string.Empty;

    /// <summary>
    /// Type of resource
    /// </summary>
    [Required]
    public ResourceType Type { get; set; }

    /// <summary>
    /// Physical location of the resource
    /// </summary>
    [MaxLength(200)]
    public string? Location { get; set; }

    /// <summary>
    /// Current status of the resource
    /// </summary>
    [Required]
    public ResourceStatus Status { get; set; }

    /// <summary>
    /// Whether this resource is active
    /// </summary>
    [Required]
    public bool IsActive { get; set; }

    // Navigation properties
    /// <summary>
    /// Unit that owns this resource
    /// </summary>
    public virtual Unit Unit { get; set; } = null!;

    /// <summary>
    /// Sessions that use this resource
    /// </summary>
    public virtual ICollection<Session> Sessions { get; set; } = new List<Session>();

    /// <summary>
    /// Constructor that sets default values
    /// </summary>
    public Resource() : base()
    {
        IsActive = true;
        Status = ResourceStatus.Available;
    }

    /// <summary>
    /// Creates a new resource
    /// </summary>
    public Resource(string name, string code, ResourceType type, Guid unitId, Guid tenantId, string createdBy, string? location = null) : this()
    {
        Name = name ?? throw new ArgumentNullException(nameof(name));
        Code = code ?? throw new ArgumentNullException(nameof(code));
        Type = type;
        UnitId = unitId;
        TenantId = tenantId;
        Location = location;
        SetCreated(createdBy);
    }

    /// <summary>
    /// Updates the resource status
    /// </summary>
    public void UpdateStatus(ResourceStatus newStatus, string updatedBy)
    {
        Status = newStatus;
        SetUpdated(updatedBy);
    }

    /// <summary>
    /// Updates the resource location
    /// </summary>
    public void UpdateLocation(string? newLocation, string updatedBy)
    {
        Location = newLocation;
        SetUpdated(updatedBy);
    }

    /// <summary>
    /// Activates this resource
    /// </summary>
    public void Activate(string updatedBy)
    {
        IsActive = true;
        SetUpdated(updatedBy);
    }

    /// <summary>
    /// Deactivates this resource
    /// </summary>
    public void Deactivate(string updatedBy)
    {
        IsActive = false;
        SetUpdated(updatedBy);
    }

    /// <summary>
    /// Marks the resource as occupied
    /// </summary>
    public void MarkAsOccupied(string updatedBy)
    {
        if (Status != ResourceStatus.Available)
            throw new InvalidOperationException("Only available resources can be marked as occupied");

        Status = ResourceStatus.Occupied;
        SetUpdated(updatedBy);
    }

    /// <summary>
    /// Marks the resource as available
    /// </summary>
    public void MarkAsAvailable(string updatedBy)
    {
        if (Status != ResourceStatus.Occupied)
            throw new InvalidOperationException("Only occupied resources can be marked as available");

        Status = ResourceStatus.Available;
        SetUpdated(updatedBy);
    }

    /// <summary>
    /// Marks the resource as under maintenance
    /// </summary>
    public void MarkAsMaintenance(string updatedBy)
    {
        Status = ResourceStatus.Maintenance;
        SetUpdated(updatedBy);
    }

    /// <summary>
    /// Marks the resource as out of order
    /// </summary>
    public void MarkAsOutOfOrder(string updatedBy)
    {
        Status = ResourceStatus.OutOfOrder;
        SetUpdated(updatedBy);
    }

    /// <summary>
    /// Checks if the resource is currently available for use
    /// </summary>
    public bool IsAvailableForUse => IsActive && Status == ResourceStatus.Available;

    /// <summary>
    /// Gets the current session using this resource
    /// </summary>
    public Session? GetCurrentSession()
    {
        return Sessions.FirstOrDefault(s => s.Status == SessionStatus.InProgress || s.Status == SessionStatus.Paused);
    }
}