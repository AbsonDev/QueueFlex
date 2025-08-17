using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using QueueManagement.Domain.Entities;
using QueueManagement.Infrastructure.Data.Interfaces;

namespace QueueManagement.Infrastructure.Data.Repositories;

/// <summary>
/// Repository implementation for Unit entity with optimized queries
/// </summary>
public class UnitRepository : GenericRepository<Unit>, IUnitRepository
{
    public UnitRepository(QueueManagementDbContext context, ILogger<UnitRepository> logger) 
        : base(context, logger) { }

    public async Task<Unit?> GetByCodeAsync(Guid tenantId, string code)
    {
        try
        {
            _logger.LogDebug("Getting unit by code {Code} for tenant {TenantId}", code, tenantId);

            return await _dbSet
                .Include(u => u.Queues.Where(q => !q.IsDeleted))
                .Include(u => u.Users.Where(us => !us.IsDeleted))
                .AsNoTracking()
                .Where(u => u.Code == code && u.TenantId == tenantId && !u.IsDeleted)
                .FirstOrDefaultAsync();
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error getting unit by code {Code} for tenant {TenantId}", code, tenantId);
            throw;
        }
    }

    public async Task<List<Unit>> SearchAsync(Guid tenantId, string searchTerm)
    {
        try
        {
            _logger.LogDebug("Searching units with term '{SearchTerm}' for tenant {TenantId}", searchTerm, tenantId);

            if (string.IsNullOrWhiteSpace(searchTerm))
                return await GetAllAsync(tenantId);

            var normalizedSearchTerm = searchTerm.ToLowerInvariant();

            return await _dbSet
                .Include(u => u.Queues.Where(q => !q.IsDeleted))
                .AsNoTracking()
                .Where(u => u.TenantId == tenantId && !u.IsDeleted &&
                           (u.Name.ToLower().Contains(normalizedSearchTerm) ||
                            u.Code.ToLower().Contains(normalizedSearchTerm) ||
                            u.Address.ToLower().Contains(normalizedSearchTerm)))
                .OrderBy(u => u.Name)
                .ToListAsync();
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error searching units with term '{SearchTerm}' for tenant {TenantId}", 
                searchTerm, tenantId);
            throw;
        }
    }

    public async Task<bool> CodeExistsAsync(Guid tenantId, string code, Guid? excludeId = null)
    {
        try
        {
            _logger.LogDebug("Checking if unit code {Code} exists for tenant {TenantId}", code, tenantId);

            var query = _dbSet
                .AsNoTracking()
                .Where(u => u.Code == code && u.TenantId == tenantId && !u.IsDeleted);

            if (excludeId.HasValue)
            {
                query = query.Where(u => u.Id != excludeId.Value);
            }

            return await query.AnyAsync();
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error checking if unit code {Code} exists for tenant {TenantId}", code, tenantId);
            throw;
        }
    }

    public async Task<Unit?> GetWithQueuesAsync(Guid tenantId, Guid unitId)
    {
        try
        {
            _logger.LogDebug("Getting unit {UnitId} with queues for tenant {TenantId}", unitId, tenantId);

            return await _dbSet
                .Include(u => u.Queues.Where(q => !q.IsDeleted))
                    .ThenInclude(q => q.Services.Where(s => !s.IsDeleted))
                .Include(u => u.OperatingHours.Where(oh => !oh.IsDeleted))
                .AsNoTracking()
                .Where(u => u.Id == unitId && u.TenantId == tenantId && !u.IsDeleted)
                .FirstOrDefaultAsync();
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error getting unit {UnitId} with queues for tenant {TenantId}", unitId, tenantId);
            throw;
        }
    }

    public async Task<Unit?> GetWithUsersAsync(Guid tenantId, Guid unitId)
    {
        try
        {
            _logger.LogDebug("Getting unit {UnitId} with users for tenant {TenantId}", unitId, tenantId);

            return await _dbSet
                .Include(u => u.Users.Where(us => !us.IsDeleted))
                .Include(u => u.OperatingHours.Where(oh => !oh.IsDeleted))
                .AsNoTracking()
                .Where(u => u.Id == unitId && u.TenantId == tenantId && !u.IsDeleted)
                .FirstOrDefaultAsync();
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error getting unit {UnitId} with users for tenant {TenantId}", unitId, tenantId);
            throw;
        }
    }

    public async Task<List<Unit>> GetActiveUnitsAsync(Guid tenantId)
    {
        try
        {
            _logger.LogDebug("Getting active units for tenant {TenantId}", tenantId);

            return await _dbSet
                .Include(u => u.Queues.Where(q => !q.IsDeleted && q.IsActive))
                .AsNoTracking()
                .Where(u => u.TenantId == tenantId && u.IsActive && !u.IsDeleted)
                .OrderBy(u => u.Name)
                .ToListAsync();
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error getting active units for tenant {TenantId}", tenantId);
            throw;
        }
    }
}