using System.ComponentModel.DataAnnotations;
using System.Text.Json;

namespace QueueManagement.Domain.Entities;

/// <summary>
/// Represents a webhook for external integrations
/// </summary>
public class Webhook : BaseEntity
{
    /// <summary>
    /// Webhook name
    /// </summary>
    [Required]
    [MaxLength(200)]
    public string Name { get; set; } = string.Empty;

    /// <summary>
    /// Webhook URL
    /// </summary>
    [Required]
    [MaxLength(500)]
    [Url]
    public string Url { get; set; } = string.Empty;

    /// <summary>
    /// Secret key for webhook authentication
    /// </summary>
    [Required]
    [MaxLength(100)]
    public string Secret { get; set; } = string.Empty;

    /// <summary>
    /// Events that trigger this webhook (JSON array)
    /// </summary>
    [Required]
    [MaxLength(1000)]
    public string Events { get; set; } = string.Empty;

    /// <summary>
    /// Whether this webhook is active
    /// </summary>
    [Required]
    public bool IsActive { get; set; }

    /// <summary>
    /// Number of retry attempts for failed webhook calls
    /// </summary>
    [Required]
    [Range(0, 10)]
    public int RetryCount { get; set; }

    /// <summary>
    /// Date and time when this webhook was last triggered
    /// </summary>
    public DateTime? LastTriggeredAt { get; set; }

    // Navigation properties
    /// <summary>
    /// Tenant that owns this webhook
    /// </summary>
    public virtual Tenant Tenant { get; set; } = null!;

    /// <summary>
    /// Constructor that sets default values
    /// </summary>
    public Webhook() : base()
    {
        IsActive = true;
        RetryCount = 3;
        Events = "[]";
    }

    /// <summary>
    /// Creates a new webhook
    /// </summary>
    public Webhook(string name, string url, string secret, string[] events, Guid tenantId, string createdBy) : this()
    {
        Name = name ?? throw new ArgumentNullException(nameof(name));
        Url = url ?? throw new ArgumentNullException(nameof(url));
        Secret = secret ?? throw new ArgumentNullException(nameof(secret));
        Events = JsonSerializer.Serialize(events ?? throw new ArgumentNullException(nameof(events)));
        TenantId = tenantId;
        SetCreated(createdBy);
    }

    /// <summary>
    /// Updates the webhook events
    /// </summary>
    public void UpdateEvents(string[] newEvents, string updatedBy)
    {
        Events = JsonSerializer.Serialize(newEvents ?? throw new ArgumentNullException(nameof(newEvents)));
        SetUpdated(updatedBy);
    }

    /// <summary>
    /// Gets the webhook events as an array
    /// </summary>
    public string[] GetEvents()
    {
        try
        {
            return JsonSerializer.Deserialize<string[]>(Events) ?? Array.Empty<string>();
        }
        catch
        {
            return Array.Empty<string>();
        }
    }

    /// <summary>
    /// Updates the webhook URL
    /// </summary>
    public void UpdateUrl(string newUrl, string updatedBy)
    {
        Url = newUrl ?? throw new ArgumentNullException(nameof(newUrl));
        SetUpdated(updatedBy);
    }

    /// <summary>
    /// Updates the webhook secret
    /// </summary>
    public void UpdateSecret(string newSecret, string updatedBy)
    {
        Secret = newSecret ?? throw new ArgumentNullException(nameof(newSecret));
        SetUpdated(updatedBy);
    }

    /// <summary>
    /// Updates the retry count
    /// </summary>
    public void UpdateRetryCount(int newRetryCount, string updatedBy)
    {
        if (newRetryCount < 0 || newRetryCount > 10)
            throw new ArgumentOutOfRangeException(nameof(newRetryCount), "Retry count must be between 0 and 10");

        RetryCount = newRetryCount;
        SetUpdated(updatedBy);
    }

    /// <summary>
    /// Activates this webhook
    /// </summary>
    public void Activate(string updatedBy)
    {
        IsActive = true;
        SetUpdated(updatedBy);
    }

    /// <summary>
    /// Deactivates this webhook
    /// </summary>
    public void Deactivate(string updatedBy)
    {
        IsActive = false;
        SetUpdated(updatedBy);
    }

    /// <summary>
    /// Marks the webhook as triggered
    /// </summary>
    public void MarkAsTriggered()
    {
        LastTriggeredAt = DateTime.UtcNow;
    }

    /// <summary>
    /// Checks if the webhook should be triggered for a specific event
    /// </summary>
    public bool ShouldTriggerForEvent(string eventName)
    {
        if (!IsActive)
            return false;

        var events = GetEvents();
        return events.Contains(eventName, StringComparer.OrdinalIgnoreCase);
    }

    /// <summary>
    /// Validates the webhook configuration
    /// </summary>
    public bool IsValid()
    {
        return !string.IsNullOrEmpty(Url) && 
               !string.IsNullOrEmpty(Secret) && 
               Uri.TryCreate(Url, UriKind.Absolute, out _);
    }
}