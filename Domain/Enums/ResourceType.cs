namespace QueueManagement.Domain.Enums;

/// <summary>
/// Represents the type of a resource
/// </summary>
public enum ResourceType
{
    /// <summary>
    /// Counter or desk resource
    /// </summary>
    Counter = 1,
    
    /// <summary>
    /// Room resource
    /// </summary>
    Room = 2,
    
    /// <summary>
    /// Equipment resource
    /// </summary>
    Equipment = 3,
    
    /// <summary>
    /// Kiosk or self-service resource
    /// </summary>
    Kiosk = 4
}