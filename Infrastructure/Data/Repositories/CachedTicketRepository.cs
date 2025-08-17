using Microsoft.Extensions.Logging;
using QueueManagement.Domain.Entities;
using QueueManagement.Domain.Enums;
using QueueManagement.Infrastructure.Data.Interfaces;
using QueueManagement.Infrastructure.Data.Caching;

namespace QueueManagement.Infrastructure.Data.Repositories;

/// <summary>
/// Cached ticket repository implementation demonstrating cache integration
/// </summary>
public class CachedTicketRepository : ITicketRepository
{
    private readonly ITicketRepository _ticketRepository;
    private readonly ICacheService _cacheService;
    private readonly ILogger<CachedTicketRepository> _logger;
    private readonly TimeSpan _defaultCacheExpiration = TimeSpan.FromMinutes(15);

    public CachedTicketRepository(
        ITicketRepository ticketRepository,
        ICacheService cacheService,
        ILogger<CachedTicketRepository> logger)
    {
        _ticketRepository = ticketRepository ?? throw new ArgumentNullException(nameof(ticketRepository));
        _cacheService = cacheService ?? throw new ArgumentNullException(nameof(cacheService));
        _logger = logger ?? throw new ArgumentNullException(nameof(logger));
    }

    public async Task<Ticket?> GetByIdAsync(Guid tenantId, Guid id)
    {
        var cacheKey = $"ticket:{tenantId}:{id}";
        return await _cacheService.GetOrSetAsync(cacheKey, 
            () => _ticketRepository.GetByIdAsync(tenantId, id), 
            _defaultCacheExpiration);
    }

    public async Task<Ticket?> GetByIdAsync(Guid id)
    {
        var cacheKey = $"ticket:{id}";
        return await _cacheService.GetOrSetAsync(cacheKey, 
            () => _ticketRepository.GetByIdAsync(id), 
            _defaultCacheExpiration);
    }

    public async Task<List<Ticket>> GetAllAsync(Guid tenantId)
    {
        var cacheKey = $"tickets:all:{tenantId}";
        return await _cacheService.GetOrSetAsync(cacheKey, 
            () => _ticketRepository.GetAllAsync(tenantId), 
            _defaultCacheExpiration);
    }

    public async Task<List<Ticket>> GetPagedAsync(Guid tenantId, int page, int pageSize)
    {
        var cacheKey = $"tickets:paged:{tenantId}:{page}:{pageSize}";
        return await _cacheService.GetOrSetAsync(cacheKey, 
            () => _ticketRepository.GetPagedAsync(tenantId, page, pageSize), 
            TimeSpan.FromMinutes(5)); // Shorter cache for paged results
    }

    public async Task<Ticket> AddAsync(Ticket entity)
    {
        var result = await _ticketRepository.AddAsync(entity);
        await InvalidateTicketCache(entity.TenantId);
        return result;
    }

    public async Task<List<Ticket>> AddRangeAsync(List<Ticket> entities)
    {
        var result = await _ticketRepository.AddRangeAsync(entities);
        if (entities.Any())
        {
            var tenantId = entities.First().TenantId;
            await InvalidateTicketCache(tenantId);
        }
        return result;
    }

    public async Task UpdateAsync(Ticket entity)
    {
        await _ticketRepository.UpdateAsync(entity);
        await InvalidateTicketCache(entity.TenantId);
    }

    public async Task DeleteAsync(Ticket entity)
    {
        await _ticketRepository.DeleteAsync(entity);
        await InvalidateTicketCache(entity.TenantId);
    }

    public async Task<int> CountAsync(Guid tenantId)
    {
        var cacheKey = $"tickets:count:{tenantId}";
        return await _cacheService.GetOrSetAsync(cacheKey, 
            () => _ticketRepository.CountAsync(tenantId), 
            TimeSpan.FromMinutes(10));
    }

    public async Task<bool> ExistsAsync(Guid tenantId, Guid id)
    {
        var cacheKey = $"tickets:exists:{tenantId}:{id}";
        return await _cacheService.GetOrSetAsync(cacheKey, 
            () => _ticketRepository.ExistsAsync(tenantId, id), 
            TimeSpan.FromMinutes(10));
    }

    public async Task<Ticket?> GetByNumberAsync(Guid tenantId, string number)
    {
        var cacheKey = $"ticket:number:{tenantId}:{number}";
        return await _cacheService.GetOrSetAsync(cacheKey, 
            () => _ticketRepository.GetByNumberAsync(tenantId, number), 
            _defaultCacheExpiration);
    }

