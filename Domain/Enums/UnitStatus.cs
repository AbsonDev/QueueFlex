namespace QueueManagement.Domain.Enums;

/// <summary>
/// Represents the operational status of a unit
/// </summary>
public enum UnitStatus
{
    /// <summary>
    /// Unit is active and operational
    /// </summary>
    Active = 1,
    
    /// <summary>
    /// Unit is inactive and not operational
    /// </summary>
    Inactive = 2,
    
    /// <summary>
    /// Unit is under maintenance
    /// </summary>
    Maintenance = 3
}