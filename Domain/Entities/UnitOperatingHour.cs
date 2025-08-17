using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace QueueManagement.Domain.Entities;

/// <summary>
/// Represents the operating hours for a unit on a specific day of the week
/// </summary>
public class UnitOperatingHour : BaseEntity
{
    /// <summary>
    /// Unit that this operating hour belongs to
    /// </summary>
    [Required]
    public Guid UnitId { get; set; }

    /// <summary>
    /// Day of the week
    /// </summary>
    [Required]
    public DayOfWeek DayOfWeek { get; set; }

    /// <summary>
    /// Time when the unit opens
    /// </summary>
    public TimeSpan? OpenTime { get; set; }

    /// <summary>
    /// Time when the unit closes
    /// </summary>
    public TimeSpan? CloseTime { get; set; }

    /// <summary>
    /// Time when the break starts
    /// </summary>
    public TimeSpan? BreakStart { get; set; }

    /// <summary>
    /// Time when the break ends
    /// </summary>
    public TimeSpan? BreakEnd { get; set; }

    /// <summary>
    /// Whether this operating hour is active
    /// </summary>
    [Required]
    public bool IsActive { get; set; }

    // Navigation properties
    /// <summary>
    /// Unit that owns this operating hour
    /// </summary>
    public virtual Unit Unit { get; set; } = null!;

    /// <summary>
    /// Constructor that sets default values
    /// </summary>
    public UnitOperatingHour() : base()
    {
        IsActive = true;
    }

    /// <summary>
    /// Creates a new operating hour
    /// </summary>
    public UnitOperatingHour(Guid unitId, DayOfWeek dayOfWeek, TimeSpan? openTime, TimeSpan? closeTime, 
        TimeSpan? breakStart = null, TimeSpan? breakEnd = null, string createdBy = "system") : this()
    {
        UnitId = unitId;
        DayOfWeek = dayOfWeek;
        OpenTime = openTime;
        CloseTime = closeTime;
        BreakStart = breakStart;
        BreakEnd = breakEnd;
        SetCreated(createdBy);
    }

    /// <summary>
    /// Checks if the unit is open at a specific time
    /// </summary>
    public bool IsOpenAt(TimeSpan time)
    {
        if (!IsActive || OpenTime == null || CloseTime == null)
            return false;

        // Handle cases where closing time is on the next day
        if (OpenTime <= CloseTime)
        {
            return time >= OpenTime && time <= CloseTime;
        }
        else
        {
            // Closing time is on the next day (e.g., 22:00 to 06:00)
            return time >= OpenTime || time <= CloseTime;
        }
    }

    /// <summary>
    /// Checks if the unit is on break at a specific time
    /// </summary>
    public bool IsOnBreakAt(TimeSpan time)
    {
        if (!IsActive || BreakStart == null || BreakEnd == null)
            return false;

        // Handle cases where break time crosses midnight
        if (BreakStart <= BreakEnd)
        {
            return time >= BreakStart && time <= BreakEnd;
        }
        else
        {
            // Break time crosses midnight
            return time >= BreakStart || time <= BreakEnd;
        }
    }

    /// <summary>
    /// Updates the operating hours
    /// </summary>
    public void UpdateHours(TimeSpan? openTime, TimeSpan? closeTime, TimeSpan? breakStart, TimeSpan? breakEnd, string updatedBy)
    {
        OpenTime = openTime;
        CloseTime = closeTime;
        BreakStart = breakStart;
        BreakEnd = breakEnd;
        SetUpdated(updatedBy);
    }

    /// <summary>
    /// Activates this operating hour
    /// </summary>
    public void Activate(string updatedBy)
    {
        IsActive = true;
        SetUpdated(updatedBy);
    }

    /// <summary>
    /// Deactivates this operating hour
    /// </summary>
    public void Deactivate(string updatedBy)
    {
        IsActive = false;
        SetUpdated(updatedBy);
    }
}