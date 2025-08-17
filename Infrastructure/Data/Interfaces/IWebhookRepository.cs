using QueueManagement.Domain.Entities;

namespace QueueManagement.Infrastructure.Data.Interfaces;

/// <summary>
/// Repository interface for Webhook entity operations
/// </summary>
public interface IWebhookRepository : IGenericRepository<Webhook>
{
    /// <summary>
    /// Get active webhooks by tenant and event type
    /// </summary>
    Task<List<Webhook>> GetActiveWebhooksByTenantAndEventAsync(Guid tenantId, string eventType);

    /// <summary>
    /// Get webhooks by tenant
    /// </summary>
    Task<List<Webhook>> GetByTenantAsync(Guid tenantId);

    /// <summary>
    /// Log webhook execution
    /// </summary>
    Task LogExecutionAsync(WebhookExecutionLog log);

    /// <summary>
    /// Get execution logs for a webhook
    /// </summary>
    Task<List<WebhookExecutionLog>> GetExecutionLogsAsync(Guid webhookId, int limit = 50);
}

/// <summary>
/// Webhook execution log entity
/// </summary>
public class WebhookExecutionLog
{
    public Guid Id { get; set; }
    public Guid WebhookId { get; set; }
    public DateTime ExecutedAt { get; set; }
    public int StatusCode { get; set; }
    public string? ResponseBody { get; set; }
    public string? ErrorMessage { get; set; }
    public TimeSpan ExecutionTime { get; set; }
    public bool IsSuccess { get; set; }
}