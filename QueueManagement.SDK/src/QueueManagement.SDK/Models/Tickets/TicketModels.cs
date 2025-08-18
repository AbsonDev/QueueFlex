using QueueManagement.SDK.Models.Common;

namespace QueueManagement.SDK.Models.Tickets;

/// <summary>
/// Represents a ticket in the queue system.
/// </summary>
public class TicketResponse
{
    /// <summary>
    /// Gets or sets the unique identifier of the ticket.
    /// </summary>
    public Guid Id { get; set; }

    /// <summary>
    /// Gets or sets the ticket number displayed to customers.
    /// </summary>
    public string Number { get; set; } = string.Empty;

    /// <summary>
    /// Gets or sets the queue ID this ticket belongs to.
    /// </summary>
    public Guid QueueId { get; set; }

    /// <summary>
    /// Gets or sets the service ID requested.
    /// </summary>
    public Guid ServiceId { get; set; }

    /// <summary>
    /// Gets or sets the unit ID where the ticket was created.
    /// </summary>
    public Guid UnitId { get; set; }

    /// <summary>
    /// Gets or sets the current status of the ticket.
    /// </summary>
    public TicketStatus Status { get; set; }

    /// <summary>
    /// Gets or sets the priority level of the ticket.
    /// </summary>
    public Priority Priority { get; set; }

    /// <summary>
    /// Gets or sets the customer's name.
    /// </summary>
    public string? CustomerName { get; set; }

    /// <summary>
    /// Gets or sets the customer's document/ID number.
    /// </summary>
    public string? CustomerDocument { get; set; }

    /// <summary>
    /// Gets or sets the customer's phone number.
    /// </summary>
    public string? CustomerPhone { get; set; }

    /// <summary>
    /// Gets or sets the customer's email address.
    /// </summary>
    public string? CustomerEmail { get; set; }

    /// <summary>
    /// Gets or sets the current position in the queue.
    /// </summary>
    public int Position { get; set; }

    /// <summary>
    /// Gets or sets the estimated wait time in minutes.
    /// </summary>
    public int EstimatedWaitMinutes { get; set; }

    /// <summary>
    /// Gets or sets the estimated service time.
    /// </summary>
    public DateTime? EstimatedServiceTime { get; set; }

    /// <summary>
    /// Gets or sets when the ticket was created.
    /// </summary>
    public DateTime CreatedAt { get; set; }

    /// <summary>
    /// Gets or sets when the ticket was called.
    /// </summary>
    public DateTime? CalledAt { get; set; }

    /// <summary>
    /// Gets or sets when service started.
    /// </summary>
    public DateTime? ServiceStartedAt { get; set; }

    /// <summary>
    /// Gets or sets when service was completed.
    /// </summary>
    public DateTime? CompletedAt { get; set; }

    /// <summary>
    /// Gets or sets the ID of the agent serving this ticket.
    /// </summary>
    public Guid? ServingAgentId { get; set; }

    /// <summary>
    /// Gets or sets the name of the serving agent.
    /// </summary>
    public string? ServingAgentName { get; set; }

    /// <summary>
    /// Gets or sets the counter/desk number where the customer should go.
    /// </summary>
    public string? CounterNumber { get; set; }

    /// <summary>
    /// Gets or sets any notes associated with the ticket.
    /// </summary>
    public string? Notes { get; set; }

    /// <summary>
    /// Gets or sets custom metadata for the ticket.
    /// </summary>
    public Dictionary<string, object>? Metadata { get; set; }

    /// <summary>
    /// Gets or sets the queue name for display purposes.
    /// </summary>
    public string? QueueName { get; set; }

    /// <summary>
    /// Gets or sets the service name for display purposes.
    /// </summary>
    public string? ServiceName { get; set; }

    /// <summary>
    /// Gets the actual wait time in minutes (null if not yet served).
    /// </summary>
    public int? ActualWaitMinutes => ServiceStartedAt.HasValue && CreatedAt != default
        ? (int)(ServiceStartedAt.Value - CreatedAt).TotalMinutes
        : null;

    /// <summary>
    /// Gets the service duration in minutes (null if not completed).
    /// </summary>
    public int? ServiceDurationMinutes => CompletedAt.HasValue && ServiceStartedAt.HasValue
        ? (int)(CompletedAt.Value - ServiceStartedAt.Value).TotalMinutes
        : null;
}

/// <summary>
/// Request to create a new ticket.
/// </summary>
public class CreateTicketRequest
{
    /// <summary>
    /// Gets or sets the queue ID to add the ticket to.
    /// </summary>
    public Guid QueueId { get; set; }

    /// <summary>
    /// Gets or sets the service ID requested.
    /// </summary>
    public Guid ServiceId { get; set; }

    /// <summary>
    /// Gets or sets the priority level (optional, defaults to Normal).
    /// </summary>
    public Priority Priority { get; set; } = Priority.Normal;

    /// <summary>
    /// Gets or sets the customer's name (optional).
    /// </summary>
    public string? CustomerName { get; set; }

