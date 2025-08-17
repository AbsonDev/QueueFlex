using QueueManagement.Domain.Entities;

namespace QueueManagement.Infrastructure.Data.Interfaces;

/// <summary>
/// Repository interface for Tenant entity operations
/// </summary>
public interface ITenantRepository : IGenericRepository<Tenant>
{
    /// <summary>
    /// Get tenant by subdomain
    /// </summary>
    Task<Tenant?> GetBySubdomainAsync(string subdomain);

    /// <summary>
    /// Check if subdomain exists
    /// </summary>
    Task<bool> SubdomainExistsAsync(string subdomain, Guid? excludeId = null);

    /// <summary>
    /// Get active tenants
    /// </summary>
    Task<List<Tenant>> GetActiveTenants();
}