using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.SignalR;
using QueueManagement.Api.DTOs.Queues;
using QueueManagement.Api.DTOs.Tickets;

namespace QueueManagement.Api.Hubs;

/// <summary>
/// SignalR hub for real-time queue management
/// </summary>
[Authorize]
public class QueueHub : Hub
{
    private readonly ILogger<QueueHub> _logger;

    public QueueHub(ILogger<QueueHub> logger)
    {
        _logger = logger;
    }

    /// <summary>
    /// Join a specific queue to receive real-time updates
    /// </summary>
    /// <param name="queueId">Queue ID to join</param>
    public async Task JoinQueue(string queueId)
    {
        try
        {
            var tenantId = GetTenantId();
            var userId = GetUserId();

            _logger.LogInformation("User {UserId} joining queue {QueueId} in tenant {TenantId}", userId, queueId, tenantId);

            await Groups.AddToGroupAsync(Context.ConnectionId, $"queue_{queueId}");
            await Groups.AddToGroupAsync(Context.ConnectionId, $"tenant_{tenantId}");

            // Notify other users in the queue
            await Clients.Group($"queue_{queueId}").SendAsync("UserJoinedQueue", new
            {
                UserId = userId,
                QueueId = queueId,
                Timestamp = DateTime.UtcNow
            });

            _logger.LogInformation("User {UserId} successfully joined queue {QueueId}", userId, queueId);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error joining queue {QueueId}", queueId);
            throw;
        }
    }

    /// <summary>
    /// Leave a specific queue
    /// </summary>
    /// <param name="queueId">Queue ID to leave</param>
    public async Task LeaveQueue(string queueId)
    {
        try
        {
            var tenantId = GetTenantId();
            var userId = GetUserId();

            _logger.LogInformation("User {UserId} leaving queue {QueueId} in tenant {TenantId}", userId, queueId, tenantId);

            await Groups.RemoveFromGroupAsync(Context.ConnectionId, $"queue_{queueId}");

            // Notify other users in the queue
            await Clients.Group($"queue_{queueId}").SendAsync("UserLeftQueue", new
            {
                UserId = userId,
                QueueId = queueId,
                Timestamp = DateTime.UtcNow
            });

            _logger.LogInformation("User {UserId} successfully left queue {QueueId}", userId, queueId);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error leaving queue {QueueId}", queueId);
            throw;
        }
    }

    /// <summary>
    /// Join a specific unit to receive real-time updates
    /// </summary>
    /// <param name="unitId">Unit ID to join</param>
    public async Task JoinUnit(string unitId)
    {
        try
        {
            var tenantId = GetTenantId();
            var userId = GetUserId();

            _logger.LogInformation("User {UserId} joining unit {UnitId} in tenant {TenantId}", userId, unitId, tenantId);

            await Groups.AddToGroupAsync(Context.ConnectionId, $"unit_{unitId}");
            await Groups.AddToGroupAsync(Context.ConnectionId, $"tenant_{tenantId}");

            // Notify other users in the unit
            await Clients.Group($"unit_{unitId}").SendAsync("UserJoinedUnit", new
            {
                UserId = userId,
                UnitId = unitId,
                Timestamp = DateTime.UtcNow
            });

            _logger.LogInformation("User {UserId} successfully joined unit {UnitId}", userId, unitId);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error joining unit {UnitId}", unitId);
            throw;
        }
    }

    /// <summary>
    /// Join tenant-wide notifications
    /// </summary>
    public async Task JoinTenant()
    {
        try
        {
            var tenantId = GetTenantId();
            var userId = GetUserId();

            _logger.LogInformation("User {UserId} joining tenant {TenantId}", userId, tenantId);

            await Groups.AddToGroupAsync(Context.ConnectionId, $"tenant_{tenantId}");

            _logger.LogInformation("User {UserId} successfully joined tenant {TenantId}", userId, tenantId);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error joining tenant");
            throw;
        }
    }

    /// <summary>
    /// Get real-time queue status
    /// </summary>
    /// <param name="queueId">Queue ID</param>
    /// <returns>Current queue status</returns>
    public async Task<QueueStatusDto> GetQueueStatus(string queueId)
    {
        try
        {
            var tenantId = GetTenantId();
            _logger.LogInformation("Getting real-time status for queue {QueueId} in tenant {TenantId}", queueId, tenantId);

            // TODO: Implement real-time queue status query
            // This would typically query the database or cache for current status

            // For now, return mock data
            var mockStatus = new QueueStatusDto
            {
                Id = Guid.Parse(queueId),
                Name = "Customer Service",
                Status = "Open",
                IsActive = true,
                CurrentTicketCount = 15,
                MaxCapacity = 100,
                IsAtCapacity = false,
                IsAcceptingTickets = true,
                LastUpdated = DateTime.UtcNow
            };

            return mockStatus;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error getting queue status for {QueueId}", queueId);
            throw;
        }
    }

