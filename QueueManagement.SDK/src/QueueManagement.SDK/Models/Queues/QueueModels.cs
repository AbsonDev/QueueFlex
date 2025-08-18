using QueueManagement.SDK.Models.Common;

namespace QueueManagement.SDK.Models.Queues;

/// <summary>
/// Represents a queue in the system.
/// </summary>
public class QueueResponse
{
    /// <summary>
    /// Gets or sets the unique identifier of the queue.
    /// </summary>
    public Guid Id { get; set; }

    /// <summary>
    /// Gets or sets the queue name.
    /// </summary>
    public string Name { get; set; } = string.Empty;

    /// <summary>
    /// Gets or sets the queue description.
    /// </summary>
    public string? Description { get; set; }

    /// <summary>
    /// Gets or sets the queue code/prefix for ticket numbers.
    /// </summary>
    public string Code { get; set; } = string.Empty;

    /// <summary>
    /// Gets or sets the unit ID this queue belongs to.
    /// </summary>
    public Guid UnitId { get; set; }

    /// <summary>
    /// Gets or sets the current status of the queue.
    /// </summary>
    public QueueStatus Status { get; set; }

    /// <summary>
    /// Gets or sets the maximum capacity of the queue.
    /// </summary>
    public int MaxCapacity { get; set; }

    /// <summary>
    /// Gets or sets the current number of waiting tickets.
    /// </summary>
    public int CurrentSize { get; set; }

    /// <summary>
    /// Gets or sets the average wait time in minutes.
    /// </summary>
    public double AverageWaitTime { get; set; }

    /// <summary>
    /// Gets or sets the average service time in minutes.
    /// </summary>
    public double AverageServiceTime { get; set; }

    /// <summary>
    /// Gets or sets the number of active agents serving this queue.
    /// </summary>
    public int ActiveAgents { get; set; }

    /// <summary>
    /// Gets or sets the list of service IDs available in this queue.
    /// </summary>
    public List<Guid> ServiceIds { get; set; } = new();

    /// <summary>
    /// Gets or sets whether the queue accepts priority tickets.
    /// </summary>
    public bool AcceptsPriority { get; set; }

    /// <summary>
    /// Gets or sets whether the queue is currently accepting new tickets.
    /// </summary>
    public bool IsAcceptingTickets { get; set; }

    /// <summary>
    /// Gets or sets the display order for UI purposes.
    /// </summary>
    public int DisplayOrder { get; set; }

    /// <summary>
    /// Gets or sets custom metadata for the queue.
    /// </summary>
    public Dictionary<string, object>? Metadata { get; set; }

    /// <summary>
    /// Gets or sets when the queue was created.
    /// </summary>
    public DateTime CreatedAt { get; set; }

    /// <summary>
    /// Gets or sets when the queue was last updated.
    /// </summary>
    public DateTime UpdatedAt { get; set; }

    /// <summary>
    /// Gets or sets the unit name for display purposes.
    /// </summary>
    public string? UnitName { get; set; }

    /// <summary>
    /// Gets whether the queue is at capacity.
    /// </summary>
    public bool IsAtCapacity => CurrentSize >= MaxCapacity;

    /// <summary>
    /// Gets the percentage of capacity used.
    /// </summary>
    public double CapacityPercentage => MaxCapacity > 0 ? (CurrentSize * 100.0) / MaxCapacity : 0;
}

/// <summary>
/// Request to create a new queue.
/// </summary>
public class CreateQueueRequest
{
    /// <summary>
    /// Gets or sets the queue name.
    /// </summary>
    public string Name { get; set; } = string.Empty;

    /// <summary>
    /// Gets or sets the queue description (optional).
    /// </summary>
    public string? Description { get; set; }

    /// <summary>
    /// Gets or sets the queue code/prefix for ticket numbers.
    /// </summary>
    public string Code { get; set; } = string.Empty;

    /// <summary>
    /// Gets or sets the unit ID this queue belongs to.
    /// </summary>
    public Guid UnitId { get; set; }

    /// <summary>
    /// Gets or sets the maximum capacity (default: 100).
    /// </summary>
    public int MaxCapacity { get; set; } = 100;

    /// <summary>
    /// Gets or sets the list of service IDs available in this queue.
    /// </summary>
    public List<Guid> ServiceIds { get; set; } = new();

    /// <summary>
    /// Gets or sets whether the queue accepts priority tickets (default: true).
    /// </summary>
    public bool AcceptsPriority { get; set; } = true;

    /// <summary>
    /// Gets or sets the display order (optional).
    /// </summary>
    public int DisplayOrder { get; set; }

    /// <summary>
    /// Gets or sets custom metadata (optional).
    /// </summary>
    public Dictionary<string, object>? Metadata { get; set; }
}

/// <summary>
/// Request to update an existing queue.
/// </summary>
public class UpdateQueueRequest
{
    /// <summary>
    /// Gets or sets the updated name (optional).
    /// </summary>
    public string? Name { get; set; }

