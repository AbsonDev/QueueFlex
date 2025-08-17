namespace QueueManagement.Domain.Enums;

/// <summary>
/// Represents the priority level of a ticket
/// </summary>
public enum Priority
{
    /// <summary>
    /// Low priority ticket
    /// </summary>
    Low = 1,
    
    /// <summary>
    /// Normal priority ticket
    /// </summary>
    Normal = 2,
    
    /// <summary>
    /// High priority ticket
    /// </summary>
    High = 3,
    
    /// <summary>
    /// VIP priority ticket
    /// </summary>
    VIP = 4
}