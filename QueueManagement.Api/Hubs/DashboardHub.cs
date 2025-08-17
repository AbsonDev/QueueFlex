using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.SignalR;
using QueueManagement.Api.DTOs.Dashboard;

namespace QueueManagement.Api.Hubs;

/// <summary>
/// SignalR hub for real-time dashboard metrics
/// </summary>
[Authorize]
public class DashboardHub : Hub
{
    private readonly ILogger<DashboardHub> _logger;

    public DashboardHub(ILogger<DashboardHub> logger)
    {
        _logger = logger;
    }

    /// <summary>
    /// Join dashboard to receive real-time metrics
    /// </summary>
    public async Task JoinDashboard()
    {
        try
        {
            var tenantId = GetTenantId();
            var userId = GetUserId();

            _logger.LogInformation("User {UserId} joining dashboard in tenant {TenantId}", userId, tenantId);

            await Groups.AddToGroupAsync(Context.ConnectionId, $"dashboard_{tenantId}");
            await Groups.AddToGroupAsync(Context.ConnectionId, $"tenant_{tenantId}");

            // Send initial metrics
            var initialMetrics = await GetInitialMetrics(tenantId);
            await Clients.Caller.SendAsync("DashboardMetrics", initialMetrics);

            _logger.LogInformation("User {UserId} successfully joined dashboard", userId);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error joining dashboard");
            throw;
        }
    }

    /// <summary>
    /// Get real-time metrics for the dashboard
    /// </summary>
    /// <returns>Current dashboard metrics</returns>
    public async Task<DashboardOverviewDto> GetRealTimeMetrics()
    {
        try
        {
            var tenantId = GetTenantId();
            _logger.LogInformation("Getting real-time metrics for tenant {TenantId}", tenantId);

            // TODO: Implement real-time metrics query
            // This would typically query the database or cache for current metrics

            // For now, return mock data
            var mockMetrics = new DashboardOverviewDto
            {
                TotalUnits = 3,
                ActiveQueues = 5,
                WaitingTickets = 25,
                ActiveSessions = 8,
                TotalUsers = 12,
                AvailableUsers = 9,
                AverageWaitingTimeMinutes = 12.5,
                AverageServiceTimeMinutes = 8.2,
                TodayCompletedTickets = 45,
                TodayCancelledTickets = 3,
                CustomerSatisfactionRating = 4.6
            };

            return mockMetrics;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error getting real-time metrics");
            throw;
        }
    }

    /// <summary>
    /// Get unit-specific dashboard metrics
    /// </summary>
    /// <param name="unitId">Unit ID</param>
    /// <returns>Unit dashboard metrics</returns>
    public async Task<UnitDashboardDto> GetUnitMetrics(string unitId)
    {
        try
        {
            var tenantId = GetTenantId();
            _logger.LogInformation("Getting unit metrics for unit {UnitId} in tenant {TenantId}", unitId, tenantId);

            // TODO: Implement unit metrics query

            // For now, return mock data
            var mockUnitMetrics = new UnitDashboardDto
            {
                UnitId = Guid.Parse(unitId),
                UnitName = "Main Office",
                IsOpen = true,
                ActiveQueues = 3,
                WaitingTickets = 15,
                ActiveSessions = 5,
                AvailableUsers = 6,
                AverageWaitingTimeMinutes = 10.5,
                QueueStatuses = new List<QueueStatusSummaryDto>
                {
                    new QueueStatusSummaryDto
                    {
                        QueueId = Guid.NewGuid(),
                        QueueName = "Customer Service",
                        Status = "Open",
                        WaitingTickets = 8,
                        EstimatedWaitingMinutes = 12,
                        IsAcceptingTickets = true
                    },
                    new QueueStatusSummaryDto
                    {
                        QueueId = Guid.NewGuid(),
                        QueueName = "Technical Support",
                        Status = "Open",
                        WaitingTickets = 7,
                        EstimatedWaitingMinutes = 15,
                        IsAcceptingTickets = true
                    }
                },
                UserStatuses = new List<UserStatusSummaryDto>
                {
                    new UserStatusSummaryDto
                    {
                        UserId = Guid.NewGuid(),
                        UserName = "John Doe",
                        Status = "Available",
                        IsAvailable = true,
                        ActiveSessions = 1,
                        CurrentSessionDuration = TimeSpan.FromMinutes(5)
                    },
                    new UserStatusSummaryDto
                    {
                        UserId = Guid.NewGuid(),
                        UserName = "Jane Smith",
                        Status = "Available",
                        IsAvailable = true,
                        ActiveSessions = 0
                    }
                }
            };

            return mockUnitMetrics;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error getting unit metrics for {UnitId}", unitId);
            throw;
        }
    }

