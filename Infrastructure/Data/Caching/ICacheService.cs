namespace QueueManagement.Infrastructure.Data.Caching;

/// <summary>
/// Cache service interface for performance optimization
/// </summary>
public interface ICacheService
{
    /// <summary>
    /// Get value from cache
    /// </summary>
    Task<T?> GetAsync<T>(string key);

    /// <summary>
    /// Set value in cache with expiration
    /// </summary>
    Task SetAsync<T>(string key, T value, TimeSpan? expiration = null);

    /// <summary>
    /// Remove value from cache
    /// </summary>
    Task RemoveAsync(string key);

    /// <summary>
    /// Remove multiple keys from cache
    /// </summary>
    Task RemoveAsync(params string[] keys);

    /// <summary>
    /// Check if key exists in cache
    /// </summary>
    Task<bool> ExistsAsync(string key);

    /// <summary>
    /// Get or set value with factory function
    /// </summary>
    Task<T> GetOrSetAsync<T>(string key, Func<Task<T>> factory, TimeSpan? expiration = null);

    /// <summary>
    /// Invalidate cache by pattern
    /// </summary>
    Task InvalidateByPatternAsync(string pattern);
}