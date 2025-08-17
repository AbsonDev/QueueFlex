namespace QueueManagement.Api.DTOs.Dashboard;

/// <summary>
/// Dashboard overview DTO
/// </summary>
public class DashboardOverviewDto
{
    /// <summary>
    /// Total number of units
    /// </summary>
    public int TotalUnits { get; set; }

    /// <summary>
    /// Total number of active queues
    /// </summary>
    public int ActiveQueues { get; set; }

    /// <summary>
    /// Total number of waiting tickets
    /// </summary>
    public int WaitingTickets { get; set; }

    /// <summary>
    /// Total number of active sessions
    /// </summary>
    public int ActiveSessions { get; set; }

    /// <summary>
    /// Total number of users
    /// </summary>
    public int TotalUsers { get; set; }

    /// <summary>
    /// Number of available users
    /// </summary>
    public int AvailableUsers { get; set; }

    /// <summary>
    /// Average waiting time in minutes
    /// </summary>
    public double AverageWaitingTimeMinutes { get; set; }

    /// <summary>
    /// Average service time in minutes
    /// </summary>
    public double AverageServiceTimeMinutes { get; set; }

    /// <summary>
    /// Today's completed tickets
    /// </summary>
    public int TodayCompletedTickets { get; set; }

    /// <summary>
    /// Today's cancelled tickets
    /// </summary>
    public int TodayCancelledTickets { get; set; }

    /// <summary>
    /// Customer satisfaction rating (1-5)
    /// </summary>
    public double CustomerSatisfactionRating { get; set; }
}

/// <summary>
/// Unit dashboard DTO
/// </summary>
public class UnitDashboardDto
{
    /// <summary>
    /// Unit ID
    /// </summary>
    public Guid UnitId { get; set; }

    /// <summary>
    /// Unit name
    /// </summary>
    public string UnitName { get; set; } = string.Empty;

    /// <summary>
    /// Whether the unit is currently open
    /// </summary>
    public bool IsOpen { get; set; }

    /// <summary>
    /// Number of active queues
    /// </summary>
    public int ActiveQueues { get; set; }

    /// <summary>
    /// Number of waiting tickets
    /// </summary>
    public int WaitingTickets { get; set; }

    /// <summary>
    /// Number of active sessions
    /// </summary>
    public int ActiveSessions { get; set; }

    /// <summary>
    /// Number of available users
    /// </summary>
    public int AvailableUsers { get; set; }

    /// <summary>
    /// Average waiting time in minutes
    /// </summary>
    public double AverageWaitingTimeMinutes { get; set; }

    /// <summary>
    /// Queue statuses
    /// </summary>
    public List<QueueStatusSummaryDto> QueueStatuses { get; set; } = new();

    /// <summary>
    /// User statuses
    /// </summary>
    public List<UserStatusSummaryDto> UserStatuses { get; set; } = new();
}

/// <summary>
/// Queue status summary DTO
/// </summary>
public class QueueStatusSummaryDto
{
    /// <summary>
    /// Queue ID
    /// </summary>
    public Guid QueueId { get; set; }

    /// <summary>
    /// Queue name
    /// </summary>
    public string QueueName { get; set; } = string.Empty;

    /// <summary>
    /// Current status
    /// </summary>
    public string Status { get; set; } = string.Empty;

    /// <summary>
    /// Number of waiting tickets
    /// </summary>
    public int WaitingTickets { get; set; }

    /// <summary>
    /// Estimated waiting time in minutes
    /// </summary>
    public int? EstimatedWaitingMinutes { get; set; }

    /// <summary>
    /// Whether the queue is accepting tickets
    /// </summary>
    public bool IsAcceptingTickets { get; set; }
}

/// <summary>
/// User status summary DTO
/// </summary>
public class UserStatusSummaryDto
{
    /// <summary>
    /// User ID
    /// </summary>
    public Guid UserId { get; set; }

    /// <summary>
    /// User name
    /// </summary>
    public string UserName { get; set; } = string.Empty;

    /// <summary>
    /// Current status
    /// </summary>
    public string Status { get; set; } = string.Empty;

    /// <summary>
    /// Whether the user is available
    /// </summary>
    public bool IsAvailable { get; set; }

    /// <summary>
    /// Number of active sessions
    /// </summary>
    public int ActiveSessions { get; set; }

    /// <summary>
    /// Current session duration if active
    /// </summary>
    public TimeSpan? CurrentSessionDuration { get; set; }
}

/// <summary>
/// Dashboard metrics DTO
/// </summary>
public class DashboardMetricsDto
{
    /// <summary>
    /// Time period for the metrics
    /// </summary>
    public string Period { get; set; } = string.Empty;

    /// <summary>
    /// Start date
    /// </summary>
    public DateTime StartDate { get; set; }

    /// <summary>
    /// End date
    /// </summary>
    public DateTime EndDate { get; set; }

    /// <summary>
    /// Total tickets processed
    /// </summary>
    public int TotalTickets { get; set; }

    /// <summary>
    /// Completed tickets
    /// </summary>
    public int CompletedTickets { get; set; }

    /// <summary>
    /// Cancelled tickets
    /// </summary>
    public int CancelledTickets { get; set; }

    /// <summary>
    /// No-show tickets
    /// </summary>
    public int NoShowTickets { get; set; }

    /// <summary>
    /// Average waiting time in minutes
    /// </summary>
    public double AverageWaitingTimeMinutes { get; set; }

    /// <summary>
    /// Average service time in minutes
    /// </summary>
    public double AverageServiceTimeMinutes { get; set; }

    /// <summary>
    /// Customer satisfaction rating (1-5)
    /// </summary>
    public double CustomerSatisfactionRating { get; set; }

    /// <summary>
    /// Peak hours (hour of day with most tickets)
    /// </summary>
    public List<int> PeakHours { get; set; } = new();

    /// <summary>
    /// Daily ticket counts
    /// </summary>
    public List<DailyTicketCountDto> DailyTicketCounts { get; set; } = new();

    /// <summary>
    /// Queue performance metrics
    /// </summary>
    public List<QueuePerformanceDto> QueuePerformance { get; set; } = new();
}

/// <summary>
/// Daily ticket count DTO
/// </summary>
public class DailyTicketCountDto
{
    /// <summary>
    /// Date
    /// </summary>
    public DateTime Date { get; set; }

    /// <summary>
    /// Total tickets
    /// </summary>
    public int TotalTickets { get; set; }

    /// <summary>
    /// Completed tickets
    /// </summary>
    public int CompletedTickets { get; set; }

    /// <summary>
    /// Cancelled tickets
    /// </summary>
    public int CancelledTickets { get; set; }
}

/// <summary>
/// Queue performance DTO
/// </summary>
public class QueuePerformanceDto
{
    /// <summary>
    /// Queue ID
    /// </summary>
    public Guid QueueId { get; set; }

    /// <summary>
    /// Queue name
    /// </summary>
    public string QueueName { get; set; } = string.Empty;

    /// <summary>
    /// Total tickets processed
    /// </summary>
    public int TotalTickets { get; set; }

    /// <summary>
    /// Average waiting time in minutes
    /// </summary>
    public double AverageWaitingTimeMinutes { get; set; }

    /// <summary>
    /// Average service time in minutes
    /// </summary>
    public double AverageServiceTimeMinutes { get; set; }

    /// <summary>
    /// Customer satisfaction rating (1-5)
    /// </summary>
    public double CustomerSatisfactionRating { get; set; }
}