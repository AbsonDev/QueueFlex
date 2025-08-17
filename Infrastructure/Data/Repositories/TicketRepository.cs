using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using QueueManagement.Domain.Entities;
using QueueManagement.Domain.Enums;
using QueueManagement.Infrastructure.Data.Interfaces;

namespace QueueManagement.Infrastructure.Data.Repositories;

/// <summary>
/// Repository implementation for Ticket entity with optimized queries
/// </summary>
public class TicketRepository : GenericRepository<Ticket>, ITicketRepository
{
    public TicketRepository(QueueManagementDbContext context, ILogger<TicketRepository> logger) 
        : base(context, logger) { }

    public async Task<Ticket?> GetByNumberAsync(Guid tenantId, string number)
    {
        try
        {
            _logger.LogDebug("Getting ticket by number {Number} for tenant {TenantId}", number, tenantId);

            return await _dbSet
                .Include(t => t.Queue)
                    .ThenInclude(q => q.Unit)
                .Include(t => t.Service)
                .AsNoTracking()
                .Where(t => t.Number == number && t.Queue.Unit.TenantId == tenantId && !t.IsDeleted)
                .FirstOrDefaultAsync();
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error getting ticket by number {Number} for tenant {TenantId}", number, tenantId);
            throw;
        }
    }

    public async Task<List<Ticket>> GetByQueueAsync(Guid tenantId, Guid queueId, TicketStatus? status = null)
    {
        try
        {
            _logger.LogDebug("Getting tickets by queue {QueueId} for tenant {TenantId} with status {Status}", 
                queueId, tenantId, status);

            var query = _dbSet
                .Include(t => t.Service)
                .Include(t => t.Sessions)
                .AsNoTracking()
                .Where(t => t.QueueId == queueId && t.Queue.Unit.TenantId == tenantId && !t.IsDeleted);

            if (status.HasValue)
            {
                query = query.Where(t => t.Status == status.Value);
            }

            return await query
                .OrderBy(t => t.Priority)
                .ThenBy(t => t.IssuedAt)
                .ToListAsync();
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error getting tickets by queue {QueueId} for tenant {TenantId}", queueId, tenantId);
            throw;
        }
    }

    public async Task<int> GetWaitingCountAsync(Guid queueId)
    {
        try
        {
            _logger.LogDebug("Getting waiting count for queue {QueueId}", queueId);

            return await _dbSet
                .AsNoTracking()
                .CountAsync(t => t.QueueId == queueId && t.Status == TicketStatus.Waiting && !t.IsDeleted);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error getting waiting count for queue {QueueId}", queueId);
            throw;
        }
    }

    public async Task<int> GetQueuePositionAsync(Guid ticketId)
    {
        try
        {
            _logger.LogDebug("Getting queue position for ticket {TicketId}", ticketId);

            var ticket = await _dbSet
                .AsNoTracking()
                .Where(t => t.Id == ticketId && !t.IsDeleted)
                .Select(t => new { t.QueueId, t.Priority, t.IssuedAt })
                .FirstOrDefaultAsync();

            if (ticket == null) return 0;

            return await _dbSet
                .AsNoTracking()
                .CountAsync(t => t.QueueId == ticket.QueueId 
                               && t.Status == TicketStatus.Waiting 
                               && !t.IsDeleted
                               && (t.Priority > ticket.Priority 
                                   || (t.Priority == ticket.Priority && t.IssuedAt < ticket.IssuedAt))) + 1;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error getting queue position for ticket {TicketId}", ticketId);
            throw;
        }
    }

    public async Task<Ticket?> GetNextInQueueAsync(Guid queueId)
    {
        try
        {
            _logger.LogDebug("Getting next ticket in queue {QueueId}", queueId);

            return await _dbSet
                .Include(t => t.Service)
                .AsNoTracking()
                .Where(t => t.QueueId == queueId && t.Status == TicketStatus.Waiting && !t.IsDeleted)
                .OrderBy(t => t.Priority)
                .ThenBy(t => t.IssuedAt)
                .FirstOrDefaultAsync();
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error getting next ticket in queue {QueueId}", queueId);
            throw;
        }
    }

