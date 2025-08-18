using QueueManagement.SDK.Models.Common;

namespace QueueManagement.SDK.Models.Sessions;

/// <summary>
/// Represents a service session.
/// </summary>
public class SessionResponse
{
    /// <summary>
    /// Gets or sets the unique identifier of the session.
    /// </summary>
    public Guid Id { get; set; }

    /// <summary>
    /// Gets or sets the user/agent ID.
    /// </summary>
    public Guid UserId { get; set; }

    /// <summary>
    /// Gets or sets the user/agent name.
    /// </summary>
    public string UserName { get; set; } = string.Empty;

    /// <summary>
    /// Gets or sets the unit ID where the session is active.
    /// </summary>
    public Guid UnitId { get; set; }

    /// <summary>
    /// Gets or sets the unit name.
    /// </summary>
    public string? UnitName { get; set; }

    /// <summary>
    /// Gets or sets the counter/desk number.
    /// </summary>
    public string CounterNumber { get; set; } = string.Empty;

    /// <summary>
    /// Gets or sets the session status.
    /// </summary>
    public SessionStatus Status { get; set; }

    /// <summary>
    /// Gets or sets the list of queue IDs this session can serve.
    /// </summary>
    public List<Guid> QueueIds { get; set; } = new();

    /// <summary>
    /// Gets or sets the list of service IDs this session can provide.
    /// </summary>
    public List<Guid> ServiceIds { get; set; } = new();

    /// <summary>
    /// Gets or sets when the session started.
    /// </summary>
    public DateTime StartedAt { get; set; }

    /// <summary>
    /// Gets or sets when the session was completed.
    /// </summary>
    public DateTime? CompletedAt { get; set; }

    /// <summary>
    /// Gets or sets the total tickets served in this session.
    /// </summary>
    public int TicketsServed { get; set; }

    /// <summary>
    /// Gets or sets the average service time in minutes.
    /// </summary>
    public double AverageServiceTime { get; set; }

    /// <summary>
    /// Gets or sets the total service time in minutes.
    /// </summary>
    public double TotalServiceTime { get; set; }

    /// <summary>
    /// Gets or sets the total pause time in minutes.
    /// </summary>
    public double TotalPauseTime { get; set; }

    /// <summary>
    /// Gets or sets the current ticket being served.
    /// </summary>
    public Guid? CurrentTicketId { get; set; }

    /// <summary>
    /// Gets or sets the current ticket number.
    /// </summary>
    public string? CurrentTicketNumber { get; set; }

    /// <summary>
    /// Gets or sets when the session was last active.
    /// </summary>
    public DateTime LastActivityAt { get; set; }

    /// <summary>
    /// Gets or sets custom metadata for the session.
    /// </summary>
    public Dictionary<string, object>? Metadata { get; set; }

    /// <summary>
    /// Gets the session duration in minutes.
    /// </summary>
    public double SessionDurationMinutes => CompletedAt.HasValue
        ? (CompletedAt.Value - StartedAt).TotalMinutes
        : (DateTime.UtcNow - StartedAt).TotalMinutes;

    /// <summary>
    /// Gets the efficiency percentage (service time / total time).
    /// </summary>
    public double EfficiencyPercentage => SessionDurationMinutes > 0
        ? (TotalServiceTime / SessionDurationMinutes) * 100
        : 0;
}

/// <summary>
/// Request to start a new session.
/// </summary>
public class StartSessionRequest
{
    /// <summary>
    /// Gets or sets the unit ID where the session will be active.
    /// </summary>
    public Guid UnitId { get; set; }

    /// <summary>
    /// Gets or sets the counter/desk number.
    /// </summary>
    public string CounterNumber { get; set; } = string.Empty;

    /// <summary>
    /// Gets or sets the list of queue IDs this session will serve.
    /// </summary>
    public List<Guid> QueueIds { get; set; } = new();

    /// <summary>
    /// Gets or sets the list of service IDs this session will provide (optional).
    /// </summary>
    public List<Guid>? ServiceIds { get; set; }

    /// <summary>
    /// Gets or sets custom metadata (optional).
    /// </summary>
    public Dictionary<string, object>? Metadata { get; set; }

    /// <summary>
    /// Gets or sets whether to auto-call next ticket (optional).
    /// </summary>
    public bool AutoCallNext { get; set; } = false;
}

/// <summary>
/// Request to complete a session.
/// </summary>
public class CompleteSessionRequest
{
    /// <summary>
    /// Gets or sets the reason for completing the session (optional).
    /// </summary>
    public string? Reason { get; set; }

    /// <summary>
    /// Gets or sets any notes about the session (optional).
    /// </summary>
    public string? Notes { get; set; }

    /// <summary>
    /// Gets or sets whether to force complete even if there's an active ticket.
    /// </summary>
    public bool ForceComplete { get; set; } = false;
}

/// <summary>
/// Request to pause a session.
/// </summary>
public class PauseSessionRequest
{
    /// <summary>
    /// Gets or sets the reason for pausing (optional).
    /// </summary>
    public string? Reason { get; set; }

    /// <summary>
    /// Gets or sets the expected duration of the pause in minutes (optional).
    /// </summary>
    public int? ExpectedDurationMinutes { get; set; }
}

/// <summary>
/// Response containing session statistics.
/// </summary>
public class SessionStatistics
{
    /// <summary>
    /// Gets or sets the session ID.
    /// </summary>
    public Guid SessionId { get; set; }

    /// <summary>
    /// Gets or sets the total tickets served.
    /// </summary>
    public int TotalTicketsServed { get; set; }

    /// <summary>
    /// Gets or sets the tickets served by status.
    /// </summary>
    public Dictionary<TicketStatus, int> TicketsByStatus { get; set; } = new();

    /// <summary>
    /// Gets or sets the tickets served by priority.
    /// </summary>
    public Dictionary<Priority, int> TicketsByPriority { get; set; } = new();

    /// <summary>
    /// Gets or sets the average service time in minutes.
    /// </summary>
    public double AverageServiceTime { get; set; }

    /// <summary>
    /// Gets or sets the minimum service time in minutes.
    /// </summary>
    public double MinServiceTime { get; set; }

    /// <summary>
    /// Gets or sets the maximum service time in minutes.
    /// </summary>
    public double MaxServiceTime { get; set; }

    /// <summary>
    /// Gets or sets the total service time in minutes.
    /// </summary>
    public double TotalServiceTime { get; set; }

    /// <summary>
    /// Gets or sets the total pause time in minutes.
    /// </summary>
    public double TotalPauseTime { get; set; }

    /// <summary>
    /// Gets or sets the number of pauses.
    /// </summary>
    public int PauseCount { get; set; }

    /// <summary>
    /// Gets or sets the efficiency percentage.
    /// </summary>
    public double EfficiencyPercentage { get; set; }

    /// <summary>
    /// Gets or sets the customer satisfaction score if available.
    /// </summary>
    public double? CustomerSatisfactionScore { get; set; }

    /// <summary>
    /// Gets or sets when these statistics were calculated.
    /// </summary>
    public DateTime CalculatedAt { get; set; }
}