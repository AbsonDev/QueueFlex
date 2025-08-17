using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using QueueManagement.Domain.Entities;
using QueueManagement.Infrastructure.Data.Interfaces;

namespace QueueManagement.Infrastructure.Data.Repositories;

/// <summary>
/// Repository implementation for Tenant entity
/// </summary>
public class TenantRepository : GenericRepository<Tenant>, ITenantRepository
{
    public TenantRepository(QueueManagementDbContext context, ILogger<TenantRepository> logger) 
        : base(context, logger) { }

    public async Task<Tenant?> GetBySubdomainAsync(string subdomain)
    {
        try
        {
            _logger.LogDebug("Getting tenant by subdomain {Subdomain}", subdomain);

            if (string.IsNullOrWhiteSpace(subdomain))
                throw new ArgumentException("Subdomain cannot be null or empty", nameof(subdomain));

            var normalizedSubdomain = subdomain.ToLowerInvariant();

            return await _dbSet
                .Include(t => t.Units.Where(u => !u.IsDeleted))
                .AsNoTracking()
                .Where(t => t.Subdomain.ToLower() == normalizedSubdomain && t.IsActive && !t.IsDeleted)
                .FirstOrDefaultAsync();
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error getting tenant by subdomain {Subdomain}", subdomain);
            throw;
        }
    }

    public async Task<bool> SubdomainExistsAsync(string subdomain, Guid? excludeId = null)
    {
        try
        {
            _logger.LogDebug("Checking if subdomain {Subdomain} exists", subdomain);

            if (string.IsNullOrWhiteSpace(subdomain))
                throw new ArgumentException("Subdomain cannot be null or empty", nameof(subdomain));

            var normalizedSubdomain = subdomain.ToLowerInvariant();

            var query = _dbSet
                .AsNoTracking()
                .Where(t => t.Subdomain.ToLower() == normalizedSubdomain && !t.IsDeleted);

            if (excludeId.HasValue)
            {
                query = query.Where(t => t.Id != excludeId.Value);
            }

            return await query.AnyAsync();
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error checking if subdomain {Subdomain} exists", subdomain);
            throw;
        }
    }

    public async Task<List<Tenant>> GetActiveTenants()
    {
        try
        {
            _logger.LogDebug("Getting all active tenants");

            return await _dbSet
                .Include(t => t.Units.Where(u => !u.IsDeleted))
                .AsNoTracking()
                .Where(t => t.IsActive && !t.IsDeleted)
                .OrderBy(t => t.Name)
                .ToListAsync();
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error getting active tenants");
            throw;
        }
    }

    // Override base methods since Tenant doesn't have TenantId
    public override async Task<Tenant?> GetByIdAsync(Guid tenantId, Guid id)
    {
        // For Tenant entity, we ignore tenantId parameter and just get by id
        return await GetByIdAsync(id);
    }

    public override async Task<List<Tenant>> GetAllAsync(Guid tenantId)
    {
        // For Tenant entity, we ignore tenantId parameter and get all active tenants
        return await GetActiveTenants();
    }

    public override async Task<List<Tenant>> GetPagedAsync(Guid tenantId, int page, int pageSize)
    {
        // For Tenant entity, we ignore tenantId parameter
        try
        {
            _logger.LogDebug("Getting paged tenants, page {Page}, size {PageSize}", page, pageSize);

            if (page < 1) page = 1;
            if (pageSize < 1) pageSize = 10;

            return await _dbSet
                .AsNoTracking()
                .Where(t => !t.IsDeleted)
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .OrderBy(t => t.Name)
                .ToListAsync();
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error getting paged tenants");
            throw;
        }
    }

    public override async Task<int> CountAsync(Guid tenantId)
    {
        // For Tenant entity, we ignore tenantId parameter
        try
        {
            _logger.LogDebug("Counting all tenants");

            return await _dbSet
                .CountAsync(t => !t.IsDeleted);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error counting tenants");
            throw;
        }
    }

    public override async Task<bool> ExistsAsync(Guid tenantId, Guid id)
    {
        // For Tenant entity, we ignore tenantId parameter
        try
        {
            _logger.LogDebug("Checking if tenant with ID {Id} exists", id);

            return await _dbSet
                .AnyAsync(t => t.Id == id && !t.IsDeleted);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error checking if tenant with ID {Id} exists", id);
            throw;
        }
    }
}