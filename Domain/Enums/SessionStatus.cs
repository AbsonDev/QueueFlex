namespace QueueManagement.Domain.Enums;

/// <summary>
/// Represents the current status of a service session
/// </summary>
public enum SessionStatus
{
    /// <summary>
    /// Session is in progress
    /// </summary>
    InProgress = 1,
    
    /// <summary>
    /// Session is paused
    /// </summary>
    Paused = 2,
    
    /// <summary>
    /// Session has been completed
    /// </summary>
    Completed = 3,
    
    /// <summary>
    /// Session has been cancelled
    /// </summary>
    Cancelled = 4
}