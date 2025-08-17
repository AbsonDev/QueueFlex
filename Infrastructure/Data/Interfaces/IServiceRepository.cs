using QueueManagement.Domain.Entities;

namespace QueueManagement.Infrastructure.Data.Interfaces;

/// <summary>
/// Repository interface for Service entity operations
/// </summary>
public interface IServiceRepository : IGenericRepository<Service>
{
    /// <summary>
    /// Get service by code
    /// </summary>
    Task<Service?> GetByCodeAsync(Guid tenantId, string code);

    /// <summary>
    /// Get active services for a tenant
    /// </summary>
    Task<List<Service>> GetActiveServicesAsync(Guid tenantId);

    /// <summary>
    /// Check if service code exists
    /// </summary>
    Task<bool> CodeExistsAsync(Guid tenantId, string code, Guid? excludeId = null);

    /// <summary>
    /// Get services by queue
    /// </summary>
    Task<List<Service>> GetServicesByQueueAsync(Guid tenantId, Guid queueId);

    /// <summary>
    /// Search services by term
    /// </summary>
    Task<List<Service>> SearchAsync(Guid tenantId, string searchTerm);
}