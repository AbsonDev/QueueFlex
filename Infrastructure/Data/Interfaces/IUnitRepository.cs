using QueueManagement.Domain.Entities;

namespace QueueManagement.Infrastructure.Data.Interfaces;

/// <summary>
/// Repository interface for Unit entity operations
/// </summary>
public interface IUnitRepository : IGenericRepository<Unit>
{
    /// <summary>
    /// Get unit by code
    /// </summary>
    Task<Unit?> GetByCodeAsync(Guid tenantId, string code);

    /// <summary>
    /// Search units by term
    /// </summary>
    Task<List<Unit>> SearchAsync(Guid tenantId, string searchTerm);

    /// <summary>
    /// Check if unit code exists
    /// </summary>
    Task<bool> CodeExistsAsync(Guid tenantId, string code, Guid? excludeId = null);

    /// <summary>
    /// Get unit with queues included
    /// </summary>
    Task<Unit?> GetWithQueuesAsync(Guid tenantId, Guid unitId);

    /// <summary>
    /// Get unit with users included
    /// </summary>
    Task<Unit?> GetWithUsersAsync(Guid tenantId, Guid unitId);

    /// <summary>
    /// Get active units for a tenant
    /// </summary>
    Task<List<Unit>> GetActiveUnitsAsync(Guid tenantId);
}