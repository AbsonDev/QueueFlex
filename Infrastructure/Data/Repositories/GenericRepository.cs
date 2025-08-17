using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using QueueManagement.Domain.Entities;
using QueueManagement.Infrastructure.Data.Interfaces;

namespace QueueManagement.Infrastructure.Data.Repositories;

/// <summary>
/// Generic repository implementation with multi-tenant support
/// </summary>
/// <typeparam name="T">Entity type that inherits from BaseEntity</typeparam>
public class GenericRepository<T> : IGenericRepository<T> where T : BaseEntity
{
    protected readonly QueueManagementDbContext _context;
    protected readonly DbSet<T> _dbSet;
    protected readonly ILogger<GenericRepository<T>> _logger;

    public GenericRepository(QueueManagementDbContext context, ILogger<GenericRepository<T>> logger)
    {
        _context = context ?? throw new ArgumentNullException(nameof(context));
        _dbSet = context.Set<T>();
        _logger = logger ?? throw new ArgumentNullException(nameof(logger));
    }

    public virtual async Task<T?> GetByIdAsync(Guid tenantId, Guid id)
    {
        try
        {
            _logger.LogDebug("Getting entity of type {EntityType} with ID {Id} for tenant {TenantId}", 
                typeof(T).Name, id, tenantId);

            // Check if entity implements IHasTenant
            if (typeof(T).GetInterfaces().Contains(typeof(IHasTenant)))
            {
                return await _dbSet
                    .AsNoTracking()
                    .Where(e => e.Id == id && EF.Property<Guid>(e, "TenantId") == tenantId)
                    .FirstOrDefaultAsync();
            }

            // For entities without tenant (like Tenant itself)
            return await _dbSet
                .AsNoTracking()
                .FirstOrDefaultAsync(e => e.Id == id);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error getting entity of type {EntityType} with ID {Id} for tenant {TenantId}", 
                typeof(T).Name, id, tenantId);
            throw;
        }
    }

    public virtual async Task<T?> GetByIdAsync(Guid id)
    {
        try
        {
            _logger.LogDebug("Getting entity of type {EntityType} with ID {Id}", typeof(T).Name, id);

            return await _dbSet
                .AsNoTracking()
                .FirstOrDefaultAsync(e => e.Id == id);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error getting entity of type {EntityType} with ID {Id}", typeof(T).Name, id);
            throw;
        }
    }

    public virtual async Task<List<T>> GetAllAsync(Guid tenantId)
    {
        try
        {
            _logger.LogDebug("Getting all entities of type {EntityType} for tenant {TenantId}", 
                typeof(T).Name, tenantId);

            // Check if entity implements IHasTenant
            if (typeof(T).GetInterfaces().Contains(typeof(IHasTenant)))
            {
                return await _dbSet
                    .AsNoTracking()
                    .Where(e => EF.Property<Guid>(e, "TenantId") == tenantId && !e.IsDeleted)
                    .ToListAsync();
            }

            // For entities without tenant
            return await _dbSet
                .AsNoTracking()
                .Where(e => !e.IsDeleted)
                .ToListAsync();
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error getting all entities of type {EntityType} for tenant {TenantId}", 
                typeof(T).Name, tenantId);
            throw;
        }
    }

    public virtual async Task<List<T>> GetPagedAsync(Guid tenantId, int page, int pageSize)
    {
        try
        {
            _logger.LogDebug("Getting paged entities of type {EntityType} for tenant {TenantId}, page {Page}, size {PageSize}", 
                typeof(T).Name, tenantId, page, pageSize);

            if (page < 1) page = 1;
            if (pageSize < 1) pageSize = 10;

            // Check if entity implements IHasTenant
            if (typeof(T).GetInterfaces().Contains(typeof(IHasTenant)))
            {
                return await _dbSet
                    .AsNoTracking()
                    .Where(e => EF.Property<Guid>(e, "TenantId") == tenantId && !e.IsDeleted)
                    .Skip((page - 1) * pageSize)
                    .Take(pageSize)
                    .ToListAsync();
            }

            // For entities without tenant
            return await _dbSet
                .AsNoTracking()
                .Where(e => !e.IsDeleted)
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .ToListAsync();
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error getting paged entities of type {EntityType} for tenant {TenantId}", 
                typeof(T).Name, tenantId);
            throw;
        }
    }

    public virtual async Task<T> AddAsync(T entity)
    {
        try
        {
            _logger.LogDebug("Adding entity of type {EntityType} with ID {Id}", typeof(T).Name, entity.Id);

            if (entity == null)
                throw new ArgumentNullException(nameof(entity));

            // Set audit fields
            entity.CreatedAt = DateTime.UtcNow;
            entity.UpdatedAt = DateTime.UtcNow;

            await _dbSet.AddAsync(entity);
            return entity;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error adding entity of type {EntityType}", typeof(T).Name);
            throw;
        }
    }

    public virtual async Task<List<T>> AddRangeAsync(List<T> entities)
    {
        try
        {
            _logger.LogDebug("Adding {Count} entities of type {EntityType}", entities.Count, typeof(T).Name);

            if (entities == null || !entities.Any())
                throw new ArgumentException("Entities list cannot be null or empty", nameof(entities));

            // Set audit fields for all entities
            foreach (var entity in entities)
            {
                entity.CreatedAt = DateTime.UtcNow;
                entity.UpdatedAt = DateTime.UtcNow;
            }

            await _dbSet.AddRangeAsync(entities);
            return entities;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error adding {Count} entities of type {EntityType}", entities.Count, typeof(T).Name);
            throw;
        }
    }

    public virtual Task UpdateAsync(T entity)
    {
        try
        {
            _logger.LogDebug("Updating entity of type {EntityType} with ID {Id}", typeof(T).Name, entity.Id);

            if (entity == null)
                throw new ArgumentNullException(nameof(entity));

            // Set audit fields
            entity.UpdatedAt = DateTime.UtcNow;

            _dbSet.Update(entity);
            return Task.CompletedTask;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error updating entity of type {EntityType} with ID {Id}", typeof(T).Name, entity.Id);
            throw;
        }
    }

    public virtual Task DeleteAsync(T entity)
    {
        try
        {
            _logger.LogDebug("Soft deleting entity of type {EntityType} with ID {Id}", typeof(T).Name, entity.Id);

            if (entity == null)
                throw new ArgumentNullException(nameof(entity));

            // Soft delete implementation
            entity.IsDeleted = true;
            entity.UpdatedAt = DateTime.UtcNow;

            _dbSet.Update(entity);
            return Task.CompletedTask;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error soft deleting entity of type {EntityType} with ID {Id}", typeof(T).Name, entity.Id);
            throw;
        }
    }

    public virtual async Task<int> CountAsync(Guid tenantId)
    {
        try
        {
            _logger.LogDebug("Counting entities of type {EntityType} for tenant {TenantId}", typeof(T).Name, tenantId);

            // Check if entity implements IHasTenant
            if (typeof(T).GetInterfaces().Contains(typeof(IHasTenant)))
            {
                return await _dbSet
                    .CountAsync(e => EF.Property<Guid>(e, "TenantId") == tenantId && !e.IsDeleted);
            }

            // For entities without tenant
            return await _dbSet
                .CountAsync(e => !e.IsDeleted);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error counting entities of type {EntityType} for tenant {TenantId}", 
                typeof(T).Name, tenantId);
            throw;
        }
    }

    public virtual async Task<bool> ExistsAsync(Guid tenantId, Guid id)
    {
        try
        {
            _logger.LogDebug("Checking if entity of type {EntityType} with ID {Id} exists for tenant {TenantId}", 
                typeof(T).Name, id, tenantId);

            // Check if entity implements IHasTenant
            if (typeof(T).GetInterfaces().Contains(typeof(IHasTenant)))
            {
                return await _dbSet
                    .AnyAsync(e => e.Id == id && EF.Property<Guid>(e, "TenantId") == tenantId && !e.IsDeleted);
            }

            // For entities without tenant
            return await _dbSet
                .AnyAsync(e => e.Id == id && !e.IsDeleted);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error checking if entity of type {EntityType} with ID {Id} exists for tenant {TenantId}", 
                typeof(T).Name, id, tenantId);
            throw;
        }
    }
}