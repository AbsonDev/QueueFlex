using QueueManagement.Domain.Entities;

namespace QueueManagement.Infrastructure.Data.Interfaces;

/// <summary>
/// Generic repository interface for basic CRUD operations
/// </summary>
/// <typeparam name="T">Entity type that inherits from BaseEntity</typeparam>
public interface IGenericRepository<T> where T : BaseEntity
{
    /// <summary>
    /// Get entity by ID with tenant filtering
    /// </summary>
    Task<T?> GetByIdAsync(Guid tenantId, Guid id);

    /// <summary>
    /// Get entity by ID without tenant filtering (for entities without tenant)
    /// </summary>
    Task<T?> GetByIdAsync(Guid id);

    /// <summary>
    /// Get all entities for a specific tenant
    /// </summary>
    Task<List<T>> GetAllAsync(Guid tenantId);

    /// <summary>
    /// Get paged entities for a specific tenant
    /// </summary>
    Task<List<T>> GetPagedAsync(Guid tenantId, int page, int pageSize);

    /// <summary>
    /// Add a new entity
    /// </summary>
    Task<T> AddAsync(T entity);

    /// <summary>
    /// Add multiple entities
    /// </summary>
    Task<List<T>> AddRangeAsync(List<T> entities);

    /// <summary>
    /// Update an existing entity
    /// </summary>
    Task UpdateAsync(T entity);

    /// <summary>
    /// Delete an entity (soft delete)
    /// </summary>
    Task DeleteAsync(T entity);

    /// <summary>
    /// Count entities for a specific tenant
    /// </summary>
    Task<int> CountAsync(Guid tenantId);

    /// <summary>
    /// Check if entity exists for a specific tenant
    /// </summary>
    Task<bool> ExistsAsync(Guid tenantId, Guid id);
}