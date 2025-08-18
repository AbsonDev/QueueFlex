using QueueManagement.SDK.Models.Common;
using QueueManagement.SDK.Models.Tickets;

namespace QueueManagement.SDK.Services;

/// <summary>
/// Client for managing tickets in the queue system.
/// </summary>
public interface ITicketsClient
{
    /// <summary>
    /// Creates a new ticket.
    /// </summary>
    Task<TicketResponse> CreateAsync(CreateTicketRequest request, CancellationToken cancellationToken = default);

    /// <summary>
    /// Gets a ticket by ID.
    /// </summary>
    Task<TicketResponse> GetAsync(Guid ticketId, CancellationToken cancellationToken = default);

    /// <summary>
    /// Updates an existing ticket.
    /// </summary>
    Task<TicketResponse> UpdateAsync(Guid ticketId, UpdateTicketRequest request, CancellationToken cancellationToken = default);

    /// <summary>
    /// Deletes a ticket.
    /// </summary>
    Task DeleteAsync(Guid ticketId, CancellationToken cancellationToken = default);

    /// <summary>
    /// Calls a ticket for service.
    /// </summary>
    Task<TicketResponse> CallAsync(Guid ticketId, CancellationToken cancellationToken = default);

    /// <summary>
    /// Starts serving a ticket.
    /// </summary>
    Task<TicketResponse> StartServingAsync(Guid ticketId, CancellationToken cancellationToken = default);

    /// <summary>
    /// Completes service for a ticket.
    /// </summary>
    Task<TicketResponse> CompleteAsync(Guid ticketId, CancellationToken cancellationToken = default);

    /// <summary>
    /// Cancels a ticket.
    /// </summary>
    Task<TicketResponse> CancelAsync(Guid ticketId, string? reason = null, CancellationToken cancellationToken = default);

    /// <summary>
    /// Marks a ticket as no-show.
    /// </summary>
    Task<TicketResponse> MarkNoShowAsync(Guid ticketId, CancellationToken cancellationToken = default);

    /// <summary>
    /// Transfers a ticket to another queue.
    /// </summary>
    Task<TicketResponse> TransferAsync(Guid ticketId, TransferTicketRequest request, CancellationToken cancellationToken = default);

    /// <summary>
    /// Gets the status of a ticket.
    /// </summary>
    Task<TicketStatusResponse> GetStatusAsync(Guid ticketId, CancellationToken cancellationToken = default);

    /// <summary>
    /// Gets the queue position of a ticket.
    /// </summary>
    Task<QueuePositionResponse> GetPositionAsync(Guid ticketId, CancellationToken cancellationToken = default);

    /// <summary>
    /// Gets tickets by queue.
    /// </summary>
    Task<List<TicketResponse>> GetByQueueAsync(Guid queueId, TicketStatus? status = null, CancellationToken cancellationToken = default);

    /// <summary>
    /// Gets tickets by customer document.
    /// </summary>
    Task<List<TicketResponse>> GetByCustomerAsync(string customerDocument, CancellationToken cancellationToken = default);

    /// <summary>
    /// Gets tickets by service.
    /// </summary>
    Task<List<TicketResponse>> GetByServiceAsync(Guid serviceId, TicketStatus? status = null, CancellationToken cancellationToken = default);

    /// <summary>
    /// Gets paginated tickets.
    /// </summary>
    Task<PagedResult<TicketResponse>> GetPagedAsync(PagedRequest request, CancellationToken cancellationToken = default);

    /// <summary>
    /// Streams tickets in a queue.
    /// </summary>
    IAsyncEnumerable<TicketResponse> StreamQueueTicketsAsync(Guid queueId, CancellationToken cancellationToken = default);

    /// <summary>
    /// Batch creates tickets.
    /// </summary>
    Task<BatchResponse<TicketResponse>> CreateBatchAsync(BatchRequest<CreateTicketRequest> request, CancellationToken cancellationToken = default);

    /// <summary>
    /// Recalls a ticket that was previously called.
    /// </summary>
    Task<TicketResponse> RecallAsync(Guid ticketId, CancellationToken cancellationToken = default);

    /// <summary>
    /// Gets the next ticket to be called in a queue.
    /// </summary>
    Task<TicketResponse?> GetNextAsync(Guid queueId, CancellationToken cancellationToken = default);
}