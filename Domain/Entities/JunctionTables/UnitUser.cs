using System.ComponentModel.DataAnnotations;

namespace QueueManagement.Domain.Entities.JunctionTables;

/// <summary>
/// Junction table for many-to-many relationship between Unit and User
/// </summary>
public class UnitUser : BaseEntity
{
    /// <summary>
    /// Unit identifier
    /// </summary>
    [Required]
    public Guid UnitId { get; set; }

    /// <summary>
    /// User identifier
    /// </summary>
    [Required]
    public Guid UserId { get; set; }

    /// <summary>
    /// Whether this assignment is active
    /// </summary>
    [Required]
    public bool IsActive { get; set; }

    /// <summary>
    /// Date and time when the user was assigned to the unit
    /// </summary>
    [Required]
    public DateTime AssignedAt { get; set; }

    // Navigation properties
    /// <summary>
    /// Unit this user is assigned to
    /// </summary>
    public virtual Unit Unit { get; set; } = null!;

    /// <summary>
    /// User assigned to this unit
    /// </summary>
    public virtual User User { get; set; } = null!;

    /// <summary>
    /// Constructor that sets default values
    /// </summary>
    public UnitUser() : base()
    {
        IsActive = true;
        AssignedAt = DateTime.UtcNow;
    }

    /// <summary>
    /// Creates a new unit-user assignment
    /// </summary>
    public UnitUser(Guid unitId, Guid userId, Guid tenantId, string createdBy) : this()
    {
        UnitId = unitId;
        UserId = userId;
        TenantId = tenantId;
        SetCreated(createdBy);
    }

    /// <summary>
    /// Activates this assignment
    /// </summary>
    public void Activate(string updatedBy)
    {
        IsActive = true;
        SetUpdated(updatedBy);
    }

    /// <summary>
    /// Deactivates this assignment
    /// </summary>
    public void Deactivate(string updatedBy)
    {
        IsActive = false;
        SetUpdated(updatedBy);
    }

    /// <summary>
    /// Updates the assignment date
    /// </summary>
    public void UpdateAssignmentDate(string updatedBy)
    {
        AssignedAt = DateTime.UtcNow;
        SetUpdated(updatedBy);
    }
}