using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Storage;
using Microsoft.Extensions.Logging;
using MediatR;
using QueueManagement.Infrastructure.Data.Interfaces;

namespace QueueManagement.Infrastructure.Data.UnitOfWork;

/// <summary>
/// Unit of Work implementation for managing database transactions and repositories
/// </summary>
public class UnitOfWork : IUnitOfWork
{
    private readonly QueueManagementDbContext _context;
    private readonly IPublisher _publisher;
    private readonly ILogger<UnitOfWork> _logger;

    // Repository instances with lazy loading
    private ITicketRepository? _ticketRepository;
    private IQueueRepository? _queueRepository;
    private ISessionRepository? _sessionRepository;
    private IUnitRepository? _unitRepository;
    private IServiceRepository? _serviceRepository;
    private IUserRepository? _userRepository;
    private ITenantRepository? _tenantRepository;
    private IWebhookRepository? _webhookRepository;

    public UnitOfWork(
        QueueManagementDbContext context, 
        IPublisher publisher,
        ILogger<UnitOfWork> logger)
    {
        _context = context ?? throw new ArgumentNullException(nameof(context));
        _publisher = publisher ?? throw new ArgumentNullException(nameof(publisher));
        _logger = logger ?? throw new ArgumentNullException(nameof(logger));
    }

    // Repository properties with lazy loading
    public ITicketRepository TicketRepository => 
        _ticketRepository ??= new Repositories.TicketRepository(_context, _logger);
    
    public IQueueRepository QueueRepository => 
        _queueRepository ??= new Repositories.QueueRepository(_context, _logger);
    
    public ISessionRepository SessionRepository => 
        _sessionRepository ??= new Repositories.SessionRepository(_context, _logger);
    
    public IUnitRepository UnitRepository => 
        _unitRepository ??= new Repositories.UnitRepository(_context, _logger);
    
    public IServiceRepository ServiceRepository => 
        _serviceRepository ??= new Repositories.ServiceRepository(_context, _logger);
    
    public IUserRepository UserRepository => 
        _userRepository ??= new Repositories.UserRepository(_context, _logger);
    
    public ITenantRepository TenantRepository => 
        _tenantRepository ??= new Repositories.TenantRepository(_context, _logger);
    
    public IWebhookRepository WebhookRepository => 
        _webhookRepository ??= new Repositories.WebhookRepository(_context, _logger);

    public async Task<int> SaveChangesAsync(CancellationToken cancellationToken = default)
    {
        try
        {
            _logger.LogDebug("Starting to save changes to database");

            // Update audit fields
            UpdateAuditFields();
            
            // Dispatch domain events
            await DispatchDomainEventsAsync();
            
            // Save changes
            var result = await _context.SaveChangesAsync(cancellationToken);
            
            _logger.LogInformation("Successfully saved {ChangesCount} changes to database", result);
            
            return result;
        }
        catch (DbUpdateConcurrencyException ex)
        {
            _logger.LogError(ex, "Concurrency conflict occurred while saving changes");
            throw new InvalidOperationException("The data was modified by another user. Please refresh and try again.", ex);
        }
        catch (DbUpdateException ex)
        {
            _logger.LogError(ex, "Database update error occurred while saving changes");
            throw new InvalidOperationException("An error occurred while saving the data. Please check your input and try again.", ex);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Unexpected error occurred while saving changes to database");
            throw;
        }
    }

    public async Task<IDbContextTransaction> BeginTransactionAsync()
    {
        try
        {
            _logger.LogDebug("Beginning new database transaction");
            return await _context.Database.BeginTransactionAsync();
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error beginning database transaction");
            throw;
        }
    }

    public async Task CommitTransactionAsync(IDbContextTransaction transaction)
    {
        try
        {
            if (transaction == null)
                throw new ArgumentNullException(nameof(transaction));

            _logger.LogDebug("Committing database transaction");
            await transaction.CommitAsync();
            _logger.LogInformation("Database transaction committed successfully");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error committing database transaction");
            throw;
        }
    }

    public async Task RollbackTransactionAsync(IDbContextTransaction transaction)
    {
        try
        {
            if (transaction == null)
                throw new ArgumentNullException(nameof(transaction));

            _logger.LogDebug("Rolling back database transaction");
            await transaction.RollbackAsync();
            _logger.LogInformation("Database transaction rolled back successfully");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error rolling back database transaction");
            throw;
        }
    }

    private void UpdateAuditFields()
    {
        var entries = _context.ChangeTracker.Entries<Domain.Entities.BaseEntity>();

        foreach (var entry in entries)
        {
            switch (entry.State)
            {
                case EntityState.Added:
                    entry.Entity.CreatedAt = DateTime.UtcNow;
                    entry.Entity.UpdatedAt = DateTime.UtcNow;
                    break;
                case EntityState.Modified:
                    entry.Entity.UpdatedAt = DateTime.UtcNow;
                    break;
            }
        }
    }

    private async Task DispatchDomainEventsAsync()
    {
        try
        {
            var domainEntities = _context.ChangeTracker
                .Entries<Domain.Entities.BaseEntity>()
                .Where(x => x.Entity.DomainEvents != null && x.Entity.DomainEvents.Any())
                .ToList();

            var domainEvents = domainEntities
                .SelectMany(x => x.Entity.DomainEvents)
                .ToList();

            // Clear domain events from entities
            domainEntities.ForEach(entity => entity.Entity.ClearDomainEvents());

            // Publish domain events
            foreach (var domainEvent in domainEvents)
            {
                _logger.LogDebug("Publishing domain event: {EventType}", domainEvent.GetType().Name);
                await _publisher.Publish(domainEvent);
            }

            if (domainEvents.Any())
            {
                _logger.LogInformation("Published {EventCount} domain events", domainEvents.Count);
            }
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error occurred while dispatching domain events");
            // Don't throw here - we don't want domain event failures to prevent data persistence
        }
    }

    public void Dispose()
    {
        try
        {
            _logger.LogDebug("Disposing UnitOfWork");
            _context?.Dispose();
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error occurred while disposing UnitOfWork");
        }
    }
}