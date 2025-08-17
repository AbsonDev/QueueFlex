namespace QueueManagement.Domain.Enums;

/// <summary>
/// Represents the current status of a ticket
/// </summary>
public enum TicketStatus
{
    /// <summary>
    /// Ticket is waiting in queue
    /// </summary>
    Waiting = 1,
    
    /// <summary>
    /// Ticket has been called for service
    /// </summary>
    Called = 2,
    
    /// <summary>
    /// Ticket is currently being processed
    /// </summary>
    InProgress = 3,
    
    /// <summary>
    /// Ticket has been completed
    /// </summary>
    Completed = 4,
    
    /// <summary>
    /// Ticket has been cancelled
    /// </summary>
    Cancelled = 5,
    
    /// <summary>
    /// Customer did not show up for service
    /// </summary>
    NoShow = 6
}