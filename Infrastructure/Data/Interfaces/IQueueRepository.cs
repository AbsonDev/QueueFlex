using QueueManagement.Domain.Entities;

namespace QueueManagement.Infrastructure.Data.Interfaces;

/// <summary>
/// Repository interface for Queue entity operations
/// </summary>
public interface IQueueRepository : IGenericRepository<Queue>
{
    /// <summary>
    /// Get queues by unit
    /// </summary>
    Task<List<Queue>> GetByUnitAsync(Guid tenantId, Guid unitId);

    /// <summary>
    /// Get queue with services included
    /// </summary>
    Task<Queue?> GetWithServicesAsync(Guid tenantId, Guid queueId);

    /// <summary>
    /// Get queue by code for a specific unit
    /// </summary>
    Task<Queue?> GetByCodeAsync(Guid tenantId, Guid unitId, string code);

    /// <summary>
    /// Get active queues for a tenant
    /// </summary>
    Task<List<Queue>> GetActiveQueuesAsync(Guid tenantId, Guid? unitId = null);

    /// <summary>
    /// Get real-time status of a queue
    /// </summary>
    Task<QueueStatusDto> GetRealTimeStatusAsync(Guid tenantId, Guid queueId);

    /// <summary>
    /// Search queues by term
    /// </summary>
    Task<List<Queue>> SearchAsync(Guid tenantId, string searchTerm);

    /// <summary>
    /// Check if queue code exists
    /// </summary>
    Task<bool> CodeExistsAsync(Guid tenantId, Guid unitId, string code, Guid? excludeId = null);
}

/// <summary>
/// DTO for queue real-time status
/// </summary>
public class QueueStatusDto
{
    public Guid QueueId { get; set; }
    public string QueueName { get; set; } = string.Empty;
    public int WaitingCount { get; set; }
    public int InServiceCount { get; set; }
    public int CompletedToday { get; set; }
    public double AverageWaitTime { get; set; }
    public DateTime LastUpdate { get; set; }
}