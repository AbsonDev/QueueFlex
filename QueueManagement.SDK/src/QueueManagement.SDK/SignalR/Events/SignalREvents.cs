using QueueManagement.SDK.Models.Common;
using QueueManagement.SDK.Models.Dashboard;
using QueueManagement.SDK.Models.Queues;
using QueueManagement.SDK.Models.Sessions;
using QueueManagement.SDK.Models.Tickets;

namespace QueueManagement.SDK.SignalR.Events;

/// <summary>
/// Base class for SignalR event arguments.
/// </summary>
public abstract class SignalREventArgs : EventArgs
{
    /// <summary>
    /// Gets or sets the timestamp of the event.
    /// </summary>
    public DateTime Timestamp { get; set; } = DateTime.UtcNow;

    /// <summary>
    /// Gets or sets the event ID.
    /// </summary>
    public Guid EventId { get; set; } = Guid.NewGuid();
}

/// <summary>
/// Event arguments for connection events.
/// </summary>
public class ConnectedEventArgs : SignalREventArgs
{
    /// <summary>
    /// Gets or sets the connection ID.
    /// </summary>
    public string ConnectionId { get; set; } = string.Empty;
}

/// <summary>
/// Event arguments for disconnection events.
/// </summary>
public class DisconnectedEventArgs : SignalREventArgs
{
    /// <summary>
    /// Gets or sets the disconnection reason.
    /// </summary>
    public string? Reason { get; set; }

    /// <summary>
    /// Gets or sets the exception if any.
    /// </summary>
    public Exception? Exception { get; set; }
}

/// <summary>
/// Event arguments for reconnecting events.
/// </summary>
public class ReconnectingEventArgs : SignalREventArgs
{
    /// <summary>
    /// Gets or sets the retry attempt number.
    /// </summary>
    public int RetryAttempt { get; set; }

    /// <summary>
    /// Gets or sets the retry delay.
    /// </summary>
    public TimeSpan RetryDelay { get; set; }
}

/// <summary>
/// Event arguments for reconnected events.
/// </summary>
public class ReconnectedEventArgs : SignalREventArgs
{
    /// <summary>
    /// Gets or sets the new connection ID.
    /// </summary>
    public string ConnectionId { get; set; } = string.Empty;

    /// <summary>
    /// Gets or sets the total reconnection time.
    /// </summary>
    public TimeSpan ReconnectionTime { get; set; }
}

/// <summary>
/// Event arguments for ticket created events.
/// </summary>
public class TicketCreatedEventArgs : SignalREventArgs
{
    /// <summary>
    /// Gets or sets the created ticket.
    /// </summary>
    public TicketResponse Ticket { get; set; } = default!;

    /// <summary>
    /// Gets or sets the queue ID.
    /// </summary>
    public Guid QueueId { get; set; }

    /// <summary>
    /// Gets or sets the unit ID.
    /// </summary>
    public Guid UnitId { get; set; }
}

/// <summary>
/// Event arguments for ticket called events.
/// </summary>
public class TicketCalledEventArgs : SignalREventArgs
{
    /// <summary>
    /// Gets or sets the ticket ID.
    /// </summary>
    public Guid TicketId { get; set; }

    /// <summary>
    /// Gets or sets the ticket number.
    /// </summary>
    public string TicketNumber { get; set; } = string.Empty;

    /// <summary>
    /// Gets or sets the queue ID.
    /// </summary>
    public Guid QueueId { get; set; }

    /// <summary>
    /// Gets or sets the counter number.
    /// </summary>
    public string? CounterNumber { get; set; }

    /// <summary>
    /// Gets or sets the agent name.
    /// </summary>
    public string? AgentName { get; set; }

    /// <summary>
    /// Gets or sets the agent ID.
    /// </summary>
    public Guid? AgentId { get; set; }
}

/// <summary>
/// Event arguments for ticket serving events.
/// </summary>
public class TicketServingEventArgs : SignalREventArgs
{
    /// <summary>
    /// Gets or sets the ticket ID.
    /// </summary>
    public Guid TicketId { get; set; }

    /// <summary>
    /// Gets or sets the ticket number.
    /// </summary>
    public string TicketNumber { get; set; } = string.Empty;

    /// <summary>
    /// Gets or sets the queue ID.
    /// </summary>
    public Guid QueueId { get; set; }

    /// <summary>
    /// Gets or sets the agent ID.
    /// </summary>
    public Guid AgentId { get; set; }

    /// <summary>
    /// Gets or sets the session ID.
    /// </summary>
    public Guid SessionId { get; set; }
}

