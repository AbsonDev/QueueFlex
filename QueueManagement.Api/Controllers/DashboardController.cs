using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using QueueManagement.Api.DTOs.Dashboard;
using QueueManagement.Api.DTOs.Common;
using MediatR;
using Swashbuckle.AspNetCore.Annotations;

namespace QueueManagement.Api.Controllers;

/// <summary>
/// Controller for dashboard metrics and overviews
/// </summary>
[SwaggerTag("Dashboard endpoints for retrieving metrics, statistics, and real-time data")]
public class DashboardController : BaseController
{
    public DashboardController(IMediator mediator, ILogger<DashboardController> logger) : base(mediator, logger) { }

    /// <summary>
    /// Get dashboard overview
    /// </summary>
    /// <returns>Dashboard overview with key metrics</returns>
    [HttpGet("overview")]
    [ProducesResponseType(typeof(ApiResponse<DashboardOverviewDto>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status400BadRequest)]
    [SwaggerOperation(
        Summary = "Get dashboard overview",
        Description = "Retrieves an overview of key dashboard metrics",
        OperationId = "GetDashboardOverview"
    )]
    public async Task<ActionResult<ApiResponse<DashboardOverviewDto>>> GetDashboardOverview()
    {
        try
        {
            var tenantId = GetTenantId();
            _logger.LogInformation("Getting dashboard overview for tenant: {TenantId}", tenantId);

            // TODO: Implement get dashboard overview query with MediatR
            // var query = new GetDashboardOverviewQuery { TenantId = tenantId };
            // var result = await _mediator.Send(query);

            // For now, return mock data
            var mockOverview = new DashboardOverviewDto
            {
                TotalUnits = 3,
                ActiveQueues = 8,
                WaitingTickets = 45,
                ActiveSessions = 12,
                TotalUsers = 25,
                AvailableUsers = 18,
                AverageWaitingTimeMinutes = 8.5,
                AverageServiceTimeMinutes = 12.3,
                TodayCompletedTickets = 156,
                TodayCancelledTickets = 3,
                CustomerSatisfactionRating = 4.2
            };

            return Success(mockOverview);
        }
        catch (Exception ex)
        {
            return LogAndReturnError<DashboardOverviewDto>(ex, "Failed to get dashboard overview");
        }
    }

    /// <summary>
    /// Get unit dashboard
    /// </summary>
    /// <param name="unitId">Unit ID</param>
    /// <returns>Unit-specific dashboard data</returns>
    [HttpGet("unit/{unitId:guid}")]
    [ProducesResponseType(typeof(ApiResponse<UnitDashboardDto>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status404NotFound)]
    [SwaggerOperation(
        Summary = "Get unit dashboard",
        Description = "Retrieves dashboard data for a specific unit",
        OperationId = "GetUnitDashboard"
    )]
    public async Task<ActionResult<ApiResponse<UnitDashboardDto>>> GetUnitDashboard(Guid unitId)
    {
        try
        {
            var tenantId = GetTenantId();
            _logger.LogInformation("Getting unit dashboard for unit {UnitId} in tenant: {TenantId}", unitId, tenantId);

            // TODO: Implement get unit dashboard query with MediatR
            // var query = new GetUnitDashboardQuery { UnitId = unitId, TenantId = tenantId };
            // var result = await _mediator.Send(query);

            // For now, return mock data
            var mockUnitDashboard = new UnitDashboardDto
            {
                UnitId = unitId,
                UnitName = "Main Office",
                IsOpen = true,
                ActiveQueues = 3,
                WaitingTickets = 18,
                ActiveSessions = 5,
                AvailableUsers = 8,
                AverageWaitingTimeMinutes = 7.2,
                QueueStatuses = new List<QueueStatusSummaryDto>
                {
                    new QueueStatusSummaryDto
                    {
                        QueueId = Guid.NewGuid(),
                        QueueName = "Customer Service",
                        Status = "Open",
                        WaitingTickets = 12,
                        EstimatedWaitingMinutes = 8,
                        IsAcceptingTickets = true
                    },
                    new QueueStatusSummaryDto
                    {
                        QueueId = Guid.NewGuid(),
                        QueueName = "Technical Support",
                        Status = "Open",
                        WaitingTickets = 6,
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
                        ActiveSessions = 0
                    },
                    new UserStatusSummaryDto
                    {
                        UserId = Guid.NewGuid(),
                        UserName = "Jane Smith",
                        Status = "In Session",
                        IsAvailable = false,
                        ActiveSessions = 1,
                        CurrentSessionDuration = TimeSpan.FromMinutes(12)
                    }
                }
            };

            return Success(mockUnitDashboard);
        }
        catch (Exception ex)
        {
            return LogAndReturnError<UnitDashboardDto>(ex, "Failed to get unit dashboard");
        }
    }

    /// <summary>
    /// Get dashboard metrics for a specific time period
    /// </summary>
    /// <param name="startDate">Start date</param>
    /// <param name="endDate">End date</param>
    /// <returns>Dashboard metrics for the specified period</returns>
    [HttpGet("metrics")]
    [ProducesResponseType(typeof(ApiResponse<DashboardMetricsDto>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status400BadRequest)]
    [SwaggerOperation(
        Summary = "Get dashboard metrics",
        Description = "Retrieves detailed dashboard metrics for a specific time period",
        OperationId = "GetDashboardMetrics"
    )]
    public async Task<ActionResult<ApiResponse<DashboardMetricsDto>>> GetDashboardMetrics(
        [FromQuery] DateTime startDate,
        [FromQuery] DateTime endDate)
    {
        try
        {
            var tenantId = GetTenantId();
            _logger.LogInformation("Getting dashboard metrics for tenant: {TenantId} from {StartDate} to {EndDate}", 
                tenantId, startDate, endDate);

            // TODO: Implement get dashboard metrics query with MediatR
            // var query = new GetDashboardMetricsQuery { TenantId = tenantId, StartDate = startDate, EndDate = endDate };
            // var result = await _mediator.Send(query);

            // For now, return mock data
            var mockMetrics = new DashboardMetricsDto
            {
                Period = "Last 7 days",
                StartDate = startDate,
                EndDate = endDate,
                TotalTickets = 1250,
                CompletedTickets = 1180,
                CancelledTickets = 45,
                NoShowTickets = 25,
                AverageWaitingTimeMinutes = 8.7,
                AverageServiceTimeMinutes = 13.2,
                CustomerSatisfactionRating = 4.3,
                PeakHours = new List<int> { 9, 10, 14, 15 },
                DailyTicketCounts = new List<DailyTicketCountDto>
                {
                    new DailyTicketCountDto
                    {
                        Date = DateTime.Today.AddDays(-6),
                        TotalTickets = 165,
                        CompletedTickets = 158,
                        CancelledTickets = 7
                    },
                    new DailyTicketCountDto
                    {
                        Date = DateTime.Today.AddDays(-5),
                        TotalTickets = 178,
                        CompletedTickets = 172,
                        CancelledTickets = 6
                    }
                },
                QueuePerformance = new List<QueuePerformanceDto>
                {
                    new QueuePerformanceDto
                    {
                        QueueId = Guid.NewGuid(),
                        QueueName = "Customer Service",
                        TotalTickets = 650,
                        AverageWaitingTimeMinutes = 6.8,
                        AverageServiceTimeMinutes = 11.5,
                        CustomerSatisfactionRating = 4.4
                    },
                    new QueuePerformanceDto
                    {
                        QueueId = Guid.NewGuid(),
                        QueueName = "Technical Support",
                        TotalTickets = 600,
                        AverageWaitingTimeMinutes = 12.3,
                        AverageServiceTimeMinutes = 18.7,
                        CustomerSatisfactionRating = 4.1
                    }
                }
            };

            return Success(mockMetrics);
        }
        catch (Exception ex)
        {
            return LogAndReturnError<DashboardMetricsDto>(ex, "Failed to get dashboard metrics");
        }
    }

    /// <summary>
    /// Get real-time queue status
    /// </summary>
    /// <returns>Current status of all queues</returns>
    [HttpGet("queues/status")]
    [ProducesResponseType(typeof(ApiResponse<List<QueueStatusSummaryDto>>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status400BadRequest)]
    [SwaggerOperation(
        Summary = "Get real-time queue status",
        Description = "Retrieves the current status of all queues",
        OperationId = "GetRealTimeQueueStatus"
    )]
    public async Task<ActionResult<ApiResponse<List<QueueStatusSummaryDto>>>> GetRealTimeQueueStatus()
    {
        try
        {
            var tenantId = GetTenantId();
            _logger.LogInformation("Getting real-time queue status for tenant: {TenantId}", tenantId);

            // TODO: Implement get real-time queue status query with MediatR
            // var query = new GetRealTimeQueueStatusQuery { TenantId = tenantId };
            // var result = await _mediator.Send(query);

            // For now, return mock data
            var mockQueueStatuses = new List<QueueStatusSummaryDto>
            {
                new QueueStatusSummaryDto
                {
                    QueueId = Guid.NewGuid(),
                    QueueName = "Customer Service",
                    Status = "Open",
                    WaitingTickets = 25,
                    EstimatedWaitingMinutes = 10,
                    IsAcceptingTickets = true
                },
                new QueueStatusSummaryDto
                {
                    QueueId = Guid.NewGuid(),
                    QueueName = "Technical Support",
                    Status = "Open",
                    WaitingTickets = 15,
                    EstimatedWaitingMinutes = 20,
                    IsAcceptingTickets = true
                },
                new QueueStatusSummaryDto
                {
                    QueueId = Guid.NewGuid(),
                    QueueName = "Document Processing",
                    Status = "Closed",
                    WaitingTickets = 0,
                    EstimatedWaitingMinutes = null,
                    IsAcceptingTickets = false
                }
            };

            return Success(mockQueueStatuses);
        }
        catch (Exception ex)
        {
            return LogAndReturnError<List<QueueStatusSummaryDto>>(ex, "Failed to get real-time queue status");
        }
    }

    /// <summary>
    /// Get user activity summary
    /// </summary>
    /// <returns>Summary of user activity and performance</returns>
    [HttpGet("users/activity")]
    [ProducesResponseType(typeof(ApiResponse<List<UserStatusSummaryDto>>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status400BadRequest)]
    [SwaggerOperation(
        Summary = "Get user activity summary",
        Description = "Retrieves a summary of user activity and performance",
        OperationId = "GetUserActivitySummary"
    )]
    public async Task<ActionResult<ApiResponse<List<UserStatusSummaryDto>>>> GetUserActivitySummary()
    {
        try
        {
            var tenantId = GetTenantId();
            _logger.LogInformation("Getting user activity summary for tenant: {TenantId}", tenantId);

            // TODO: Implement get user activity summary query with MediatR
            // var query = new GetUserActivitySummaryQuery { TenantId = tenantId };
            // var result = await _mediator.Send(query);

            // For now, return mock data
            var mockUserActivity = new List<UserStatusSummaryDto>
            {
                new UserStatusSummaryDto
                {
                    UserId = Guid.NewGuid(),
                    UserName = "John Doe",
                    Status = "Available",
                    IsAvailable = true,
                    ActiveSessions = 0
                },
                new UserStatusSummaryDto
                {
                    UserId = Guid.NewGuid(),
                    UserName = "Jane Smith",
                    Status = "In Session",
                    IsAvailable = false,
                    ActiveSessions = 1,
                    CurrentSessionDuration = TimeSpan.FromMinutes(8)
                },
                new UserStatusSummaryDto
                {
                    UserId = Guid.NewGuid(),
                    UserName = "Bob Johnson",
                    Status = "Break",
                    IsAvailable = false,
                    ActiveSessions = 0
                }
            };

            return Success(mockUserActivity);
        }
        catch (Exception ex)
        {
            return LogAndReturnError<List<UserStatusSummaryDto>>(ex, "Failed to get user activity summary");
        }
    }

    /// <summary>
    /// Get performance trends
    /// </summary>
    /// <param name="days">Number of days to analyze</param>
    /// <returns>Performance trends over the specified period</returns>
    [HttpGet("trends")]
    [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status400BadRequest)]
    [SwaggerOperation(
        Summary = "Get performance trends",
        Description = "Retrieves performance trends over a specified period",
        OperationId = "GetPerformanceTrends"
    )]
    public async Task<ActionResult<ApiResponse<object>>> GetPerformanceTrends([FromQuery] int days = 30)
    {
        try
        {
            var tenantId = GetTenantId();
            _logger.LogInformation("Getting performance trends for tenant: {TenantId} over {Days} days", tenantId, days);

            // TODO: Implement get performance trends query with MediatR
            // var query = new GetPerformanceTrendsQuery { TenantId = tenantId, Days = days };
            // var result = await _mediator.Send(query);

            // For now, return mock data
            var mockTrends = new
            {
                Period = $"{days} days",
                AverageWaitingTimeTrend = new[] { 8.2, 7.8, 8.1, 7.9, 8.3, 8.0, 7.7 },
                AverageServiceTimeTrend = new[] { 12.5, 12.8, 12.3, 12.6, 12.4, 12.7, 12.2 },
                CustomerSatisfactionTrend = new[] { 4.1, 4.2, 4.3, 4.2, 4.4, 4.3, 4.2 },
                TicketVolumeTrend = new[] { 150, 165, 158, 172, 168, 175, 170 }
            };

            return Success(mockTrends);
        }
        catch (Exception ex)
        {
            return LogAndReturnError<object>(ex, "Failed to get performance trends");
        }
    }
}