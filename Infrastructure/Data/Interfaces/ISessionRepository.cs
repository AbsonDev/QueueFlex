using QueueManagement.Domain.Entities;

namespace QueueManagement.Infrastructure.Data.Interfaces;

/// <summary>
/// Repository interface for Session entity operations
/// </summary>
public interface ISessionRepository : IGenericRepository<Session>
{
    /// <summary>
    /// Get active session by user
    /// </summary>
    Task<Session?> GetActiveSessionByUserAsync(Guid tenantId, Guid userId);

    /// <summary>
    /// Get active sessions for a tenant
    /// </summary>
    Task<List<Session>> GetActiveSessionsAsync(Guid tenantId, Guid? unitId = null);

    /// <summary>
    /// Get session by ticket
    /// </summary>
    Task<Session?> GetByTicketAsync(Guid tenantId, Guid ticketId);

    /// <summary>
    /// Get user sessions with optional date filtering
    /// </summary>
    Task<List<Session>> GetUserSessionsAsync(Guid tenantId, Guid userId, DateTime? fromDate = null);

    /// <summary>
    /// Get completed sessions in date range
    /// </summary>
    Task<List<Session>> GetCompletedSessionsAsync(Guid tenantId, DateTime fromDate, DateTime toDate);

    /// <summary>
    /// Get average session duration
    /// </summary>
    Task<double> GetAverageSessionDurationAsync(Guid tenantId, Guid? userId = null, DateTime? fromDate = null);

    /// <summary>
    /// Get sessions by resource
    /// </summary>
    Task<List<Session>> GetSessionsByResourceAsync(Guid tenantId, Guid resourceId);
}