    /// <summary>
    /// Gets or sets the customer's document/ID number (optional).
    /// </summary>
    public string? CustomerDocument { get; set; }

    /// <summary>
    /// Gets or sets the customer's phone number (optional).
    /// </summary>
    public string? CustomerPhone { get; set; }

    /// <summary>
    /// Gets or sets the customer's email address (optional).
    /// </summary>
    public string? CustomerEmail { get; set; }

    /// <summary>
    /// Gets or sets any notes for the ticket (optional).
    /// </summary>
    public string? Notes { get; set; }

    /// <summary>
    /// Gets or sets custom metadata (optional).
    /// </summary>
    public Dictionary<string, object>? Metadata { get; set; }

    /// <summary>
    /// Gets or sets whether to send SMS notification (optional).
    /// </summary>
    public bool SendSmsNotification { get; set; }

    /// <summary>
    /// Gets or sets whether to send email notification (optional).
    /// </summary>
    public bool SendEmailNotification { get; set; }

    /// <summary>
    /// Gets or sets the preferred language for notifications (optional).
    /// </summary>
    public string? PreferredLanguage { get; set; }
}

/// <summary>
/// Request to update an existing ticket.
/// </summary>
public class UpdateTicketRequest
{
    /// <summary>
    /// Gets or sets the updated priority (optional).
    /// </summary>
    public Priority? Priority { get; set; }

    /// <summary>
    /// Gets or sets the updated customer name (optional).
    /// </summary>
    public string? CustomerName { get; set; }

    /// <summary>
    /// Gets or sets the updated customer phone (optional).
    /// </summary>
    public string? CustomerPhone { get; set; }

    /// <summary>
    /// Gets or sets the updated customer email (optional).
    /// </summary>
    public string? CustomerEmail { get; set; }

    /// <summary>
    /// Gets or sets updated notes (optional).
    /// </summary>
    public string? Notes { get; set; }

    /// <summary>
    /// Gets or sets updated metadata (optional).
    /// </summary>
    public Dictionary<string, object>? Metadata { get; set; }
}

/// <summary>
/// Request to transfer a ticket to another queue.
/// </summary>
public class TransferTicketRequest
{
    /// <summary>
    /// Gets or sets the target queue ID.
    /// </summary>
    public Guid TargetQueueId { get; set; }

    /// <summary>
    /// Gets or sets the new service ID (optional, keeps current if not specified).
    /// </summary>
    public Guid? ServiceId { get; set; }

    /// <summary>
    /// Gets or sets the reason for transfer.
    /// </summary>
    public string? Reason { get; set; }

    /// <summary>
    /// Gets or sets whether to maintain priority in the new queue.
    /// </summary>
    public bool MaintainPriority { get; set; } = true;

    /// <summary>
    /// Gets or sets whether to notify the customer of the transfer.
    /// </summary>
    public bool NotifyCustomer { get; set; } = true;
}

/// <summary>
/// Response containing ticket status information.
/// </summary>
public class TicketStatusResponse
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
    /// Gets or sets the current status.
    /// </summary>
    public TicketStatus Status { get; set; }

    /// <summary>
    /// Gets or sets the position in queue.
    /// </summary>
    public int Position { get; set; }

    /// <summary>
    /// Gets or sets the estimated wait time in minutes.
    /// </summary>
    public int EstimatedWaitMinutes { get; set; }

    /// <summary>
    /// Gets or sets the estimated service time.
    /// </summary>
    public DateTime? EstimatedServiceTime { get; set; }

    /// <summary>
    /// Gets or sets the counter number if called.
    /// </summary>
    public string? CounterNumber { get; set; }

    /// <summary>
    /// Gets or sets the serving agent name if applicable.
    /// </summary>
    public string? ServingAgentName { get; set; }

    /// <summary>
    /// Gets or sets when the status was last updated.
    /// </summary>
    public DateTime LastUpdated { get; set; }
}

/// <summary>
/// Response containing queue position information.
/// </summary>
public class QueuePositionResponse
{
    /// <summary>
    /// Gets or sets the ticket ID.
    /// </summary>
    public Guid TicketId { get; set; }

    /// <summary>
    /// Gets or sets the current position.
    /// </summary>
    public int CurrentPosition { get; set; }

    /// <summary>
    /// Gets or sets the total tickets ahead.
    /// </summary>
    public int TicketsAhead { get; set; }

    /// <summary>
    /// Gets or sets the estimated wait time in minutes.
    /// </summary>
    public int EstimatedWaitMinutes { get; set; }

    /// <summary>
    /// Gets or sets the average service time in minutes.
    /// </summary>
    public double AverageServiceTimeMinutes { get; set; }

    /// <summary>
    /// Gets or sets the number of active agents.
    /// </summary>
    public int ActiveAgents { get; set; }

    /// <summary>
    /// Gets or sets when this information was calculated.
    /// </summary>
    public DateTime CalculatedAt { get; set; }
}