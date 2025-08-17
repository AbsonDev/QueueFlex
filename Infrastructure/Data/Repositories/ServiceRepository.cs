using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using QueueManagement.Domain.Entities;
using QueueManagement.Infrastructure.Data.Interfaces;

namespace QueueManagement.Infrastructure.Data.Repositories;

/// <summary>
/// Repository implementation for Service entity with optimized queries
/// </summary>
public class ServiceRepository : GenericRepository<Service>, IServiceRepository
{
    public ServiceRepository(QueueManagementDbContext context, ILogger<ServiceRepository> logger) 
        : base(context, logger) { }

    public async Task<Service?> GetByCodeAsync(Guid tenantId, string code)
    {
        try
        {
            _logger.LogDebug("Getting service by code {Code} for tenant {TenantId}", code, tenantId);

            return await _dbSet
                .Include(s => s.Queues.Where(q => !q.IsDeleted))
                .AsNoTracking()
                .Where(s => s.Code == code && s.Unit.TenantId == tenantId && !s.IsDeleted)
                .FirstOrDefaultAsync();
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error getting service by code {Code} for tenant {TenantId}", code, tenantId);
            throw;
        }
    }

    public async Task<List<Service>> GetActiveServicesAsync(Guid tenantId)
    {
        try
        {
            _logger.LogDebug("Getting active services for tenant {TenantId}", tenantId);

            return await _dbSet
                .Include(s => s.Unit)
                .Include(s => s.Queues.Where(q => !q.IsDeleted))
                .AsNoTracking()
                .Where(s => s.Unit.TenantId == tenantId && s.IsActive && !s.IsDeleted)
                .OrderBy(s => s.Unit.Name)
                .ThenBy(s => s.Name)
                .ToListAsync();
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error getting active services for tenant {TenantId}", tenantId);
            throw;
        }
    }

    public async Task<bool> CodeExistsAsync(Guid tenantId, string code, Guid? excludeId = null)
    {
        try
        {
            _logger.LogDebug("Checking if service code {Code} exists for tenant {TenantId}", code, tenantId);

            var query = _dbSet
                .AsNoTracking()
                .Where(s => s.Code == code && s.Unit.TenantId == tenantId && !s.IsDeleted);

            if (excludeId.HasValue)
            {
                query = query.Where(s => s.Id != excludeId.Value);
            }

            return await query.AnyAsync();
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error checking if service code {Code} exists for tenant {TenantId}", code, tenantId);
            throw;
        }
    }

    public async Task<List<Service>> GetServicesByQueueAsync(Guid tenantId, Guid queueId)
    {
        try
        {
            _logger.LogDebug("Getting services by queue {QueueId} for tenant {TenantId}", queueId, tenantId);

            return await _dbSet
                .Include(s => s.Unit)
                .AsNoTracking()
                .Where(s => s.Queues.Any(q => q.Id == queueId && q.Unit.TenantId == tenantId) && !s.IsDeleted)
                .OrderBy(s => s.Name)
                .ToListAsync();
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error getting services by queue {QueueId} for tenant {TenantId}", queueId, tenantId);
            throw;
        }
    }

    public async Task<List<Service>> SearchAsync(Guid tenantId, string searchTerm)
    {
        try
        {
            _logger.LogDebug("Searching services with term '{SearchTerm}' for tenant {TenantId}", searchTerm, tenantId);

            if (string.IsNullOrWhiteSpace(searchTerm))
                return await GetActiveServicesAsync(tenantId);

            var normalizedSearchTerm = searchTerm.ToLowerInvariant();

            return await _dbSet
                .Include(s => s.Unit)
                .Include(s => s.Queues.Where(q => !q.IsDeleted))
                .AsNoTracking()
                .Where(s => s.Unit.TenantId == tenantId && !s.IsDeleted &&
                           (s.Name.ToLower().Contains(normalizedSearchTerm) ||
                            s.Code.ToLower().Contains(normalizedSearchTerm) ||
                            s.Description.ToLower().Contains(normalizedSearchTerm)))
                .OrderBy(s => s.Unit.Name)
                .ThenBy(s => s.Name)
                .ToListAsync();
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error searching services with term '{SearchTerm}' for tenant {TenantId}", 
                searchTerm, tenantId);
            throw;
        }
    }
}