    public async Task<List<Ticket>> GetTicketHistoryAsync(Guid tenantId, string? customerDocument)
    {
        try
        {
            _logger.LogDebug("Getting ticket history for tenant {TenantId} with customer document {CustomerDocument}", 
                tenantId, customerDocument);

            var query = _dbSet
                .Include(t => t.Queue)
                    .ThenInclude(q => q.Unit)
                .Include(t => t.Service)
                .AsNoTracking()
                .Where(t => t.Queue.Unit.TenantId == tenantId && !t.IsDeleted);

            if (!string.IsNullOrEmpty(customerDocument))
            {
                query = query.Where(t => t.CustomerDocument == customerDocument);
            }

            return await query
                .OrderByDescending(t => t.IssuedAt)
                .Take(100) // Limit to last 100 tickets for performance
                .ToListAsync();
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error getting ticket history for tenant {TenantId}", tenantId);
            throw;
        }
    }

    public async Task<int> GetDailyCountAsync(Guid queueId, DateTime date)
    {
        try
        {
            _logger.LogDebug("Getting daily count for queue {QueueId} on date {Date}", queueId, date);

            var startOfDay = date.Date;
            var endOfDay = startOfDay.AddDays(1);

            return await _dbSet
                .AsNoTracking()
                .CountAsync(t => t.QueueId == queueId 
                               && t.IssuedAt >= startOfDay 
                               && t.IssuedAt < endOfDay 
                               && !t.IsDeleted);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error getting daily count for queue {QueueId} on date {Date}", queueId, date);
            throw;
        }
    }

    public async Task<List<Ticket>> GetByCustomerAsync(Guid tenantId, string customerDocument)
    {
        try
        {
            _logger.LogDebug("Getting tickets by customer {CustomerDocument} for tenant {TenantId}", 
                customerDocument, tenantId);

            return await _dbSet
                .Include(t => t.Queue)
                    .ThenInclude(q => q.Unit)
                .Include(t => t.Service)
                .AsNoTracking()
                .Where(t => t.CustomerDocument == customerDocument 
                           && t.Queue.Unit.TenantId == tenantId 
                           && !t.IsDeleted)
                .OrderByDescending(t => t.IssuedAt)
                .ToListAsync();
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error getting tickets by customer {CustomerDocument} for tenant {TenantId}", 
                customerDocument, tenantId);
            throw;
        }
    }

    public async Task<double> GetAverageWaitTimeAsync(Guid queueId, DateTime? fromDate = null)
    {
        try
        {
            _logger.LogDebug("Getting average wait time for queue {QueueId} from date {FromDate}", queueId, fromDate);

            var query = _dbSet
                .AsNoTracking()
                .Where(t => t.QueueId == queueId 
                           && t.Status == TicketStatus.Completed 
                           && t.CalledAt.HasValue 
                           && t.IssuedAt != null
                           && !t.IsDeleted);

            if (fromDate.HasValue)
            {
                query = query.Where(t => t.IssuedAt >= fromDate.Value);
            }

            var waitTimes = await query
                .Select(t => EF.Functions.DateDiffMinute(t.IssuedAt, t.CalledAt.Value))
                .ToListAsync();

            return waitTimes.Any() ? waitTimes.Average() : 0;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error getting average wait time for queue {QueueId}", queueId);
            throw;
        }
    }

    public async Task<List<Ticket>> GetActiveTicketsAsync(Guid tenantId, Guid? unitId = null)
    {
        try
        {
            _logger.LogDebug("Getting active tickets for tenant {TenantId} and unit {UnitId}", tenantId, unitId);

            var query = _dbSet
                .Include(t => t.Queue)
                    .ThenInclude(q => q.Unit)
                .Include(t => t.Service)
                .AsNoTracking()
                .Where(t => t.Queue.Unit.TenantId == tenantId 
                           && t.Status != TicketStatus.Completed 
                           && t.Status != TicketStatus.Cancelled
                           && !t.IsDeleted);

            if (unitId.HasValue)
            {
                query = query.Where(t => t.Queue.UnitId == unitId.Value);
            }

            return await query
                .OrderBy(t => t.Queue.Unit.Name)
                .ThenBy(t => t.Queue.Name)
                .ThenBy(t => t.Priority)
                .ThenBy(t => t.IssuedAt)
                .ToListAsync();
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error getting active tickets for tenant {TenantId}", tenantId);
            throw;
        }
    }
}