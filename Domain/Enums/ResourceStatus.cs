namespace QueueManagement.Domain.Enums;

/// <summary>
/// Represents the current status of a resource
/// </summary>
public enum ResourceStatus
{
    /// <summary>
    /// Resource is available for use
    /// </summary>
    Available = 1,
    
    /// <summary>
    /// Resource is currently occupied
    /// </summary>
    Occupied = 2,
    
    /// <summary>
    /// Resource is under maintenance
    /// </summary>
    Maintenance = 3,
    
    /// <summary>
    /// Resource is out of order
    /// </summary>
    OutOfOrder = 4
}