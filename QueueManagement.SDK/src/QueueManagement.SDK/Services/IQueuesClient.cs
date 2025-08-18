using QueueManagement.SDK.Models.Common;
using QueueManagement.SDK.Models.Queues;

namespace QueueManagement.SDK.Services;

/// <summary>
/// Client for managing queues.
/// </summary>
public interface IQueuesClient
{
    /// <summary>
    /// Creates a new queue.
    /// </summary>
    Task<QueueResponse> CreateAsync(CreateQueueRequest request, CancellationToken cancellationToken = default);

    /// <summary>
    /// Gets a queue by ID.
    /// </summary>
    Task<QueueResponse> GetAsync(Guid queueId, CancellationToken cancellationToken = default);

    /// <summary>
    /// Updates an existing queue.
    /// </summary>
    Task<QueueResponse> UpdateAsync(Guid queueId, UpdateQueueRequest request, CancellationToken cancellationToken = default);

    /// <summary>
    /// Deletes a queue.
    /// </summary>
    Task DeleteAsync(Guid queueId, CancellationToken cancellationToken = default);

    /// <summary>
    /// Gets the status of a queue.
    /// </summary>
    Task<QueueStatusResponse> GetStatusAsync(Guid queueId, CancellationToken cancellationToken = default);

    /// <summary>
    /// Updates the status of a queue.
    /// </summary>
    Task<QueueResponse> UpdateStatusAsync(Guid queueId, QueueStatus status, CancellationToken cancellationToken = default);

    /// <summary>
    /// Gets metrics for a queue.
    /// </summary>
    Task<QueueMetricsResponse> GetMetricsAsync(Guid queueId, CancellationToken cancellationToken = default);

    /// <summary>
    /// Gets all queues in a unit.
    /// </summary>
    Task<List<QueueResponse>> GetByUnitAsync(Guid unitId, CancellationToken cancellationToken = default);

    /// <summary>
    /// Gets all queues.
    /// </summary>
    Task<List<QueueResponse>> GetAllAsync(CancellationToken cancellationToken = default);

    /// <summary>
    /// Gets paginated queues.
    /// </summary>
    Task<PagedResult<QueueResponse>> GetPagedAsync(PagedRequest request, CancellationToken cancellationToken = default);

    /// <summary>
    /// Opens a queue.
    /// </summary>
    Task<QueueResponse> OpenAsync(Guid queueId, CancellationToken cancellationToken = default);

    /// <summary>
    /// Closes a queue.
    /// </summary>
    Task<QueueResponse> CloseAsync(Guid queueId, CancellationToken cancellationToken = default);

    /// <summary>
    /// Pauses a queue.
    /// </summary>
    Task<QueueResponse> PauseAsync(Guid queueId, CancellationToken cancellationToken = default);

    /// <summary>
    /// Resumes a paused queue.
    /// </summary>
    Task<QueueResponse> ResumeAsync(Guid queueId, CancellationToken cancellationToken = default);

    /// <summary>
    /// Clears all waiting tickets from a queue.
    /// </summary>
    Task ClearAsync(Guid queueId, CancellationToken cancellationToken = default);

    /// <summary>
    /// Gets the estimated wait time for a queue.
    /// </summary>
    Task<TimeSpan> GetEstimatedWaitTimeAsync(Guid queueId, CancellationToken cancellationToken = default);
}