    /// <summary>
    /// Gets or sets the updated description (optional).
    /// </summary>
    public string? Description { get; set; }

    /// <summary>
    /// Gets or sets the updated maximum capacity (optional).
    /// </summary>
    public int? MaxCapacity { get; set; }

    /// <summary>
    /// Gets or sets the updated service IDs (optional).
    /// </summary>
    public List<Guid>? ServiceIds { get; set; }

    /// <summary>
    /// Gets or sets whether to accept priority tickets (optional).
    /// </summary>
    public bool? AcceptsPriority { get; set; }

    /// <summary>
    /// Gets or sets the updated display order (optional).
    /// </summary>
    public int? DisplayOrder { get; set; }

    /// <summary>
    /// Gets or sets updated metadata (optional).
    /// </summary>
    public Dictionary<string, object>? Metadata { get; set; }
}

/// <summary>
/// Response containing queue status information.
/// </summary>
public class QueueStatusResponse
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
    /// Gets or sets the current status.
    /// </summary>
    public QueueStatus Status { get; set; }

    /// <summary>
    /// Gets or sets the number of waiting tickets.
    /// </summary>
    public int WaitingTickets { get; set; }

    /// <summary>
    /// Gets or sets the number of tickets being served.
    /// </summary>
    public int ServingTickets { get; set; }

    /// <summary>
    /// Gets or sets the number of completed tickets today.
    /// </summary>
    public int CompletedToday { get; set; }

    /// <summary>
    /// Gets or sets the average wait time in minutes.
    /// </summary>
    public double AverageWaitTime { get; set; }

    /// <summary>
    /// Gets or sets the average service time in minutes.
    /// </summary>
    public double AverageServiceTime { get; set; }

    /// <summary>
    /// Gets or sets the number of active agents.
    /// </summary>
    public int ActiveAgents { get; set; }

    /// <summary>
    /// Gets or sets the capacity percentage.
    /// </summary>
    public double CapacityPercentage { get; set; }

    /// <summary>
    /// Gets or sets when this status was calculated.
    /// </summary>
    public DateTime CalculatedAt { get; set; }
}

/// <summary>
/// Response containing queue metrics.
/// </summary>
public class QueueMetricsResponse
{
    /// <summary>
    /// Gets or sets the queue ID.
    /// </summary>
    public Guid QueueId { get; set; }

    /// <summary>
    /// Gets or sets the time period for these metrics.
    /// </summary>
    public string Period { get; set; } = string.Empty;

    /// <summary>
    /// Gets or sets the total tickets created.
    /// </summary>
    public int TotalTickets { get; set; }

    /// <summary>
    /// Gets or sets the total tickets served.
    /// </summary>
    public int ServedTickets { get; set; }

    /// <summary>
    /// Gets or sets the total cancelled tickets.
    /// </summary>
    public int CancelledTickets { get; set; }

    /// <summary>
    /// Gets or sets the total no-show tickets.
    /// </summary>
    public int NoShowTickets { get; set; }

    /// <summary>
    /// Gets or sets the average wait time in minutes.
    /// </summary>
    public double AverageWaitTime { get; set; }

    /// <summary>
    /// Gets or sets the maximum wait time in minutes.
    /// </summary>
    public double MaxWaitTime { get; set; }

    /// <summary>
    /// Gets or sets the minimum wait time in minutes.
    /// </summary>
    public double MinWaitTime { get; set; }

    /// <summary>
    /// Gets or sets the average service time in minutes.
    /// </summary>
    public double AverageServiceTime { get; set; }

    /// <summary>
    /// Gets or sets the service level percentage (tickets served within target time).
    /// </summary>
    public double ServiceLevelPercentage { get; set; }

    /// <summary>
    /// Gets or sets the customer satisfaction score (if available).
    /// </summary>
    public double? CustomerSatisfactionScore { get; set; }

    /// <summary>
    /// Gets or sets hourly breakdown of metrics.
    /// </summary>
    public List<HourlyMetric>? HourlyBreakdown { get; set; }

    /// <summary>
    /// Gets or sets when these metrics were calculated.
    /// </summary>
    public DateTime CalculatedAt { get; set; }
}

/// <summary>
/// Represents metrics for a specific hour.
/// </summary>
public class HourlyMetric
{
    /// <summary>
    /// Gets or sets the hour (0-23).
    /// </summary>
    public int Hour { get; set; }

    /// <summary>
    /// Gets or sets the number of tickets created.
    /// </summary>
    public int TicketsCreated { get; set; }

    /// <summary>
    /// Gets or sets the number of tickets served.
    /// </summary>
    public int TicketsServed { get; set; }

    /// <summary>
    /// Gets or sets the average wait time in minutes.
    /// </summary>
    public double AverageWaitTime { get; set; }

    /// <summary>
    /// Gets or sets the average service time in minutes.
    /// </summary>
    public double AverageServiceTime { get; set; }
}