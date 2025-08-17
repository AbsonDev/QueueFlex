using QueueManagement.Domain.Entities;
using QueueManagement.Domain.Enums;

namespace QueueManagement.Infrastructure.Data.Interfaces;

/// <summary>
/// Repository interface for Ticket entity operations
/// </summary>
public interface ITicketRepository : IGenericRepository<Ticket>
{
    /// <summary>
    /// Get ticket by number for a specific tenant
    /// </summary>
    Task<Ticket?> GetByNumberAsync(Guid tenantId, string number);

    /// <summary>
    /// Get tickets by queue with optional status filtering
    /// </summary>
    Task<List<Ticket>> GetByQueueAsync(Guid tenantId, Guid queueId, TicketStatus? status = null);

    /// <summary>
    /// Get count of waiting tickets in a queue
    /// </summary>
    Task<int> GetWaitingCountAsync(Guid queueId);

    /// <summary>
    /// Get position of a ticket in the queue
    /// </summary>
    Task<int> GetQueuePositionAsync(Guid ticketId);

    /// <summary>
    /// Get next ticket in queue
    /// </summary>
    Task<Ticket?> GetNextInQueueAsync(Guid queueId);

    /// <summary>
    /// Get ticket history for a customer
    /// </summary>
    Task<List<Ticket>> GetTicketHistoryAsync(Guid tenantId, string? customerDocument);

    /// <summary>
    /// Get daily count of tickets for a queue
    /// </summary>
    Task<int> GetDailyCountAsync(Guid queueId, DateTime date);

    /// <summary>
    /// Get tickets by customer document
    /// </summary>
    Task<List<Ticket>> GetByCustomerAsync(Guid tenantId, string customerDocument);

    /// <summary>
    /// Get average wait time for a queue
    /// </summary>
    Task<double> GetAverageWaitTimeAsync(Guid queueId, DateTime? fromDate = null);

    /// <summary>
    /// Get active tickets for a tenant
    /// </summary>
    Task<List<Ticket>> GetActiveTicketsAsync(Guid tenantId, Guid? unitId = null);
}