    public async Task<List<Ticket>> GetByQueueAsync(Guid tenantId, Guid queueId, TicketStatus? status = null)
    {
        var statusSuffix = status?.ToString() ?? "all";
        var cacheKey = $"tickets:queue:{tenantId}:{queueId}:{statusSuffix}";
        return await _cacheService.GetOrSetAsync(cacheKey, 
            () => _ticketRepository.GetByQueueAsync(tenantId, queueId, status), 
            TimeSpan.FromMinutes(5)); // Shorter cache for queue-specific data
    }

    public async Task<int> GetWaitingCountAsync(Guid queueId)
    {
        var cacheKey = $"tickets:waiting:count:{queueId}";
        return await _cacheService.GetOrSetAsync(cacheKey, 
            () => _ticketRepository.GetWaitingCountAsync(queueId), 
            TimeSpan.FromMinutes(2)); // Very short cache for real-time data
    }

    public async Task<int> GetQueuePositionAsync(Guid ticketId)
    {
        var cacheKey = $"tickets:position:{ticketId}";
        return await _cacheService.GetOrSetAsync(cacheKey, 
            () => _ticketRepository.GetQueuePositionAsync(ticketId), 
            TimeSpan.FromMinutes(1)); // Very short cache for position data
    }

    public async Task<Ticket?> GetNextInQueueAsync(Guid queueId)
    {
        var cacheKey = $"tickets:next:{queueId}";
        return await _cacheService.GetOrSetAsync(cacheKey, 
            () => _ticketRepository.GetNextInQueueAsync(queueId), 
            TimeSpan.FromMinutes(1)); // Very short cache for next ticket
    }

    public async Task<List<Ticket>> GetTicketHistoryAsync(Guid tenantId, string? customerDocument)
    {
        var documentSuffix = customerDocument ?? "all";
        var cacheKey = $"tickets:history:{tenantId}:{documentSuffix}";
        return await _cacheService.GetOrSetAsync(cacheKey, 
            () => _ticketRepository.GetTicketHistoryAsync(tenantId, customerDocument), 
            TimeSpan.FromMinutes(30)); // Longer cache for historical data
    }

    public async Task<int> GetDailyCountAsync(Guid queueId, DateTime date)
    {
        var dateString = date.ToString("yyyy-MM-dd");
        var cacheKey = $"tickets:daily:{queueId}:{dateString}";
        return await _cacheService.GetOrSetAsync(cacheKey, 
            () => _ticketRepository.GetDailyCountAsync(queueId, date), 
            TimeSpan.FromHours(1)); // Cache for a day
    }

    public async Task<List<Ticket>> GetByCustomerAsync(Guid tenantId, string customerDocument)
    {
        var cacheKey = $"tickets:customer:{tenantId}:{customerDocument}";
        return await _cacheService.GetOrSetAsync(cacheKey, 
            () => _ticketRepository.GetByCustomerAsync(tenantId, customerDocument), 
            _defaultCacheExpiration);
    }

    public async Task<double> GetAverageWaitTimeAsync(Guid queueId, DateTime? fromDate = null)
    {
        var dateSuffix = fromDate?.ToString("yyyy-MM-dd") ?? "all";
        var cacheKey = $"tickets:avgwaittime:{queueId}:{dateSuffix}";
        return await _cacheService.GetOrSetAsync(cacheKey, 
            () => _ticketRepository.GetAverageWaitTimeAsync(queueId, fromDate), 
            TimeSpan.FromMinutes(30));
    }

    public async Task<List<Ticket>> GetActiveTicketsAsync(Guid tenantId, Guid? unitId = null)
    {
        var unitSuffix = unitId?.ToString() ?? "all";
        var cacheKey = $"tickets:active:{tenantId}:{unitSuffix}";
        return await _cacheService.GetOrSetAsync(cacheKey, 
            () => _ticketRepository.GetActiveTicketsAsync(tenantId, unitId), 
            TimeSpan.FromMinutes(5)); // Shorter cache for active data
    }

    /// <summary>
    /// Invalidate all ticket-related cache for a specific tenant
    /// </summary>
    private async Task InvalidateTicketCache(Guid tenantId)
    {
        try
        {
            var patterns = new[]
            {
                $"ticket:{tenantId}:*",
                $"tickets:all:{tenantId}",
                $"tickets:paged:{tenantId}:*",
                $"tickets:count:{tenantId}",
                $"tickets:exists:{tenantId}:*",
                $"tickets:queue:{tenantId}:*",
                $"tickets:history:{tenantId}:*",
                $"tickets:customer:{tenantId}:*",
                $"tickets:active:{tenantId}:*"
            };

            foreach (var pattern in patterns)
            {
                await _cacheService.InvalidateByPatternAsync(pattern);
            }

            _logger.LogDebug("Invalidated ticket cache for tenant {TenantId}", tenantId);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error invalidating ticket cache for tenant {TenantId}", tenantId);
        }
    }
}