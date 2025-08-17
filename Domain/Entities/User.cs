using System.ComponentModel.DataAnnotations;
using QueueManagement.Domain.Enums;

namespace QueueManagement.Domain.Entities;

/// <summary>
/// Represents a user (attendant, manager, etc.) in the system
/// </summary>
public class User : BaseEntity
{
    /// <summary>
    /// User's full name
    /// </summary>
    [Required]
    [MaxLength(200)]
    public string Name { get; set; } = string.Empty;

    /// <summary>
    /// User's email address
    /// </summary>
    [Required]
    [MaxLength(200)]
    [EmailAddress]
    public string Email { get; set; } = string.Empty;

    /// <summary>
    /// Employee code
    /// </summary>
    [Required]
    [MaxLength(50)]
    public string EmployeeCode { get; set; } = string.Empty;

    /// <summary>
    /// User's role in the system
    /// </summary>
    [Required]
    public UserRole Role { get; set; }

    /// <summary>
    /// Current status of the user
    /// </summary>
    [Required]
    public UserStatus Status { get; set; }

    // Navigation properties
    /// <summary>
    /// Tenant that owns this user
    /// </summary>
    public virtual Tenant Tenant { get; set; } = null!;

    /// <summary>
    /// Units this user is assigned to
    /// </summary>
    public virtual ICollection<UnitUser> UnitUsers { get; set; } = new List<UnitUser>();

    /// <summary>
    /// Sessions this user has handled
    /// </summary>
    public virtual ICollection<Session> Sessions { get; set; } = new List<Session>();

    /// <summary>
    /// Ticket status history changes made by this user
    /// </summary>
    public virtual ICollection<TicketStatusHistory> TicketStatusHistories { get; set; } = new List<TicketStatusHistory>();

    /// <summary>
    /// Constructor that sets default values
    /// </summary>
    public User() : base()
    {
        Status = UserStatus.Active;
        Role = UserRole.Attendant;
    }

    /// <summary>
    /// Creates a new user
    /// </summary>
    public User(string name, string email, string employeeCode, UserRole role, Guid tenantId, string createdBy) : this()
    {
        Name = name ?? throw new ArgumentNullException(nameof(name));
        Email = email ?? throw new ArgumentNullException(nameof(email));
        EmployeeCode = employeeCode ?? throw new ArgumentNullException(nameof(employeeCode));
        Role = role;
        TenantId = tenantId;
        SetCreated(createdBy);
    }

    /// <summary>
    /// Updates the user's role
    /// </summary>
    public void UpdateRole(UserRole newRole, string updatedBy)
    {
        Role = newRole;
        SetUpdated(updatedBy);
    }

    /// <summary>
    /// Updates the user's status
    /// </summary>
    public void UpdateStatus(UserStatus newStatus, string updatedBy)
    {
        Status = newStatus;
        SetUpdated(updatedBy);
    }

    /// <summary>
    /// Checks if the user has a specific role
    /// </summary>
    public bool HasRole(UserRole role)
    {
        return Role == role;
    }

    /// <summary>
    /// Checks if the user has admin privileges
    /// </summary>
    public bool IsAdmin => Role == UserRole.Admin;

    /// <summary>
    /// Checks if the user has manager privileges
    /// </summary>
    public bool IsManager => Role == UserRole.Manager || Role == UserRole.Admin;

    /// <summary>
    /// Checks if the user has supervisor privileges
    /// </summary>
    public bool IsSupervisor => Role == UserRole.Supervisor || Role == UserRole.Manager || Role == UserRole.Admin;

    /// <summary>
    /// Checks if the user is currently available for work
    /// </summary>
    public bool IsAvailable => Status == UserStatus.Active;
}