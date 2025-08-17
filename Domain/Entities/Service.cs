using System.ComponentModel.DataAnnotations;
using System.Text.Json;

namespace QueueManagement.Domain.Entities;

/// <summary>
/// Represents a service type that can be offered in queues
/// </summary>
public class Service : BaseEntity
{
    /// <summary>
    /// Service name
    /// </summary>
    [Required]
    [MaxLength(200)]
    public string Name { get; set; } = string.Empty;

    /// <summary>
    /// Unique code for the service within the tenant
    /// </summary>
    [Required]
    [MaxLength(50)]
    public string Code { get; set; } = string.Empty;

    /// <summary>
    /// Service description
    /// </summary>
    [MaxLength(1000)]
    public string? Description { get; set; }

    /// <summary>
    /// Estimated duration in minutes
    /// </summary>
    [Required]
    [Range(1, 1440)] // 1 minute to 24 hours
    public int EstimatedDurationMinutes { get; set; }

    /// <summary>
    /// Color for UI representation
    /// </summary>
    [Required]
    [MaxLength(7)] // #RRGGBB format
    public string Color { get; set; } = "#000000";

    /// <summary>
    /// Whether this service requires a physical resource
    /// </summary>
    [Required]
    public bool RequiresResource { get; set; }

    /// <summary>
    /// Whether this service is active
    /// </summary>
    [Required]
    public bool IsActive { get; set; }

    /// <summary>
    /// JSON configuration for service flexibility
    /// </summary>
    [MaxLength(4000)]
    public string? ConfigurationJson { get; set; }

    // Navigation properties
    /// <summary>
    /// Tenant that owns this service
    /// </summary>
    public virtual Tenant Tenant { get; set; } = null!;

    /// <summary>
    /// Queues that offer this service
    /// </summary>
    public virtual ICollection<QueueService> QueueServices { get; set; } = new List<QueueService>();

    /// <summary>
    /// Tickets that use this service
    /// </summary>
    public virtual ICollection<Ticket> Tickets { get; set; } = new List<Ticket>();

    /// <summary>
    /// Constructor that sets default values
    /// </summary>
    public Service() : base()
    {
        IsActive = true;
        RequiresResource = false;
        EstimatedDurationMinutes = 30;
        Color = "#007bff";
    }

    /// <summary>
    /// Creates a new service
    /// </summary>
    public Service(string name, string code, int estimatedDurationMinutes, Guid tenantId, string createdBy) : this()
    {
        Name = name ?? throw new ArgumentNullException(nameof(name));
        Code = code ?? throw new ArgumentNullException(nameof(code));
        EstimatedDurationMinutes = estimatedDurationMinutes;
        TenantId = tenantId;
        SetCreated(createdBy);
    }

    /// <summary>
    /// Updates the service configuration
    /// </summary>
    public void UpdateConfiguration(object configuration, string updatedBy)
    {
        ConfigurationJson = JsonSerializer.Serialize(configuration);
        SetUpdated(updatedBy);
    }

    /// <summary>
    /// Gets the configuration object
    /// </summary>
    public T? GetConfiguration<T>()
    {
        if (string.IsNullOrEmpty(ConfigurationJson))
            return default;

        try
        {
            return JsonSerializer.Deserialize<T>(ConfigurationJson);
        }
        catch
        {
            return default;
        }
    }

    /// <summary>
    /// Updates the service color
    /// </summary>
    public void UpdateColor(string color, string updatedBy)
    {
        Color = color ?? throw new ArgumentNullException(nameof(color));
        SetUpdated(updatedBy);
    }

    /// <summary>
    /// Activates this service
    /// </summary>
    public void Activate(string updatedBy)
    {
        IsActive = true;
        SetUpdated(updatedBy);
    }

    /// <summary>
    /// Deactivates this service
    /// </summary>
    public void Deactivate(string updatedBy)
    {
        IsActive = false;
        SetUpdated(updatedBy);
    }

    /// <summary>
    /// Updates the estimated duration
    /// </summary>
    public void UpdateDuration(int durationMinutes, string updatedBy)
    {
        if (durationMinutes < 1 || durationMinutes > 1440)
            throw new ArgumentOutOfRangeException(nameof(durationMinutes), "Duration must be between 1 and 1440 minutes");

        EstimatedDurationMinutes = durationMinutes;
        SetUpdated(updatedBy);
    }
}