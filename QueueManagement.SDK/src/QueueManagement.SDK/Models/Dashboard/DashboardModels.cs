using QueueManagement.SDK.Models.Common;

namespace QueueManagement.SDK.Models.Dashboard;

/// <summary>
/// Represents the overall dashboard metrics.
/// </summary>
public class DashboardMetricsResponse
{
    /// <summary>
    /// Gets or sets the timestamp of the metrics.
    /// </summary>
    public DateTime Timestamp { get; set; }

    /// <summary>
    /// Gets or sets the total active units.
    /// </summary>
    public int ActiveUnits { get; set; }

    /// <summary>
    /// Gets or sets the total active queues.
    /// </summary>
    public int ActiveQueues { get; set; }

    /// <summary>
    /// Gets or sets the total active sessions.
    /// </summary>
    public int ActiveSessions { get; set; }

    /// <summary>
    /// Gets or sets the total waiting tickets.
    /// </summary>
    public int TotalWaitingTickets { get; set; }

    /// <summary>
    /// Gets or sets the total tickets being served.
    /// </summary>
    public int TotalServingTickets { get; set; }

    /// <summary>
    /// Gets or sets the total tickets created today.
    /// </summary>
    public int TicketsCreatedToday { get; set; }

    /// <summary>
    /// Gets or sets the total tickets completed today.
    /// </summary>
    public int TicketsCompletedToday { get; set; }

    /// <summary>
    /// Gets or sets the average wait time across all queues (in minutes).
    /// </summary>
    public double AverageWaitTime { get; set; }

    /// <summary>
    /// Gets or sets the average service time across all queues (in minutes).
    /// </summary>
    public double AverageServiceTime { get; set; }

    /// <summary>
    /// Gets or sets the service level percentage (tickets served within target time).
    /// </summary>
    public double ServiceLevelPercentage { get; set; }

    /// <summary>
    /// Gets or sets the customer satisfaction score.
    /// </summary>
    public double? CustomerSatisfactionScore { get; set; }

    /// <summary>
    /// Gets or sets metrics by unit.
    /// </summary>
    public List<UnitMetrics>? UnitMetrics { get; set; }

    /// <summary>
    /// Gets or sets metrics by queue.
    /// </summary>
    public List<QueueMetrics>? QueueMetrics { get; set; }

    /// <summary>
    /// Gets or sets agent performance metrics.
    /// </summary>
    public List<AgentPerformance>? AgentPerformance { get; set; }
}

/// <summary>
/// Represents metrics for a specific unit.
/// </summary>
public class UnitMetrics
{
    /// <summary>
    /// Gets or sets the unit ID.
    /// </summary>
    public Guid UnitId { get; set; }

    /// <summary>
    /// Gets or sets the unit name.
    /// </summary>
    public string UnitName { get; set; } = string.Empty;

    /// <summary>
    /// Gets or sets whether the unit is open.
    /// </summary>
    public bool IsOpen { get; set; }

    /// <summary>
    /// Gets or sets the number of active queues.
    /// </summary>
    public int ActiveQueues { get; set; }

    /// <summary>
    /// Gets or sets the number of active sessions.
    /// </summary>
    public int ActiveSessions { get; set; }

    /// <summary>
    /// Gets or sets the total waiting tickets.
    /// </summary>
    public int WaitingTickets { get; set; }

    /// <summary>
    /// Gets or sets the total serving tickets.
    /// </summary>
    public int ServingTickets { get; set; }

    /// <summary>
    /// Gets or sets the tickets completed today.
    /// </summary>
    public int CompletedToday { get; set; }

    /// <summary>
    /// Gets or sets the average wait time (in minutes).
    /// </summary>
    public double AverageWaitTime { get; set; }

    /// <summary>
    /// Gets or sets the average service time (in minutes).
    /// </summary>
    public double AverageServiceTime { get; set; }

    /// <summary>
    /// Gets or sets the capacity utilization percentage.
    /// </summary>
    public double CapacityUtilization { get; set; }
}

/// <summary>
/// Represents metrics for a specific queue.
/// </summary>
public class QueueMetrics
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
    /// Gets or sets the queue status.
    /// </summary>
    public QueueStatus Status { get; set; }

    /// <summary>
    /// Gets or sets the number of waiting tickets.
    /// </summary>
    public int WaitingTickets { get; set; }

    /// <summary>
    /// Gets or sets the number of serving tickets.
    /// </summary>
    public int ServingTickets { get; set; }

    /// <summary>
    /// Gets or sets the tickets completed today.
    /// </summary>
    public int CompletedToday { get; set; }

    /// <summary>
    /// Gets or sets the average wait time (in minutes).
    /// </summary>
    public double AverageWaitTime { get; set; }

    /// <summary>
    /// Gets or sets the average service time (in minutes).
    /// </summary>
    public double AverageServiceTime { get; set; }

    /// <summary>
    /// Gets or sets the number of active agents.
    /// </summary>
    public int ActiveAgents { get; set; }

    /// <summary>
    /// Gets or sets the longest wait time (in minutes).
    /// </summary>
    public double LongestWaitTime { get; set; }

    /// <summary>
    /// Gets or sets the service level percentage.
    /// </summary>
    public double ServiceLevelPercentage { get; set; }
}

