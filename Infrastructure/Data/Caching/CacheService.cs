using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Caching.Distributed;
using Microsoft.Extensions.Logging;
using System.Text.Json;

namespace QueueManagement.Infrastructure.Data.Caching;

/// <summary>
/// Cache service implementation with memory and distributed cache support
/// </summary>
public class CacheService : ICacheService
{
    private readonly IMemoryCache _memoryCache;
    private readonly IDistributedCache _distributedCache;
    private readonly ILogger<CacheService> _logger;
    private readonly TimeSpan _defaultExpiration = TimeSpan.FromMinutes(30);

    public CacheService(
        IMemoryCache memoryCache,
        IDistributedCache distributedCache,
        ILogger<CacheService> logger)
    {
        _memoryCache = memoryCache ?? throw new ArgumentNullException(nameof(memoryCache));
        _distributedCache = distributedCache ?? throw new ArgumentNullException(nameof(distributedCache));
        _logger = logger ?? throw new ArgumentNullException(nameof(logger));
    }

    public async Task<T?> GetAsync<T>(string key)
    {
        try
        {
            if (string.IsNullOrWhiteSpace(key))
                throw new ArgumentException("Cache key cannot be null or empty", nameof(key));

            _logger.LogDebug("Getting value from cache with key: {Key}", key);

            // Try memory cache first (faster)
            if (_memoryCache.TryGetValue(key, out T? memoryValue))
            {
                _logger.LogDebug("Cache hit in memory cache for key: {Key}", key);
                return memoryValue;
            }

            // Try distributed cache
            var distributedValue = await _distributedCache.GetStringAsync(key);
            if (!string.IsNullOrEmpty(distributedValue))
            {
                try
                {
                    var value = JsonSerializer.Deserialize<T>(distributedValue);
                    
                    // Store in memory cache for faster subsequent access
                    _memoryCache.Set(key, value, _defaultExpiration);
                    
                    _logger.LogDebug("Cache hit in distributed cache for key: {Key}", key);
                    return value;
                }
                catch (JsonException ex)
                {
                    _logger.LogWarning(ex, "Failed to deserialize cached value for key: {Key}", key);
                    await RemoveAsync(key);
                }
            }

            _logger.LogDebug("Cache miss for key: {Key}", key);
            return default;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error getting value from cache with key: {Key}", key);
            return default;
        }
    }

    public async Task SetAsync<T>(string key, T value, TimeSpan? expiration = null)
    {
        try
        {
            if (string.IsNullOrWhiteSpace(key))
                throw new ArgumentException("Cache key cannot be null or empty", nameof(key));

            var expirationTime = expiration ?? _defaultExpiration;
            _logger.LogDebug("Setting value in cache with key: {Key}, expiration: {Expiration}", key, expirationTime);

            // Set in memory cache
            _memoryCache.Set(key, value, expirationTime);

            // Set in distributed cache
            var jsonValue = JsonSerializer.Serialize(value);
            var options = new DistributedCacheEntryOptions
            {
                AbsoluteExpirationRelativeToNow = expirationTime
            };

            await _distributedCache.SetStringAsync(key, jsonValue, options);

            _logger.LogDebug("Successfully set value in cache with key: {Key}", key);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error setting value in cache with key: {Key}", key);
        }
    }

    public async Task RemoveAsync(string key)
    {
        try
        {
            if (string.IsNullOrWhiteSpace(key))
                throw new ArgumentException("Cache key cannot be null or empty", nameof(key));

            _logger.LogDebug("Removing value from cache with key: {Key}", key);

            // Remove from memory cache
            _memoryCache.Remove(key);

            // Remove from distributed cache
            await _distributedCache.RemoveAsync(key);

            _logger.LogDebug("Successfully removed value from cache with key: {Key}", key);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error removing value from cache with key: {Key}", key);
        }
    }

    public async Task RemoveAsync(params string[] keys)
    {
        try
        {
            if (keys == null || keys.Length == 0)
                return;

            _logger.LogDebug("Removing {Count} values from cache", keys.Length);

            var tasks = keys.Select(key => RemoveAsync(key));
            await Task.WhenAll(tasks);

            _logger.LogDebug("Successfully removed {Count} values from cache", keys.Length);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error removing multiple values from cache");
        }
    }

    public async Task<bool> ExistsAsync(string key)
    {
        try
        {
            if (string.IsNullOrWhiteSpace(key))
                return false;

            // Check memory cache first
            if (_memoryCache.TryGetValue(key, out _))
                return true;

            // Check distributed cache
            var value = await _distributedCache.GetStringAsync(key);
            return !string.IsNullOrEmpty(value);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error checking if key exists in cache: {Key}", key);
            return false;
        }
    }

    public async Task<T> GetOrSetAsync<T>(string key, Func<Task<T>> factory, TimeSpan? expiration = null)
    {
        try
        {
            if (string.IsNullOrWhiteSpace(key))
                throw new ArgumentException("Cache key cannot be null or empty", nameof(key));

            if (factory == null)
                throw new ArgumentNullException(nameof(factory));

            // Try to get from cache first
            var cachedValue = await GetAsync<T>(key);
            if (cachedValue != null)
                return cachedValue;

            // If not in cache, execute factory and cache result
            _logger.LogDebug("Cache miss for key: {Key}, executing factory", key);
            var value = await factory();

            if (value != null)
            {
                await SetAsync(key, value, expiration);
            }

            return value;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error in GetOrSet for key: {Key}", key);
            throw;
        }
    }

    public async Task InvalidateByPatternAsync(string pattern)
    {
        try
        {
            if (string.IsNullOrWhiteSpace(pattern))
                return;

            _logger.LogDebug("Invalidating cache by pattern: {Pattern}", pattern);

            // For memory cache, we can't easily search by pattern
            // In a production environment, you might want to maintain a list of keys
            // For now, we'll log this limitation
            _logger.LogWarning("Pattern-based invalidation not fully implemented for memory cache. Pattern: {Pattern}", pattern);

            // For distributed cache, you might implement pattern-based deletion
            // This depends on your specific distributed cache implementation
            // (Redis supports pattern-based deletion, SQL Server doesn't)
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error invalidating cache by pattern: {Pattern}", pattern);
        }
    }
}