/// <summary>
/// Event arguments for ticket completed events.
/// </summary>
public class TicketCompletedEventArgs : SignalREventArgs
{
    /// <summary>
    /// Gets or sets the ticket ID.
    /// </summary>
    public Guid TicketId { get; set; }

    /// <summary>
    /// Gets or sets the ticket number.
    /// </summary>
    public string TicketNumber { get; set; } = string.Empty;

    /// <summary>
    /// Gets or sets the queue ID.
    /// </summary>
    public Guid QueueId { get; set; }

    /// <summary>
    /// Gets or sets the service duration in minutes.
    /// </summary>
    public double ServiceDurationMinutes { get; set; }

    /// <summary>
    /// Gets or sets the wait time in minutes.
    /// </summary>
    public double WaitTimeMinutes { get; set; }
}

/// <summary>
/// Event arguments for ticket cancelled events.
/// </summary>
public class TicketCancelledEventArgs : SignalREventArgs
{
    /// <summary>
    /// Gets or sets the ticket ID.
    /// </summary>
    public Guid TicketId { get; set; }

    /// <summary>
    /// Gets or sets the ticket number.
    /// </summary>
    public string TicketNumber { get; set; } = string.Empty;

    /// <summary>
    /// Gets or sets the queue ID.
    /// </summary>
    public Guid QueueId { get; set; }

    /// <summary>
    /// Gets or sets the cancellation reason.
    /// </summary>
    public string? Reason { get; set; }
}

/// <summary>
/// Event arguments for ticket transferred events.
/// </summary>
public class TicketTransferredEventArgs : SignalREventArgs
{
    /// <summary>
    /// Gets or sets the ticket ID.
    /// </summary>
    public Guid TicketId { get; set; }

    /// <summary>
    /// Gets or sets the ticket number.
    /// </summary>
    public string TicketNumber { get; set; } = string.Empty;

    /// <summary>
    /// Gets or sets the source queue ID.
    /// </summary>
    public Guid SourceQueueId { get; set; }

    /// <summary>
    /// Gets or sets the target queue ID.
    /// </summary>
    public Guid TargetQueueId { get; set; }

    /// <summary>
    /// Gets or sets the transfer reason.
    /// </summary>
    public string? Reason { get; set; }
}

/// <summary>
/// Event arguments for queue status changed events.
/// </summary>
public class QueueStatusChangedEventArgs : SignalREventArgs
{
    /// <summary>
    /// Gets or sets the queue ID.
    /// </summary>
    public Guid QueueId { get; set; }

    /// <summary>
    /// Gets or sets the queue name.
    /// </summary>
    public string QueueName { get; set; } = string.Empty;

    /// <summary>
    /// Gets or sets the old status.
    /// </summary>
    public QueueStatus OldStatus { get; set; }

    /// <summary>
    /// Gets or sets the new status.
    /// </summary>
    public QueueStatus NewStatus { get; set; }
}

/// <summary>
/// Event arguments for queue metrics updated events.
/// </summary>
public class QueueMetricsUpdatedEventArgs : SignalREventArgs
{
    /// <summary>
    /// Gets or sets the queue metrics.
    /// </summary>
    public QueueMetricsResponse Metrics { get; set; } = default!;
}

/// <summary>
/// Event arguments for session started events.
/// </summary>
public class SessionStartedEventArgs : SignalREventArgs
{
    /// <summary>
    /// Gets or sets the session.
    /// </summary>
    public SessionResponse Session { get; set; } = default!;
}

/// <summary>
/// Event arguments for session completed events.
/// </summary>
public class SessionCompletedEventArgs : SignalREventArgs
{
    /// <summary>
    /// Gets or sets the session ID.
    /// </summary>
    public Guid SessionId { get; set; }

    /// <summary>
    /// Gets or sets the user ID.
    /// </summary>
    public Guid UserId { get; set; }

    /// <summary>
    /// Gets or sets the tickets served count.
    /// </summary>
    public int TicketsServed { get; set; }

    /// <summary>
    /// Gets or sets the session duration in minutes.
    /// </summary>
    public double SessionDurationMinutes { get; set; }
}

/// <summary>
/// Event arguments for dashboard metrics updated events.
/// </summary>
public class DashboardMetricsUpdatedEventArgs : SignalREventArgs
{
    /// <summary>
    /// Gets or sets the dashboard metrics.
    /// </summary>
    public DashboardMetricsResponse Metrics { get; set; } = default!;
}

/// <summary>
/// Event arguments for alert triggered events.
/// </summary>
public class AlertTriggeredEventArgs : SignalREventArgs
{
    /// <summary>
    /// Gets or sets the alert.
    /// </summary>
    public Alert Alert { get; set; } = default!;
}