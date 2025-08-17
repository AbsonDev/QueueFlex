using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using QueueManagement.Domain.Entities;
using QueueManagement.Infrastructure.Data.Interfaces;

namespace QueueManagement.Infrastructure.Data.Repositories;

/// <summary>
/// Repository implementation for Queue entity with optimized queries
/// </summary>
public class QueueRepository : GenericRepository<Queue>, IQueueRepository
{
    public QueueRepository(QueueManagementDbContext context, ILogger<QueueRepository> logger) 
        : base(context, logger) { }

    public async Task<List<Queue>> GetByUnitAsync(Guid tenantId, Guid unitId)
    {
        try
        {
            _logger.LogDebug("Getting queues by unit {UnitId} for tenant {TenantId}", unitId, tenantId);

            return await _dbSet
                .Include(q => q.Unit)
                .Include(q => q.Services)
                .AsNoTracking()
                .Where(q => q.UnitId == unitId && q.Unit.TenantId == tenantId && !q.IsDeleted)
                .OrderBy(q => q.Name)
                .ToListAsync();
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error getting queues by unit {UnitId} for tenant {TenantId}", unitId, tenantId);
            throw;
        }
    }

    public async Task<Queue?> GetWithServicesAsync(Guid tenantId, Guid queueId)
    {
        try
        {
            _logger.LogDebug("Getting queue {QueueId} with services for tenant {TenantId}", queueId, tenantId);

            return await _dbSet
                .Include(q => q.Unit)
                .Include(q => q.Services)
                .Include(q => q.Tickets.Where(t => !t.IsDeleted))
                .AsNoTracking()
                .Where(q => q.Id == queueId && q.Unit.TenantId == tenantId && !q.IsDeleted)
                .FirstOrDefaultAsync();
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error getting queue {QueueId} with services for tenant {TenantId}", queueId, tenantId);
            throw;
        }
    }

    public async Task<Queue?> GetByCodeAsync(Guid tenantId, Guid unitId, string code)
    {
        try
        {
            _logger.LogDebug("Getting queue by code {Code} for unit {UnitId} and tenant {TenantId}", 
                code, unitId, tenantId);

            return await _dbSet
                .Include(q => q.Unit)
                .Include(q => q.Services)
                .AsNoTracking()
                .Where(q => q.Code == code && q.UnitId == unitId && q.Unit.TenantId == tenantId && !q.IsDeleted)
                .FirstOrDefaultAsync();
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error getting queue by code {Code} for unit {UnitId} and tenant {TenantId}", 
                code, unitId, tenantId);
            throw;
        }
    }

    public async Task<List<Queue>> GetActiveQueuesAsync(Guid tenantId, Guid? unitId = null)
    {
        try
        {
            _logger.LogDebug("Getting active queues for tenant {TenantId} and unit {UnitId}", tenantId, unitId);

            var query = _dbSet
                .Include(q => q.Unit)
                .Include(q => q.Services)
                .AsNoTracking()
                .Where(q => q.Unit.TenantId == tenantId && q.IsActive && !q.IsDeleted);

            if (unitId.HasValue)
            {
                query = query.Where(q => q.UnitId == unitId.Value);
            }

            return await query
                .OrderBy(q => q.Unit.Name)
                .ThenBy(q => q.Name)
                .ToListAsync();
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error getting active queues for tenant {TenantId}", tenantId);
            throw;
        }
    }

    public async Task<QueueStatusDto> GetRealTimeStatusAsync(Guid tenantId, Guid queueId)
    {
        try
        {
            _logger.LogDebug("Getting real-time status for queue {QueueId} and tenant {TenantId}", queueId, tenantId);

            var queue = await _dbSet
                .Include(q => q.Tickets.Where(t => !t.IsDeleted))
                .AsNoTracking()
                .Where(q => q.Id == queueId && q.Unit.TenantId == tenantId && !q.IsDeleted)
                .FirstOrDefaultAsync();

            if (queue == null)
                throw new InvalidOperationException($"Queue {queueId} not found for tenant {tenantId}");

            var waitingCount = queue.Tickets.Count(t => t.Status == Domain.Enums.TicketStatus.Waiting);
            var inServiceCount = queue.Tickets.Count(t => t.Status == Domain.Enums.TicketStatus.InService);
            
            var today = DateTime.UtcNow.Date;
            var completedToday = queue.Tickets.Count(t => 
                t.Status == Domain.Enums.TicketStatus.Completed && 
                t.CompletedAt.HasValue && 
                t.CompletedAt.Value.Date == today);

            // Calculate average wait time for completed tickets in the last 7 days
            var lastWeek = DateTime.UtcNow.AddDays(-7);
            var completedTickets = queue.Tickets.Where(t => 
                t.Status == Domain.Enums.TicketStatus.Completed && 
                t.CalledAt.HasValue && 
                t.IssuedAt != null &&
                t.IssuedAt >= lastWeek);

            double averageWaitTime = 0;
            if (completedTickets.Any())
            {
                var waitTimes = completedTickets.Select(t => 
                    (t.CalledAt.Value - t.IssuedAt).TotalMinutes);
                averageWaitTime = waitTimes.Average();
            }

            return new QueueStatusDto
            {
                QueueId = queue.Id,
                QueueName = queue.Name,
                WaitingCount = waitingCount,
                InServiceCount = inServiceCount,
                CompletedToday = completedToday,
                AverageWaitTime = averageWaitTime,
                LastUpdate = DateTime.UtcNow
            };
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error getting real-time status for queue {QueueId} and tenant {TenantId}", 
                queueId, tenantId);
            throw;
        }
    }

    public async Task<List<Queue>> SearchAsync(Guid tenantId, string searchTerm)
    {
        try
        {
            _logger.LogDebug("Searching queues with term '{SearchTerm}' for tenant {TenantId}", searchTerm, tenantId);

            if (string.IsNullOrWhiteSpace(searchTerm))
                return await GetActiveQueuesAsync(tenantId);

            var normalizedSearchTerm = searchTerm.ToLowerInvariant();

            return await _dbSet
                .Include(q => q.Unit)
                .Include(q => q.Services)
                .AsNoTracking()
                .Where(q => q.Unit.TenantId == tenantId && !q.IsDeleted &&
                           (q.Name.ToLower().Contains(normalizedSearchTerm) ||
                            q.Code.ToLower().Contains(normalizedSearchTerm) ||
                            q.Description.ToLower().Contains(normalizedSearchTerm)))
                .OrderBy(q => q.Unit.Name)
                .ThenBy(q => q.Name)
                .ToListAsync();
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error searching queues with term '{SearchTerm}' for tenant {TenantId}", 
                searchTerm, tenantId);
            throw;
        }
    }

    public async Task<bool> CodeExistsAsync(Guid tenantId, Guid unitId, string code, Guid? excludeId = null)
    {
        try
        {
            _logger.LogDebug("Checking if queue code {Code} exists for unit {UnitId} and tenant {TenantId}", 
                code, unitId, tenantId);

            var query = _dbSet
                .AsNoTracking()
                .Where(q => q.Code == code && q.UnitId == unitId && q.Unit.TenantId == tenantId && !q.IsDeleted);

            if (excludeId.HasValue)
            {
                query = query.Where(q => q.Id != excludeId.Value);
            }

            return await query.AnyAsync();
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error checking if queue code {Code} exists for unit {UnitId} and tenant {TenantId}", 
                code, unitId, tenantId);
            throw;
        }
    }
}