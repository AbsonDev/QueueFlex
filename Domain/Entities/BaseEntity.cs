using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace QueueManagement.Domain.Entities;

/// <summary>
/// Base entity class with common properties for all entities
/// </summary>
public abstract class BaseEntity
{
    /// <summary>
    /// Unique identifier for the entity
    /// </summary>
    [Key]
    public Guid Id { get; set; }

    /// <summary>
    /// Tenant identifier for multi-tenancy
    /// </summary>
    [Required]
    public Guid TenantId { get; set; }

    /// <summary>
    /// Date and time when the entity was created
    /// </summary>
    [Required]
    public DateTime CreatedAt { get; set; }

    /// <summary>
    /// Date and time when the entity was last updated
    /// </summary>
    [Required]
    public DateTime UpdatedAt { get; set; }

    /// <summary>
    /// User who created the entity
    /// </summary>
    [Required]
    [MaxLength(100)]
    public string CreatedBy { get; set; } = string.Empty;

    /// <summary>
    /// User who last updated the entity
    /// </summary>
    [Required]
    [MaxLength(100)]
    public string UpdatedBy { get; set; } = string.Empty;

    /// <summary>
    /// Soft delete flag
    /// </summary>
    [Required]
    public bool IsDeleted { get; set; }

    /// <summary>
    /// Row version for optimistic concurrency
    /// </summary>
    [Timestamp]
    public byte[]? RowVersion { get; set; }

    /// <summary>
    /// Constructor that sets default values
    /// </summary>
    protected BaseEntity()
    {
        Id = Guid.NewGuid();
        CreatedAt = DateTime.UtcNow;
        UpdatedAt = DateTime.UtcNow;
        IsDeleted = false;
    }

    /// <summary>
    /// Sets the creation audit information
    /// </summary>
    public virtual void SetCreated(string createdBy)
    {
        CreatedBy = createdBy ?? throw new ArgumentNullException(nameof(createdBy));
        CreatedAt = DateTime.UtcNow;
        UpdatedAt = DateTime.UtcNow;
        UpdatedBy = createdBy;
    }

    /// <summary>
    /// Sets the update audit information
    /// </summary>
    public virtual void SetUpdated(string updatedBy)
    {
        UpdatedBy = updatedBy ?? throw new ArgumentNullException(nameof(updatedBy));
        UpdatedAt = DateTime.UtcNow;
    }

    /// <summary>
    /// Marks the entity as deleted (soft delete)
    /// </summary>
    public virtual void MarkAsDeleted(string deletedBy)
    {
        IsDeleted = true;
        SetUpdated(deletedBy);
    }

    /// <summary>
    /// Restores the entity from soft delete
    /// </summary>
    public virtual void Restore(string restoredBy)
    {
        IsDeleted = false;
        SetUpdated(restoredBy);
    }
}