    /// <summary>
    /// Notify queue status change to all connected clients
    /// </summary>
    /// <param name="queueId">Queue ID</param>
    /// <param name="status">New status</param>
    public async Task NotifyQueueStatusChange(string queueId, QueueStatusDto status)
    {
        try
        {
            _logger.LogInformation("Notifying queue status change for {QueueId}", queueId);

            await Clients.Group($"queue_{queueId}").SendAsync("QueueStatusChanged", status);
            await Clients.Group($"tenant_{status.Id}").SendAsync("QueueStatusChanged", new
            {
                QueueId = queueId,
                Status = status
            });

            _logger.LogInformation("Queue status change notification sent for {QueueId}", queueId);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error notifying queue status change for {QueueId}", queueId);
            throw;
        }
    }

    /// <summary>
    /// Notify ticket called to all connected clients
    /// </summary>
    /// <param name="queueId">Queue ID</param>
    /// <param name="ticket">Called ticket information</param>
    public async Task NotifyTicketCalled(string queueId, TicketStatusDto ticket)
    {
        try
        {
            _logger.LogInformation("Notifying ticket called for queue {QueueId}, ticket {TicketNumber}", queueId, ticket.Number);

            await Clients.Group($"queue_{queueId}").SendAsync("TicketCalled", ticket);
            await Clients.Group($"tenant_{queueId}").SendAsync("TicketCalled", new
            {
                QueueId = queueId,
                Ticket = ticket
            });

            _logger.LogInformation("Ticket called notification sent for queue {QueueId}, ticket {TicketNumber}", queueId, ticket.Number);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error notifying ticket called for queue {QueueId}", queueId);
            throw;
        }
    }

    /// <summary>
    /// Notify new ticket created to all connected clients
    /// </summary>
    /// <param name="queueId">Queue ID</param>
    /// <param name="ticket">New ticket information</param>
    public async Task NotifyTicketCreated(string queueId, TicketStatusDto ticket)
    {
        try
        {
            _logger.LogInformation("Notifying new ticket created for queue {QueueId}, ticket {TicketNumber}", queueId, ticket.Number);

            await Clients.Group($"queue_{queueId}").SendAsync("TicketCreated", ticket);
            await Clients.Group($"tenant_{queueId}").SendAsync("TicketCreated", new
            {
                QueueId = queueId,
                Ticket = ticket
            });

            _logger.LogInformation("New ticket notification sent for queue {QueueId}, ticket {TicketNumber}", queueId, ticket.Number);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error notifying new ticket for queue {QueueId}", queueId);
            throw;
        }
    }

    /// <summary>
    /// Notify ticket status change to all connected clients
    /// </summary>
    /// <param name="queueId">Queue ID</param>
    /// <param name="ticketId">Ticket ID</param>
    /// <param name="newStatus">New status</param>
    public async Task NotifyTicketStatusChange(string queueId, string ticketId, string newStatus)
    {
        try
        {
            _logger.LogInformation("Notifying ticket status change for queue {QueueId}, ticket {TicketId} to {NewStatus}", queueId, ticketId, newStatus);

            await Clients.Group($"queue_{queueId}").SendAsync("TicketStatusChanged", new
            {
                TicketId = ticketId,
                NewStatus = newStatus,
                Timestamp = DateTime.UtcNow
            });

            _logger.LogInformation("Ticket status change notification sent for queue {QueueId}, ticket {TicketId}", queueId, ticketId);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error notifying ticket status change for queue {QueueId}", queueId);
            throw;
        }
    }

    /// <summary>
    /// Get tenant ID from connection claims
    /// </summary>
    private string GetTenantId()
    {
        var tenantIdClaim = Context.User?.FindFirst("tenant_id")?.Value;
        if (string.IsNullOrEmpty(tenantIdClaim))
        {
            throw new UnauthorizedAccessException("Tenant ID not found in claims");
        }
        return tenantIdClaim;
    }

    /// <summary>
    /// Get user ID from connection claims
    /// </summary>
    private string GetUserId()
    {
        var userIdClaim = Context.User?.FindFirst("user_id")?.Value;
        if (string.IsNullOrEmpty(userIdClaim))
        {
            throw new UnauthorizedAccessException("User ID not found in claims");
        }
        return userIdClaim;
    }

    /// <summary>
    /// Called when a client connects
    /// </summary>
    public override async Task OnConnectedAsync()
    {
        try
        {
            var tenantId = GetTenantId();
            var userId = GetUserId();

            _logger.LogInformation("Client connected: User {UserId} in tenant {TenantId}", userId, tenantId);

            // Add to tenant group by default
            await Groups.AddToGroupAsync(Context.ConnectionId, $"tenant_{tenantId}");

            await base.OnConnectedAsync();
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error in OnConnectedAsync");
            throw;
        }
    }

    /// <summary>
    /// Called when a client disconnects
    /// </summary>
    public override async Task OnDisconnectedAsync(Exception? exception)
    {
        try
        {
            var tenantId = GetTenantId();
            var userId = GetUserId();

            _logger.LogInformation("Client disconnected: User {UserId} in tenant {TenantId}", userId, tenantId);

            await base.OnDisconnectedAsync(exception);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error in OnDisconnectedAsync");
        }
    }
}