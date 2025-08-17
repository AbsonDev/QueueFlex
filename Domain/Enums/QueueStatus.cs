namespace QueueManagement.Domain.Enums;

/// <summary>
/// Represents the current status of a queue
/// </summary>
public enum QueueStatus
{
    /// <summary>
    /// Queue is open and accepting tickets
    /// </summary>
    Open = 1,
    
    /// <summary>
    /// Queue is closed and not accepting tickets
    /// </summary>
    Closed = 2,
    
    /// <summary>
    /// Queue is paused temporarily
    /// </summary>
    Paused = 3
}