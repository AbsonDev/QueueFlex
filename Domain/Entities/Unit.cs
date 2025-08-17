using System.ComponentModel.DataAnnotations;
using QueueManagement.Domain.Enums;
using QueueManagement.Domain.ValueObjects;

namespace QueueManagement.Domain.Entities;

/// <summary>
/// Represents a unit (branch/office) of a tenant
/// </summary>
public class Unit : BaseEntity
{
    /// <summary>
    /// Unit name
    /// </summary>
    [Required]
    [MaxLength(200)]
    public string Name { get; set; } = string.Empty;

    /// <summary>
    /// Unique code for the unit within the tenant
    /// </summary>
    [Required]
    [MaxLength(50)]
    public string Code { get; set; } = string.Empty;

    /// <summary>
    /// Physical address of the unit
    /// </summary>
    [Required]
    public Address Address { get; set; } = null!;

    /// <summary>
    /// Current status of the unit
    /// </summary>
    [Required]
    public UnitStatus Status { get; set; }

    // Navigation properties
    /// <summary>
    /// Tenant that owns this unit
    /// </summary>
    public virtual Tenant Tenant { get; set; } = null!;

    /// <summary>
    /// Queues associated with this unit
    /// </summary>
    public virtual ICollection<Queue> Queues { get; set; } = new List<Queue>();

    /// <summary>
    /// Operating hours for this unit
    /// </summary>
    public virtual ICollection<UnitOperatingHour> OperatingHours { get; set; } = new List<UnitOperatingHour>();

    /// <summary>
    /// Users assigned to this unit
    /// </summary>
    public virtual ICollection<UnitUser> UnitUsers { get; set; } = new List<UnitUser>();

    /// <summary>
    /// Resources available at this unit
    /// </summary>
    public virtual ICollection<Resource> Resources { get; set; } = new List<Resource>();

    /// <summary>
    /// Constructor that sets default values
    /// </summary>
    public Unit() : base()
    {
        Status = UnitStatus.Active;
    }

    /// <summary>
    /// Creates a new unit
    /// </summary>
    public Unit(string name, string code, Address address, Guid tenantId, string createdBy) : this()
    {
        Name = name ?? throw new ArgumentNullException(nameof(name));
        Code = code ?? throw new ArgumentNullException(nameof(code));
        Address = address ?? throw new ArgumentNullException(nameof(address));
        TenantId = tenantId;
        SetCreated(createdBy);
    }

    /// <summary>
    /// Updates the unit status
    /// </summary>
    public void UpdateStatus(UnitStatus newStatus, string updatedBy)
    {
        Status = newStatus;
        SetUpdated(updatedBy);
    }

    /// <summary>
    /// Updates the unit address
    /// </summary>
    public void UpdateAddress(Address newAddress, string updatedBy)
    {
        Address = newAddress ?? throw new ArgumentNullException(nameof(newAddress));
        SetUpdated(updatedBy);
    }

    /// <summary>
    /// Checks if the unit is currently open based on operating hours
    /// </summary>
    public bool IsOpen(DateTime currentTime)
    {
        var dayOfWeek = currentTime.DayOfWeek;
        var timeOfDay = currentTime.TimeOfDay;

        var operatingHour = OperatingHours
            .FirstOrDefault(oh => oh.DayOfWeek == dayOfWeek && oh.IsActive);

        if (operatingHour == null)
            return false;

        return operatingHour.OpenTime <= timeOfDay && timeOfDay <= operatingHour.CloseTime;
    }
}