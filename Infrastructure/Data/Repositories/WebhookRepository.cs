using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using QueueManagement.Domain.Entities;
using QueueManagement.Infrastructure.Data.Interfaces;

namespace QueueManagement.Infrastructure.Data.Repositories;

/// <summary>
/// Repository implementation for Webhook entity
/// </summary>
public class WebhookRepository : GenericRepository<Webhook>, IWebhookRepository
{
    public WebhookRepository(QueueManagementDbContext context, ILogger<WebhookRepository> logger) 
        : base(context, logger) { }

    public async Task<List<Webhook>> GetActiveWebhooksByTenantAndEventAsync(Guid tenantId, string eventType)
    {
        try
        {
            _logger.LogDebug("Getting active webhooks for tenant {TenantId} and event {EventType}", tenantId, eventType);

            if (string.IsNullOrWhiteSpace(eventType))
                throw new ArgumentException("Event type cannot be null or empty", nameof(eventType));

            return await _dbSet
                .AsNoTracking()
                .Where(w => w.TenantId == tenantId && 
                           w.EventType == eventType && 
                           w.IsActive && 
                           !w.IsDeleted)
                .OrderBy(w => w.Name)
                .ToListAsync();
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error getting active webhooks for tenant {TenantId} and event {EventType}", 
                tenantId, eventType);
            throw;
        }
    }

    public async Task<List<Webhook>> GetByTenantAsync(Guid tenantId)
    {
        try
        {
            _logger.LogDebug("Getting webhooks for tenant {TenantId}", tenantId);

            return await _dbSet
                .AsNoTracking()
                .Where(w => w.TenantId == tenantId && !w.IsDeleted)
                .OrderBy(w => w.Name)
                .ToListAsync();
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error getting webhooks for tenant {TenantId}", tenantId);
            throw;
        }
    }

    public async Task LogExecutionAsync(WebhookExecutionLog log)
    {
        try
        {
            _logger.LogDebug("Logging webhook execution for webhook {WebhookId}", log.WebhookId);

            if (log == null)
                throw new ArgumentNullException(nameof(log));

            // Set audit fields
            log.Id = Guid.NewGuid();
            log.ExecutedAt = DateTime.UtcNow;

            // Add to context - this will be saved when SaveChanges is called
            _context.Add(log);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error logging webhook execution for webhook {WebhookId}", log?.WebhookId);
            throw;
        }
    }

    public async Task<List<WebhookExecutionLog>> GetExecutionLogsAsync(Guid webhookId, int limit = 50)
    {
        try
        {
            _logger.LogDebug("Getting execution logs for webhook {WebhookId} with limit {Limit}", webhookId, limit);

            if (limit <= 0) limit = 50;
            if (limit > 1000) limit = 1000; // Prevent excessive data retrieval

            // Since WebhookExecutionLog is not a DbSet, we'll need to create a separate table
            // For now, we'll return an empty list - this should be implemented with a proper entity
            _logger.LogWarning("WebhookExecutionLog entity not implemented in DbContext. Returning empty list.");
            
            return new List<WebhookExecutionLog>();
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error getting execution logs for webhook {WebhookId}", webhookId);
            throw;
        }
    }
}