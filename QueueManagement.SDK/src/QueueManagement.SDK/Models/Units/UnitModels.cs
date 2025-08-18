namespace QueueManagement.SDK.Models.Units;

/// <summary>
/// Represents a service unit/branch.
/// </summary>
public class UnitResponse
{
    /// <summary>
    /// Gets or sets the unique identifier of the unit.
    /// </summary>
    public Guid Id { get; set; }

    /// <summary>
    /// Gets or sets the unit name.
    /// </summary>
    public string Name { get; set; } = string.Empty;

    /// <summary>
    /// Gets or sets the unit code.
    /// </summary>
    public string Code { get; set; } = string.Empty;

    /// <summary>
    /// Gets or sets the unit description.
    /// </summary>
    public string? Description { get; set; }

    /// <summary>
    /// Gets or sets the address.
    /// </summary>
    public string? Address { get; set; }

    /// <summary>
    /// Gets or sets the city.
    /// </summary>
    public string? City { get; set; }

    /// <summary>
    /// Gets or sets the state/province.
    /// </summary>
    public string? State { get; set; }

    /// <summary>
    /// Gets or sets the country.
    /// </summary>
    public string? Country { get; set; }

    /// <summary>
    /// Gets or sets the postal code.
    /// </summary>
    public string? PostalCode { get; set; }

    /// <summary>
    /// Gets or sets the phone number.
    /// </summary>
    public string? Phone { get; set; }

    /// <summary>
    /// Gets or sets the email address.
    /// </summary>
    public string? Email { get; set; }

    /// <summary>
    /// Gets or sets the timezone.
    /// </summary>
    public string TimeZone { get; set; } = "UTC";

    /// <summary>
    /// Gets or sets whether the unit is currently open.
    /// </summary>
    public bool IsOpen { get; set; }

    /// <summary>
    /// Gets or sets the opening hours.
    /// </summary>
    public Dictionary<string, WorkingHours>? OpeningHours { get; set; }

    /// <summary>
    /// Gets or sets the list of queue IDs in this unit.
    /// </summary>
    public List<Guid> QueueIds { get; set; } = new();

    /// <summary>
    /// Gets or sets the list of service IDs available in this unit.
    /// </summary>
    public List<Guid> ServiceIds { get; set; } = new();

    /// <summary>
    /// Gets or sets the maximum daily capacity.
    /// </summary>
    public int MaxDailyCapacity { get; set; }

    /// <summary>
    /// Gets or sets custom metadata.
    /// </summary>
    public Dictionary<string, object>? Metadata { get; set; }

    /// <summary>
    /// Gets or sets when the unit was created.
    /// </summary>
    public DateTime CreatedAt { get; set; }

    /// <summary>
    /// Gets or sets when the unit was last updated.
    /// </summary>
    public DateTime UpdatedAt { get; set; }
}

/// <summary>
/// Represents working hours for a day.
/// </summary>
public class WorkingHours
{
    /// <summary>
    /// Gets or sets whether the unit is open on this day.
    /// </summary>
    public bool IsOpen { get; set; }

    /// <summary>
    /// Gets or sets the opening time (HH:mm format).
    /// </summary>
    public string? OpenTime { get; set; }

    /// <summary>
    /// Gets or sets the closing time (HH:mm format).
    /// </summary>
    public string? CloseTime { get; set; }

    /// <summary>
    /// Gets or sets break periods.
    /// </summary>
    public List<BreakPeriod>? Breaks { get; set; }
}

/// <summary>
/// Represents a break period.
/// </summary>
public class BreakPeriod
{
    /// <summary>
    /// Gets or sets the break start time (HH:mm format).
    /// </summary>
    public string StartTime { get; set; } = string.Empty;

    /// <summary>
    /// Gets or sets the break end time (HH:mm format).
    /// </summary>
    public string EndTime { get; set; } = string.Empty;

    /// <summary>
    /// Gets or sets the break description.
    /// </summary>
    public string? Description { get; set; }
}

/// <summary>
/// Request to create a new unit.
/// </summary>
public class CreateUnitRequest
{
    /// <summary>
    /// Gets or sets the unit name.
    /// </summary>
    public string Name { get; set; } = string.Empty;

    /// <summary>
    /// Gets or sets the unit code.
    /// </summary>
    public string Code { get; set; } = string.Empty;

    /// <summary>
    /// Gets or sets the unit description (optional).
    /// </summary>
    public string? Description { get; set; }

    /// <summary>
    /// Gets or sets the address (optional).
    /// </summary>
    public string? Address { get; set; }

    /// <summary>
    /// Gets or sets the city (optional).
    /// </summary>
    public string? City { get; set; }

    /// <summary>
    /// Gets or sets the state/province (optional).
    /// </summary>
    public string? State { get; set; }

    /// <summary>
    /// Gets or sets the country (optional).
    /// </summary>
    public string? Country { get; set; }

    /// <summary>
    /// Gets or sets the postal code (optional).
    /// </summary>
    public string? PostalCode { get; set; }

    /// <summary>
    /// Gets or sets the phone number (optional).
    /// </summary>
    public string? Phone { get; set; }

    /// <summary>
    /// Gets or sets the email address (optional).
    /// </summary>
    public string? Email { get; set; }

    /// <summary>
    /// Gets or sets the timezone (default: UTC).
    /// </summary>
    public string TimeZone { get; set; } = "UTC";

    /// <summary>
    /// Gets or sets the opening hours (optional).
    /// </summary>
    public Dictionary<string, WorkingHours>? OpeningHours { get; set; }

    /// <summary>
    /// Gets or sets the maximum daily capacity (default: 1000).
    /// </summary>
    public int MaxDailyCapacity { get; set; } = 1000;

    /// <summary>
    /// Gets or sets custom metadata (optional).
    /// </summary>
    public Dictionary<string, object>? Metadata { get; set; }
}

/// <summary>
/// Request to update an existing unit.
/// </summary>
public class UpdateUnitRequest
{
    /// <summary>
    /// Gets or sets the updated name (optional).
    /// </summary>
    public string? Name { get; set; }

    /// <summary>
    /// Gets or sets the updated description (optional).
    /// </summary>
    public string? Description { get; set; }

    /// <summary>
    /// Gets or sets the updated address (optional).
    /// </summary>
    public string? Address { get; set; }

    /// <summary>
    /// Gets or sets the updated city (optional).
    /// </summary>
    public string? City { get; set; }

    /// <summary>
    /// Gets or sets the updated state/province (optional).
    /// </summary>
    public string? State { get; set; }

    /// <summary>
    /// Gets or sets the updated country (optional).
    /// </summary>
    public string? Country { get; set; }

    /// <summary>
    /// Gets or sets the updated postal code (optional).
    /// </summary>
    public string? PostalCode { get; set; }

    /// <summary>
    /// Gets or sets the updated phone number (optional).
    /// </summary>
    public string? Phone { get; set; }

    /// <summary>
    /// Gets or sets the updated email address (optional).
    /// </summary>
    public string? Email { get; set; }

    /// <summary>
    /// Gets or sets the updated timezone (optional).
    /// </summary>
    public string? TimeZone { get; set; }

    /// <summary>
    /// Gets or sets the updated opening hours (optional).
    /// </summary>
    public Dictionary<string, WorkingHours>? OpeningHours { get; set; }

    /// <summary>
    /// Gets or sets the updated maximum daily capacity (optional).
    /// </summary>
    public int? MaxDailyCapacity { get; set; }

    /// <summary>
    /// Gets or sets updated metadata (optional).
    /// </summary>
    public Dictionary<string, object>? Metadata { get; set; }
}