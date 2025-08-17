using System.ComponentModel.DataAnnotations;

namespace QueueManagement.Api.DTOs.Users;

/// <summary>
/// User response DTO
/// </summary>
public class UserDto
{
    /// <summary>
    /// User ID
    /// </summary>
    public Guid Id { get; set; }

    /// <summary>
    /// User's full name
    /// </summary>
    public string Name { get; set; } = string.Empty;

    /// <summary>
    /// User's email address
    /// </summary>
    public string Email { get; set; } = string.Empty;

    /// <summary>
    /// Employee code
    /// </summary>
    public string EmployeeCode { get; set; } = string.Empty;

    /// <summary>
    /// User's role in the system
    /// </summary>
    public string Role { get; set; } = string.Empty;

    /// <summary>
    /// Current status of the user
    /// </summary>
    public string Status { get; set; } = string.Empty;

    /// <summary>
    /// Number of units this user is assigned to
    /// </summary>
    public int UnitCount { get; set; }

    /// <summary>
    /// Number of active sessions this user has handled
    /// </summary>
    public int ActiveSessionCount { get; set; }

    /// <summary>
    /// Whether the user is currently available for work
    /// </summary>
    public bool IsAvailable { get; set; }

    /// <summary>
    /// Creation date
    /// </summary>
    public DateTime CreatedAt { get; set; }

    /// <summary>
    /// Last update date
    /// </summary>
    public DateTime UpdatedAt { get; set; }
}

/// <summary>
/// Create user request DTO
/// </summary>
public class CreateUserDto
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
    [EmailAddress]
    [MaxLength(200)]
    public string Email { get; set; } = string.Empty;

    /// <summary>
    /// Employee code
    /// </summary>
    [Required]
    [MaxLength(50)]
    public string EmployeeCode { get; set; } = string.Empty;

    /// <summary>
    /// User's password
    /// </summary>
    [Required]
    [MinLength(8)]
    public string Password { get; set; } = string.Empty;

    /// <summary>
    /// User's role in the system
    /// </summary>
    [Required]
    public string Role { get; set; } = string.Empty;

    /// <summary>
    /// Unit IDs to assign this user to
    /// </summary>
    public List<Guid> UnitIds { get; set; } = new();
}

/// <summary>
/// Update user request DTO
/// </summary>
public class UpdateUserDto
{
    /// <summary>
    /// User's full name
    /// </summary>
    [MaxLength(200)]
    public string? Name { get; set; }

    /// <summary>
    /// User's email address
    /// </summary>
    [EmailAddress]
    [MaxLength(200)]
    public string? Email { get; set; }

    /// <summary>
    /// Employee code
    /// </summary>
    [MaxLength(50)]
    public string? EmployeeCode { get; set; }

    /// <summary>
    /// User's role in the system
    /// </summary>
    public string? Role { get; set; }

    /// <summary>
    /// Unit IDs to assign this user to
    /// </summary>
    public List<Guid>? UnitIds { get; set; }
}

/// <summary>
/// Update user status request DTO
/// </summary>
public class UpdateUserStatusDto
{
    /// <summary>
    /// New status
    /// </summary>
    [Required]
    public string Status { get; set; } = string.Empty;

    /// <summary>
    /// Reason for status change
    /// </summary>
    [MaxLength(500)]
    public string? Reason { get; set; }
}

/// <summary>
/// Change password request DTO
/// </summary>
public class ChangePasswordDto
{
    /// <summary>
    /// Current password
    /// </summary>
    [Required]
    public string CurrentPassword { get; set; } = string.Empty;

    /// <summary>
    /// New password
    /// </summary>
    [Required]
    [MinLength(8)]
    public string NewPassword { get; set; } = string.Empty;

    /// <summary>
    /// Confirm new password
    /// </summary>
    [Required]
    [Compare(nameof(NewPassword), ErrorMessage = "Passwords do not match")]
    public string ConfirmPassword { get; set; } = string.Empty;
}

/// <summary>
/// User summary DTO for lists
/// </summary>
public class UserSummaryDto
{
    /// <summary>
    /// User ID
    /// </summary>
    public Guid Id { get; set; }

    /// <summary>
    /// User's full name
    /// </summary>
    public string Name { get; set; } = string.Empty;

    /// <summary>
    /// Employee code
    /// </summary>
    public string EmployeeCode { get; set; } = string.Empty;

    /// <summary>
    /// User's role in the system
    /// </summary>
    public string Role { get; set; } = string.Empty;

    /// <summary>
    /// Current status of the user
    /// </summary>
    public string Status { get; set; } = string.Empty;

    /// <summary>
    /// Whether the user is currently available for work
    /// </summary>
    public bool IsAvailable { get; set; }

    /// <summary>
    /// Number of units this user is assigned to
    /// </summary>
    public int UnitCount { get; set; }
}