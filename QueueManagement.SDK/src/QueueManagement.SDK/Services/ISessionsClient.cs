using QueueManagement.SDK.Models.Common;
using QueueManagement.SDK.Models.Sessions;

namespace QueueManagement.SDK.Services;

/// <summary>
/// Client for managing service sessions.
/// </summary>
public interface ISessionsClient
{
    /// <summary>
    /// Starts a new session.
    /// </summary>
    Task<SessionResponse> StartAsync(StartSessionRequest request, CancellationToken cancellationToken = default);

    /// <summary>
    /// Gets a session by ID.
    /// </summary>
    Task<SessionResponse> GetAsync(Guid sessionId, CancellationToken cancellationToken = default);

    /// <summary>
    /// Completes a session.
    /// </summary>
    Task<SessionResponse> CompleteAsync(Guid sessionId, CompleteSessionRequest? request = null, CancellationToken cancellationToken = default);

    /// <summary>
    /// Pauses a session.
    /// </summary>
    Task<SessionResponse> PauseAsync(Guid sessionId, PauseSessionRequest? request = null, CancellationToken cancellationToken = default);

    /// <summary>
    /// Resumes a paused session.
    /// </summary>
    Task<SessionResponse> ResumeAsync(Guid sessionId, CancellationToken cancellationToken = default);

    /// <summary>
    /// Gets the active session for a user.
    /// </summary>
    Task<SessionResponse?> GetActiveByUserAsync(Guid userId, CancellationToken cancellationToken = default);

    /// <summary>
    /// Gets all active sessions.
    /// </summary>
    Task<List<SessionResponse>> GetActiveSessionsAsync(Guid? unitId = null, CancellationToken cancellationToken = default);

    /// <summary>
    /// Gets session statistics.
    /// </summary>
    Task<SessionStatistics> GetStatisticsAsync(Guid sessionId, CancellationToken cancellationToken = default);

    /// <summary>
    /// Gets sessions by unit.
    /// </summary>
    Task<List<SessionResponse>> GetByUnitAsync(Guid unitId, SessionStatus? status = null, CancellationToken cancellationToken = default);

    /// <summary>
    /// Gets sessions by user.
    /// </summary>
    Task<List<SessionResponse>> GetByUserAsync(Guid userId, DateTime? fromDate = null, DateTime? toDate = null, CancellationToken cancellationToken = default);

    /// <summary>
    /// Gets paginated sessions.
    /// </summary>
    Task<PagedResult<SessionResponse>> GetPagedAsync(PagedRequest request, CancellationToken cancellationToken = default);

    /// <summary>
    /// Calls the next ticket in the session's queues.
    /// </summary>
    Task<Models.Tickets.TicketResponse?> CallNextTicketAsync(Guid sessionId, CancellationToken cancellationToken = default);

    /// <summary>
    /// Completes the current ticket being served.
    /// </summary>
    Task<Models.Tickets.TicketResponse> CompleteCurrentTicketAsync(Guid sessionId, CancellationToken cancellationToken = default);

    /// <summary>
    /// Transfers the current ticket to another queue.
    /// </summary>
    Task<Models.Tickets.TicketResponse> TransferCurrentTicketAsync(Guid sessionId, Models.Tickets.TransferTicketRequest request, CancellationToken cancellationToken = default);

    /// <summary>
    /// Updates session queues.
    /// </summary>
    Task<SessionResponse> UpdateQueuesAsync(Guid sessionId, List<Guid> queueIds, CancellationToken cancellationToken = default);

    /// <summary>
    /// Updates session services.
    /// </summary>
    Task<SessionResponse> UpdateServicesAsync(Guid sessionId, List<Guid> serviceIds, CancellationToken cancellationToken = default);
}