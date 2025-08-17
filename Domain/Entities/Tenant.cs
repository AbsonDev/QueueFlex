using System.ComponentModel.DataAnnotations;
using QueueManagement.Domain.Enums;

namespace QueueManagement.Domain.Entities;

/// <summary>
/// Represents a tenant (company/client) in the multi-tenant system
/// </summary>
public class Tenant : BaseEntity
{
    /// <summary>
    /// Company name
    /// </summary>
    [Required]
    [MaxLength(200)]
    public string Name { get; set; } = string.Empty;

    /// <summary>
    /// Unique subdomain for the tenant
    /// </summary>
    [Required]
    [MaxLength(50)]
    public string Subdomain { get; set; } = string.Empty;

    /// <summary>
    /// Timezone for the tenant
    /// </summary>
    [Required]
    [MaxLength(50)]
    public string TimeZone { get; set; } = string.Empty;

    /// <summary>
    /// Whether the tenant is active
    /// </summary>
    [Required]
    public bool IsActive { get; set; }

    /// <summary>
    /// Subscription plan for the tenant
    /// </summary>
    [Required]
    public TenantPlan Plan { get; set; }

    // Navigation properties
    /// <summary>
    /// Units associated with this tenant
    /// </summary>
    public virtual ICollection<Unit> Units { get; set; } = new List<Unit>();

    /// <summary>
    /// Services associated with this tenant
    /// </summary>
    public virtual ICollection<Service> Services { get; set; } = new List<Service>();

    /// <summary>
    /// Users associated with this tenant
    /// </summary>
    public virtual ICollection<User> Users { get; set; } = new List<User>();

    /// <summary>
    /// Webhooks associated with this tenant
    /// </summary>
    public virtual ICollection<Webhook> Webhooks { get; set; } = new List<Webhook>();

    /// <summary>
    /// Constructor that sets default values
    /// </summary>
    public Tenant() : base()
    {
        IsActive = true;
        Plan = TenantPlan.Free;
    }

    /// <summary>
    /// Creates a new tenant
    /// </summary>
    public Tenant(string name, string subdomain, string timeZone, string createdBy) : this()
    {
        Name = name ?? throw new ArgumentNullException(nameof(name));
        Subdomain = subdomain ?? throw new ArgumentNullException(nameof(subdomain));
        TimeZone = timeZone ?? throw new ArgumentNullException(nameof(timeZone));
        SetCreated(createdBy);
    }

    /// <summary>
    /// Updates the tenant plan
    /// </summary>
    public void UpdatePlan(TenantPlan newPlan, string updatedBy)
    {
        Plan = newPlan;
        SetUpdated(updatedBy);
    }

    /// <summary>
    /// Activates the tenant
    /// </summary>
    public void Activate(string updatedBy)
    {
        IsActive = true;
        SetUpdated(updatedBy);
    }

    /// <summary>
    /// Deactivates the tenant
    /// </summary>
    public void Deactivate(string updatedBy)
    {
        IsActive = false;
        SetUpdated(updatedBy);
    }
}