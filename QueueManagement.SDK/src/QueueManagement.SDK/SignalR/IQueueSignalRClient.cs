using QueueManagement.SDK.SignalR.Events;

namespace QueueManagement.SDK.SignalR;

/// <summary>
/// Interface for SignalR real-time client.
/// </summary>
public interface IQueueSignalRClient : IDisposable
{
    /// <summary>
    /// Gets whether the client is connected.
    /// </summary>
    bool IsConnected { get; }

    /// <summary>
    /// Gets the connection state.
    /// </summary>
    ConnectionState State { get; }

    /// <summary>
    /// Connects to the SignalR hub.
    /// </summary>
    Task ConnectAsync(CancellationToken cancellationToken = default);

    /// <summary>
    /// Disconnects from the SignalR hub.
    /// </summary>
    Task DisconnectAsync(CancellationToken cancellationToken = default);

    /// <summary>
    /// Joins a queue for real-time updates.
    /// </summary>
    Task JoinQueueAsync(Guid queueId, CancellationToken cancellationToken = default);

    /// <summary>
    /// Leaves a queue.
    /// </summary>
    Task LeaveQueueAsync(Guid queueId, CancellationToken cancellationToken = default);

    /// <summary>
    /// Joins a unit for real-time updates.
    /// </summary>
    Task JoinUnitAsync(Guid unitId, CancellationToken cancellationToken = default);

    /// <summary>
    /// Leaves a unit.
    /// </summary>
    Task LeaveUnitAsync(Guid unitId, CancellationToken cancellationToken = default);

    /// <summary>
    /// Joins the dashboard for global updates.
    /// </summary>
    Task JoinDashboardAsync(CancellationToken cancellationToken = default);

    /// <summary>
    /// Leaves the dashboard.
    /// </summary>
    Task LeaveDashboardAsync(CancellationToken cancellationToken = default);

    /// <summary>
    /// Requests current queue status.
    /// </summary>
    Task RequestQueueStatusAsync(Guid queueId, CancellationToken cancellationToken = default);

    /// <summary>
    /// Requests current dashboard metrics.
    /// </summary>
    Task RequestDashboardMetricsAsync(CancellationToken cancellationToken = default);

    // Events
    /// <summary>
    /// Occurs when connected to the hub.
    /// </summary>
    event EventHandler<ConnectedEventArgs>? Connected;

    /// <summary>
    /// Occurs when disconnected from the hub.
    /// </summary>
    event EventHandler<DisconnectedEventArgs>? Disconnected;

    /// <summary>
    /// Occurs when reconnecting to the hub.
    /// </summary>
    event EventHandler<ReconnectingEventArgs>? Reconnecting;

    /// <summary>
    /// Occurs when reconnected to the hub.
    /// </summary>
    event EventHandler<ReconnectedEventArgs>? Reconnected;

    /// <summary>
    /// Occurs when a ticket is created.
    /// </summary>
    event EventHandler<TicketCreatedEventArgs>? TicketCreated;

    /// <summary>
    /// Occurs when a ticket is called.
    /// </summary>
    event EventHandler<TicketCalledEventArgs>? TicketCalled;

    /// <summary>
    /// Occurs when ticket service starts.
    /// </summary>
    event EventHandler<TicketServingEventArgs>? TicketServing;

    /// <summary>
    /// Occurs when a ticket is completed.
    /// </summary>
    event EventHandler<TicketCompletedEventArgs>? TicketCompleted;

    /// <summary>
    /// Occurs when a ticket is cancelled.
    /// </summary>
    event EventHandler<TicketCancelledEventArgs>? TicketCancelled;

    /// <summary>
    /// Occurs when a ticket is transferred.
    /// </summary>
    event EventHandler<TicketTransferredEventArgs>? TicketTransferred;

    /// <summary>
    /// Occurs when queue status changes.
    /// </summary>
    event EventHandler<QueueStatusChangedEventArgs>? QueueStatusChanged;

    /// <summary>
    /// Occurs when queue metrics are updated.
    /// </summary>
    event EventHandler<QueueMetricsUpdatedEventArgs>? QueueMetricsUpdated;

    /// <summary>
    /// Occurs when a session starts.
    /// </summary>
    event EventHandler<SessionStartedEventArgs>? SessionStarted;

    /// <summary>
    /// Occurs when a session is completed.
    /// </summary>
    event EventHandler<SessionCompletedEventArgs>? SessionCompleted;

    /// <summary>
    /// Occurs when dashboard metrics are updated.
    /// </summary>
    event EventHandler<DashboardMetricsUpdatedEventArgs>? DashboardMetricsUpdated;

    /// <summary>
    /// Occurs when an alert is triggered.
    /// </summary>
    event EventHandler<AlertTriggeredEventArgs>? AlertTriggered;
}

/// <summary>
/// Represents the connection state.
/// </summary>
public enum ConnectionState
{
    /// <summary>
    /// Disconnected state.
    /// </summary>
    Disconnected,

    /// <summary>
    /// Connecting state.
    /// </summary>
    Connecting,

    /// <summary>
    /// Connected state.
    /// </summary>
    Connected,

    /// <summary>
    /// Reconnecting state.
    /// </summary>
    Reconnecting
}