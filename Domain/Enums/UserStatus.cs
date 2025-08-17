namespace QueueManagement.Domain.Enums;

/// <summary>
/// Represents the current status of a user
/// </summary>
public enum UserStatus
{
    /// <summary>
    /// User is active and available
    /// </summary>
    Active = 1,
    
    /// <summary>
    /// User is inactive
    /// </summary>
    Inactive = 2,
    
    /// <summary>
    /// User is on break
    /// </summary>
    OnBreak = 3,
    
    /// <summary>
    /// User is busy with a task
    /// </summary>
    Busy = 4
}