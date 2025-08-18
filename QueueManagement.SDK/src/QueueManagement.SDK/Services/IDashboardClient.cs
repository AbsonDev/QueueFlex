using QueueManagement.SDK.Models.Dashboard;

namespace QueueManagement.SDK.Services;

/// <summary>
/// Client for accessing dashboard metrics and analytics.
/// </summary>
public interface IDashboardClient
{
    /// <summary>
    /// Gets dashboard metrics.
    /// </summary>
    Task<DashboardMetricsResponse> GetMetricsAsync(DashboardMetricsRequest? request = null, CancellationToken cancellationToken = default);

    /// <summary>
    /// Gets real-time statistics.
    /// </summary>
    Task<RealTimeStatistics> GetRealTimeStatisticsAsync(Guid? unitId = null, CancellationToken cancellationToken = default);

    /// <summary>
    /// Gets unit metrics.
    /// </summary>
    Task<UnitMetrics> GetUnitMetricsAsync(Guid unitId, CancellationToken cancellationToken = default);

    /// <summary>
    /// Gets queue metrics.
    /// </summary>
    Task<QueueMetrics> GetQueueMetricsAsync(Guid queueId, CancellationToken cancellationToken = default);

    /// <summary>
    /// Gets agent performance metrics.
    /// </summary>
    Task<AgentPerformance> GetAgentPerformanceAsync(Guid agentId, DateTime? date = null, CancellationToken cancellationToken = default);

    /// <summary>
    /// Gets top performing agents.
    /// </summary>
    Task<List<AgentPerformance>> GetTopAgentsAsync(int count = 10, DateTime? date = null, CancellationToken cancellationToken = default);

    /// <summary>
    /// Gets active alerts.
    /// </summary>
    Task<List<Alert>> GetAlertsAsync(string? severity = null, CancellationToken cancellationToken = default);

    /// <summary>
    /// Acknowledges an alert.
    /// </summary>
    Task<Alert> AcknowledgeAlertAsync(Guid alertId, CancellationToken cancellationToken = default);

    /// <summary>
    /// Gets historical metrics.
    /// </summary>
    Task<DashboardMetricsResponse> GetHistoricalMetricsAsync(DateTime fromDate, DateTime toDate, Guid? unitId = null, CancellationToken cancellationToken = default);

    /// <summary>
    /// Exports metrics to CSV.
    /// </summary>
    Task<Stream> ExportMetricsAsync(DateTime fromDate, DateTime toDate, string format = "csv", CancellationToken cancellationToken = default);

    /// <summary>
    /// Gets service level metrics.
    /// </summary>
    Task<Dictionary<string, double>> GetServiceLevelMetricsAsync(Guid? unitId = null, CancellationToken cancellationToken = default);

    /// <summary>
    /// Gets customer satisfaction metrics.
    /// </summary>
    Task<Dictionary<string, double>> GetCustomerSatisfactionMetricsAsync(Guid? unitId = null, DateTime? date = null, CancellationToken cancellationToken = default);
}