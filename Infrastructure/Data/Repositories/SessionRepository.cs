using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using QueueManagement.Domain.Entities;
using QueueManagement.Infrastructure.Data.Interfaces;

namespace QueueManagement.Infrastructure.Data.Repositories;

/// <summary>
/// Repository implementation for Session entity with optimized queries
/// </summary>
public class SessionRepository : GenericRepository<Session>, ISessionRepository
{
    public SessionRepository(QueueManagementDbContext context, ILogger<SessionRepository> logger) 
        : base(context, logger) { }

    public async Task<Session?> GetActiveSessionByUserAsync(Guid tenantId, Guid userId)
    {
        try
        {
            _logger.LogDebug("Getting active session by user {UserId} for tenant {TenantId}", userId, tenantId);

            return await _dbSet
                .Include(s => s.User)
                .Include(s => s.Ticket)
                    .ThenInclude(t => t.Queue)
                        .ThenInclude(q => q.Unit)
                .Include(s => s.Resource)
                .AsNoTracking()
                .Where(s => s.UserId == userId && 
                           s.User.Unit.TenantId == tenantId && 
                           s.Status == Domain.Enums.SessionStatus.Active && 
                           !s.IsDeleted)
                .FirstOrDefaultAsync();
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error getting active session by user {UserId} for tenant {TenantId}", userId, tenantId);
            throw;
        }
    }

    public async Task<List<Session>> GetActiveSessionsAsync(Guid tenantId, Guid? unitId = null)
    {
        try
        {
            _logger.LogDebug("Getting active sessions for tenant {TenantId} and unit {UnitId}", tenantId, unitId);

            var query = _dbSet
                .Include(s => s.User)
                .Include(s => s.Ticket)
                    .ThenInclude(t => t.Queue)
                        .ThenInclude(q => q.Unit)
                .Include(s => s.Resource)
                .AsNoTracking()
                .Where(s => s.User.Unit.TenantId == tenantId && 
                           s.Status == Domain.Enums.SessionStatus.Active && 
                           !s.IsDeleted);

            if (unitId.HasValue)
            {
                query = query.Where(s => s.User.UnitId == unitId.Value);
            }

            return await query
                .OrderBy(s => s.User.Unit.Name)
                .ThenBy(s => s.User.Name)
                .ThenBy(s => s.StartedAt)
                .ToListAsync();
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error getting active sessions for tenant {TenantId}", tenantId);
            throw;
        }
    }

    public async Task<Session?> GetByTicketAsync(Guid tenantId, Guid ticketId)
    {
        try
        {
            _logger.LogDebug("Getting session by ticket {TicketId} for tenant {TenantId}", ticketId, tenantId);

            return await _dbSet
                .Include(s => s.User)
                .Include(s => s.Ticket)
                    .ThenInclude(t => t.Queue)
                        .ThenInclude(q => q.Unit)
                .Include(s => s.Resource)
                .AsNoTracking()
                .Where(s => s.TicketId == ticketId && 
                           s.Ticket.Queue.Unit.TenantId == tenantId && 
                           !s.IsDeleted)
                .OrderByDescending(s => s.StartedAt)
                .FirstOrDefaultAsync();
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error getting session by ticket {TicketId} for tenant {TenantId}", ticketId, tenantId);
            throw;
        }
    }

    public async Task<List<Session>> GetUserSessionsAsync(Guid tenantId, Guid userId, DateTime? fromDate = null)
    {
        try
        {
            _logger.LogDebug("Getting user sessions for user {UserId} and tenant {TenantId} from date {FromDate}", 
                userId, tenantId, fromDate);

            var query = _dbSet
                .Include(s => s.Ticket)
                    .ThenInclude(t => t.Queue)
                        .ThenInclude(q => q.Unit)
                .Include(s => s.Resource)
                .AsNoTracking()
                .Where(s => s.UserId == userId && 
                           s.User.Unit.TenantId == tenantId && 
                           !s.IsDeleted);

            if (fromDate.HasValue)
            {
                query = query.Where(s => s.StartedAt >= fromDate.Value);
            }

            return await query
                .OrderByDescending(s => s.StartedAt)
                .Take(100) // Limit to last 100 sessions for performance
                .ToListAsync();
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error getting user sessions for user {UserId} and tenant {TenantId}", userId, tenantId);
            throw;
        }
    }

    public async Task<List<Session>> GetCompletedSessionsAsync(Guid tenantId, DateTime fromDate, DateTime toDate)
    {
        try
        {
            _logger.LogDebug("Getting completed sessions for tenant {TenantId} from {FromDate} to {ToDate}", 
                tenantId, fromDate, toDate);

            return await _dbSet
                .Include(s => s.User)
                .Include(s => s.Ticket)
                    .ThenInclude(t => t.Queue)
                        .ThenInclude(q => q.Unit)
                .Include(s => s.Resource)
                .AsNoTracking()
                .Where(s => s.User.Unit.TenantId == tenantId && 
                           s.Status == Domain.Enums.SessionStatus.Completed && 
                           s.StartedAt >= fromDate && 
                           s.StartedAt <= toDate && 
                           !s.IsDeleted)
                .OrderByDescending(s => s.StartedAt)
                .ToListAsync();
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error getting completed sessions for tenant {TenantId} from {FromDate} to {ToDate}", 
                tenantId, fromDate, toDate);
            throw;
        }
    }

    public async Task<double> GetAverageSessionDurationAsync(Guid tenantId, Guid? userId = null, DateTime? fromDate = null)
    {
        try
        {
            _logger.LogDebug("Getting average session duration for tenant {TenantId}, user {UserId}, from date {FromDate}", 
                tenantId, userId, fromDate);

            var query = _dbSet
                .AsNoTracking()
                .Where(s => s.User.Unit.TenantId == tenantId && 
                           s.Status == Domain.Enums.SessionStatus.Completed && 
                           s.StartedAt != null && 
                           s.EndedAt != null && 
                           !s.IsDeleted);

            if (userId.HasValue)
            {
                query = query.Where(s => s.UserId == userId.Value);
            }

            if (fromDate.HasValue)
            {
                query = query.Where(s => s.StartedAt >= fromDate.Value);
            }

            var durations = await query
                .Select(s => EF.Functions.DateDiffMinute(s.StartedAt, s.EndedAt))
                .ToListAsync();

            return durations.Any() ? durations.Average() : 0;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error getting average session duration for tenant {TenantId}", tenantId);
            throw;
        }
    }

    public async Task<List<Session>> GetSessionsByResourceAsync(Guid tenantId, Guid resourceId)
    {
        try
        {
            _logger.LogDebug("Getting sessions by resource {ResourceId} for tenant {TenantId}", resourceId, tenantId);

            return await _dbSet
                .Include(s => s.User)
                .Include(s => s.Ticket)
                    .ThenInclude(t => t.Queue)
                        .ThenInclude(q => q.Unit)
                .Include(s => s.Resource)
                .AsNoTracking()
                .Where(s => s.ResourceId == resourceId && 
                           s.Resource.Unit.TenantId == tenantId && 
                           !s.IsDeleted)
                .OrderByDescending(s => s.StartedAt)
                .Take(100) // Limit to last 100 sessions for performance
                .ToListAsync();
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error getting sessions by resource {ResourceId} for tenant {TenantId}", resourceId, tenantId);
            throw;
        }
    }
}