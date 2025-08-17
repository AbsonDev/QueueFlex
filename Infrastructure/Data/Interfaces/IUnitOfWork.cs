namespace QueueManagement.Infrastructure.Data.Interfaces;

/// <summary>
/// Unit of Work interface for managing database transactions and repositories
/// </summary>
public interface IUnitOfWork : IDisposable
{
    /// <summary>
    /// Ticket repository
    /// </summary>
    ITicketRepository TicketRepository { get; }

    /// <summary>
    /// Queue repository
    /// </summary>
    IQueueRepository QueueRepository { get; }

    /// <summary>
    /// Session repository
    /// </summary>
    ISessionRepository SessionRepository { get; }

    /// <summary>
    /// Unit repository
    /// </summary>
    IUnitRepository UnitRepository { get; }

    /// <summary>
    /// Service repository
    /// </summary>
    IServiceRepository ServiceRepository { get; }

    /// <summary>
    /// User repository
    /// </summary>
    IUserRepository UserRepository { get; }

    /// <summary>
    /// Tenant repository
    /// </summary>
    ITenantRepository TenantRepository { get; }

    /// <summary>
    /// Webhook repository
    /// </summary>
    IWebhookRepository WebhookRepository { get; }

    /// <summary>
    /// Save all changes to database
    /// </summary>
    Task<int> SaveChangesAsync(CancellationToken cancellationToken = default);

    /// <summary>
    /// Begin a new transaction
    /// </summary>
    Task<IDbContextTransaction> BeginTransactionAsync();

    /// <summary>
    /// Commit the current transaction
    /// </summary>
    Task CommitTransactionAsync(IDbContextTransaction transaction);

    /// <summary>
    /// Rollback the current transaction
    /// </summary>
    Task RollbackTransactionAsync(IDbContextTransaction transaction);
}