    /// <summary>
    /// Get historical metrics for a specific time period
    /// </summary>
    /// <param name="startDate">Start date</param>
    /// <param name="endDate">End date</param>
    /// <returns>Historical metrics</returns>
    public async Task<DashboardMetricsDto> GetHistoricalMetrics(DateTime startDate, DateTime endDate)
    {
        try
        {
            var tenantId = GetTenantId();
            _logger.LogInformation("Getting historical metrics for tenant {TenantId} from {StartDate} to {EndDate}", tenantId, startDate, endDate);

            // TODO: Implement historical metrics query

            // For now, return mock data
            var mockHistoricalMetrics = new DashboardMetricsDto
            {
                Period = "Custom",
                StartDate = startDate,
                EndDate = endDate,
                TotalTickets = 150,
                CompletedTickets = 142,
                CancelledTickets = 5,
                NoShowTickets = 3,
                AverageWaitingTimeMinutes = 11.8,
                AverageServiceTimeMinutes = 7.9,
                CustomerSatisfactionRating = 4.7,
                PeakHours = new List<int> { 9, 10, 14, 15 },
                DailyTicketCounts = new List<DailyTicketCountDto>
                {
                    new DailyTicketCountDto
                    {
                        Date = startDate,
                        TotalTickets = 25,
                        CompletedTickets = 24,
                        CancelledTickets = 1
                    },
                    new DailyTicketCountDto
                    {
                        Date = startDate.AddDays(1),
                        TotalTickets = 28,
                        CompletedTickets = 26,
                        CancelledTickets = 2
                    }
                },
                QueuePerformance = new List<QueuePerformanceDto>
                {
                    new QueuePerformanceDto
                    {
                        QueueId = Guid.NewGuid(),
                        QueueName = "Customer Service",
                        TotalTickets = 80,
                        AverageWaitingTimeMinutes = 12.5,
                        AverageServiceTimeMinutes = 8.1,
                        CustomerSatisfactionRating = 4.6
                    },
                    new QueuePerformanceDto
                    {
                        QueueId = Guid.NewGuid(),
                        QueueName = "Technical Support",
                        TotalTickets = 70,
                        AverageWaitingTimeMinutes = 11.2,
                        AverageServiceTimeMinutes = 7.8,
                        CustomerSatisfactionRating = 4.8
                    }
                }
            };

            return mockHistoricalMetrics;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error getting historical metrics");
            throw;
        }
    }

    /// <summary>
    /// Notify metrics update to all connected dashboard clients
    /// </summary>
    /// <param name="tenantId">Tenant ID</param>
    /// <param name="metrics">Updated metrics</param>
    public async Task NotifyMetricsUpdate(string tenantId, DashboardOverviewDto metrics)
    {
        try
        {
            _logger.LogInformation("Notifying metrics update for tenant {TenantId}", tenantId);

            await Clients.Group($"dashboard_{tenantId}").SendAsync("MetricsUpdated", metrics);

            _logger.LogInformation("Metrics update notification sent for tenant {TenantId}", tenantId);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error notifying metrics update for tenant {TenantId}", tenantId);
            throw;
        }
    }

    /// <summary>
    /// Notify unit metrics update to all connected clients
    /// </summary>
    /// <param name="unitId">Unit ID</param>
    /// <param name="metrics">Updated unit metrics</param>
    public async Task NotifyUnitMetricsUpdate(string unitId, UnitDashboardDto metrics)
    {
        try
        {
            _logger.LogInformation("Notifying unit metrics update for unit {UnitId}", unitId);

            await Clients.Group($"unit_{unitId}").SendAsync("UnitMetricsUpdated", metrics);

            _logger.LogInformation("Unit metrics update notification sent for unit {UnitId}", unitId);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error notifying unit metrics update for unit {UnitId}", unitId);
            throw;
        }
    }

    /// <summary>
    /// Notify queue metrics update to all connected clients
    /// </summary>
    /// <param name="queueId">Queue ID</param>
    /// <param name="metrics">Updated queue metrics</param>
    public async Task NotifyQueueMetricsUpdate(string queueId, object metrics)
    {
        try
        {
            _logger.LogInformation("Notifying queue metrics update for queue {QueueId}", queueId);

            await Clients.Group($"queue_{queueId}").SendAsync("QueueMetricsUpdated", metrics);

            _logger.LogInformation("Queue metrics update notification sent for queue {QueueId}", queueId);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error notifying queue metrics update for queue {QueueId}", queueId);
            throw;
        }
    }

    /// <summary>
    /// Get initial metrics when joining dashboard
    /// </summary>
    /// <param name="tenantId">Tenant ID</param>
    /// <returns>Initial dashboard metrics</returns>
    private async Task<DashboardOverviewDto> GetInitialMetrics(string tenantId)
    {
        // TODO: Implement initial metrics query
        // This would typically query the database for current metrics

        return new DashboardOverviewDto
        {
            TotalUnits = 3,
            ActiveQueues = 5,
            WaitingTickets = 25,
            ActiveSessions = 8,
            TotalUsers = 12,
            AvailableUsers = 9,
            AverageWaitingTimeMinutes = 12.5,
            AverageServiceTimeMinutes = 8.2,
            TodayCompletedTickets = 45,
            TodayCancelledTickets = 3,
            CustomerSatisfactionRating = 4.6
        };
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

            _logger.LogInformation("Dashboard client connected: User {UserId} in tenant {TenantId}", userId, tenantId);

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

            _logger.LogInformation("Dashboard client disconnected: User {UserId} in tenant {TenantId}", userId, tenantId);

            await base.OnDisconnectedAsync(exception);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error in OnDisconnectedAsync");
        }
    }
}