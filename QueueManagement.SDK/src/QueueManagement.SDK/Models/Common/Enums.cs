namespace QueueManagement.SDK.Models.Common;

/// <summary>
/// Represents the status of a ticket in the queue.
/// </summary>
public enum TicketStatus
{
    /// <summary>
    /// Ticket is waiting in the queue.
    /// </summary>
    Waiting,
    
    /// <summary>
    /// Ticket has been called.
    /// </summary>
    Called,
    
    /// <summary>
    /// Ticket is currently being served.
    /// </summary>
    Serving,
    
    /// <summary>
    /// Ticket service has been completed.
    /// </summary>
    Completed,
    
    /// <summary>
    /// Ticket has been cancelled.
    /// </summary>
    Cancelled,
    
    /// <summary>
    /// Customer did not show up when called.
    /// </summary>
    NoShow,
    
    /// <summary>
    /// Ticket has been transferred to another queue.
    /// </summary>
    Transferred
}

/// <summary>
/// Represents the priority level of a ticket.
/// </summary>
public enum Priority
{
    /// <summary>
    /// Low priority.
    /// </summary>
    Low = 0,
    
    /// <summary>
    /// Normal priority (default).
    /// </summary>
    Normal = 1,
    
    /// <summary>
    /// High priority.
    /// </summary>
    High = 2,
    
    /// <summary>
    /// Urgent priority.
    /// </summary>
    Urgent = 3,
    
    /// <summary>
    /// VIP priority.
    /// </summary>
    VIP = 4
}

/// <summary>
/// Represents the status of a queue.
/// </summary>
public enum QueueStatus
{
    /// <summary>
    /// Queue is open and accepting new tickets.
    /// </summary>
    Open,
    
    /// <summary>
    /// Queue is paused temporarily.
    /// </summary>
    Paused,
    
    /// <summary>
    /// Queue is closed and not accepting new tickets.
    /// </summary>
    Closed,
    
    /// <summary>
    /// Queue is at full capacity.
    /// </summary>
    Full
}

/// <summary>
/// Represents the status of a service session.
/// </summary>
public enum SessionStatus
{
    /// <summary>
    /// Session is active.
    /// </summary>
    Active,
    
    /// <summary>
    /// Session is paused.
    /// </summary>
    Paused,
    
    /// <summary>
    /// Session has been completed.
    /// </summary>
    Completed,
    
    /// <summary>
    /// Session was terminated.
    /// </summary>
    Terminated
}

/// <summary>
/// Represents the type of service.
/// </summary>
public enum ServiceType
{
    /// <summary>
    /// Standard service.
    /// </summary>
    Standard,
    
    /// <summary>
    /// Express service.
    /// </summary>
    Express,
    
    /// <summary>
    /// Priority service.
    /// </summary>
    Priority,
    
    /// <summary>
    /// Appointment-based service.
    /// </summary>
    Appointment
}

/// <summary>
/// Represents the role of a user.
/// </summary>
public enum UserRole
{
    /// <summary>
    /// Service agent who serves customers.
    /// </summary>
    Agent,
    
    /// <summary>
    /// Supervisor who oversees operations.
    /// </summary>
    Supervisor,
    
    /// <summary>
    /// Manager with administrative privileges.
    /// </summary>
    Manager,
    
    /// <summary>
    /// System administrator.
    /// </summary>
    Admin,
    
    /// <summary>
    /// Read-only viewer.
    /// </summary>
    Viewer
}

/// <summary>
/// Represents the type of webhook event.
/// </summary>
public enum WebhookEventType
{
    /// <summary>
    /// Ticket was created.
    /// </summary>
    TicketCreated,
    
    /// <summary>
    /// Ticket was called.
    /// </summary>
    TicketCalled,
    
    /// <summary>
    /// Ticket service started.
    /// </summary>
    TicketServing,
    
    /// <summary>
    /// Ticket was completed.
    /// </summary>
    TicketCompleted,
    
    /// <summary>
    /// Ticket was cancelled.
    /// </summary>
    TicketCancelled,
    
    /// <summary>
    /// Ticket was transferred.
    /// </summary>
    TicketTransferred,
    
    /// <summary>
    /// Queue status changed.
    /// </summary>
    QueueStatusChanged,
    
    /// <summary>
    /// Session started.
    /// </summary>
    SessionStarted,
    
    /// <summary>
    /// Session completed.
    /// </summary>
    SessionCompleted,
    
    /// <summary>
    /// Unit opened.
    /// </summary>
    UnitOpened,
    
    /// <summary>
    /// Unit closed.
    /// </summary>
    UnitClosed
}