/// <summary>
/// Represents performance metrics for an agent.
/// </summary>
public class AgentPerformance
{
    /// <summary>
    /// Gets or sets the agent ID.
    /// </summary>
    public Guid AgentId { get; set; }

    /// <summary>
    /// Gets or sets the agent name.
    /// </summary>
    public string AgentName { get; set; } = string.Empty;

    /// <summary>
    /// Gets or sets the session ID if currently active.
    /// </summary>
    public Guid? SessionId { get; set; }

    /// <summary>
    /// Gets or sets the counter number.
    /// </summary>
    public string? CounterNumber { get; set; }

    /// <summary>
    /// Gets or sets the current status.
    /// </summary>
    public string Status { get; set; } = string.Empty;

    /// <summary>
    /// Gets or sets the tickets served today.
    /// </summary>
    public int TicketsServedToday { get; set; }

    /// <summary>
    /// Gets or sets the average service time (in minutes).
    /// </summary>
    public double AverageServiceTime { get; set; }

    /// <summary>
    /// Gets or sets the total service time today (in minutes).
    /// </summary>
    public double TotalServiceTime { get; set; }

    /// <summary>
    /// Gets or sets the total pause time today (in minutes).
    /// </summary>
    public double TotalPauseTime { get; set; }

    /// <summary>
    /// Gets or sets the efficiency percentage.
    /// </summary>
    public double EfficiencyPercentage { get; set; }

    /// <summary>
    /// Gets or sets the customer satisfaction score.
    /// </summary>
    public double? CustomerSatisfactionScore { get; set; }

    /// <summary>
    /// Gets or sets the current ticket being served.
    /// </summary>
    public string? CurrentTicket { get; set; }

    /// <summary>
    /// Gets or sets when the agent started their session.
    /// </summary>
    public DateTime? SessionStartTime { get; set; }
}

/// <summary>
/// Request to get dashboard metrics.
/// </summary>
public class DashboardMetricsRequest
{
    /// <summary>
    /// Gets or sets the unit ID to filter by (optional).
    /// </summary>
    public Guid? UnitId { get; set; }

    /// <summary>
    /// Gets or sets whether to include unit metrics.
    /// </summary>
    public bool IncludeUnitMetrics { get; set; } = true;

    /// <summary>
    /// Gets or sets whether to include queue metrics.
    /// </summary>
    public bool IncludeQueueMetrics { get; set; } = true;

    /// <summary>
    /// Gets or sets whether to include agent performance.
    /// </summary>
    public bool IncludeAgentPerformance { get; set; } = true;

    /// <summary>
    /// Gets or sets the date to get metrics for (default: today).
    /// </summary>
    public DateTime? Date { get; set; }
}

/// <summary>
/// Represents real-time statistics.
/// </summary>
public class RealTimeStatistics
{
    /// <summary>
    /// Gets or sets the timestamp.
    /// </summary>
    public DateTime Timestamp { get; set; }

    /// <summary>
    /// Gets or sets the tickets created in the last minute.
    /// </summary>
    public int TicketsCreatedLastMinute { get; set; }

    /// <summary>
    /// Gets or sets the tickets called in the last minute.
    /// </summary>
    public int TicketsCalledLastMinute { get; set; }

    /// <summary>
    /// Gets or sets the tickets completed in the last minute.
    /// </summary>
    public int TicketsCompletedLastMinute { get; set; }

    /// <summary>
    /// Gets or sets the current wait time trend (positive = increasing).
    /// </summary>
    public double WaitTimeTrend { get; set; }

    /// <summary>
    /// Gets or sets the current service time trend (positive = increasing).
    /// </summary>
    public double ServiceTimeTrend { get; set; }

    /// <summary>
    /// Gets or sets active alerts.
    /// </summary>
    public List<Alert>? Alerts { get; set; }
}

/// <summary>
/// Represents a system alert.
/// </summary>
public class Alert
{
    /// <summary>
    /// Gets or sets the alert ID.
    /// </summary>
    public Guid Id { get; set; }

    /// <summary>
    /// Gets or sets the alert type.
    /// </summary>
    public string Type { get; set; } = string.Empty;

    /// <summary>
    /// Gets or sets the alert severity (Info, Warning, Error, Critical).
    /// </summary>
    public string Severity { get; set; } = string.Empty;

    /// <summary>
    /// Gets or sets the alert message.
    /// </summary>
    public string Message { get; set; } = string.Empty;

    /// <summary>
    /// Gets or sets the affected entity ID.
    /// </summary>
    public Guid? EntityId { get; set; }

    /// <summary>
    /// Gets or sets the affected entity type.
    /// </summary>
    public string? EntityType { get; set; }

    /// <summary>
    /// Gets or sets when the alert was created.
    /// </summary>
    public DateTime CreatedAt { get; set; }

    /// <summary>
    /// Gets or sets whether the alert has been acknowledged.
    /// </summary>
    public bool IsAcknowledged { get; set; }
}