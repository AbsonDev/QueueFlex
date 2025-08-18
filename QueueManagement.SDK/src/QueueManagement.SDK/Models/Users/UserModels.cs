using QueueManagement.SDK.Models.Common;

namespace QueueManagement.SDK.Models.Users;

/// <summary>
/// Represents a user in the system.
/// </summary>
public class UserResponse
{
    /// <summary>
    /// Gets or sets the unique identifier of the user.
    /// </summary>
    public Guid Id { get; set; }

    /// <summary>
    /// Gets or sets the username.
    /// </summary>
    public string Username { get; set; } = string.Empty;

    /// <summary>
    /// Gets or sets the email address.
    /// </summary>
    public string Email { get; set; } = string.Empty;

    /// <summary>
    /// Gets or sets the first name.
    /// </summary>
    public string FirstName { get; set; } = string.Empty;

    /// <summary>
    /// Gets or sets the last name.
    /// </summary>
    public string LastName { get; set; } = string.Empty;

    /// <summary>
    /// Gets or sets the full name.
    /// </summary>
    public string FullName => $"{FirstName} {LastName}".Trim();

    /// <summary>
    /// Gets or sets the user's role.
    /// </summary>
    public UserRole Role { get; set; }

    /// <summary>
    /// Gets or sets the unit ID the user is assigned to.
    /// </summary>
    public Guid? UnitId { get; set; }

    /// <summary>
    /// Gets or sets the unit name.
    /// </summary>
    public string? UnitName { get; set; }

    /// <summary>
    /// Gets or sets the list of queue IDs the user can serve.
    /// </summary>
    public List<Guid> QueueIds { get; set; } = new();

    /// <summary>
    /// Gets or sets the list of service IDs the user can provide.
    /// </summary>
    public List<Guid> ServiceIds { get; set; } = new();

    /// <summary>
    /// Gets or sets whether the user is currently active.
    /// </summary>
    public bool IsActive { get; set; }

    /// <summary>
    /// Gets or sets whether the user is currently online.
    /// </summary>
    public bool IsOnline { get; set; }

    /// <summary>
    /// Gets or sets the phone number.
    /// </summary>
    public string? PhoneNumber { get; set; }

    /// <summary>
    /// Gets or sets the employee ID.
    /// </summary>
    public string? EmployeeId { get; set; }

    /// <summary>
    /// Gets or sets the department.
    /// </summary>
    public string? Department { get; set; }

    /// <summary>
    /// Gets or sets the preferred language.
    /// </summary>
    public string PreferredLanguage { get; set; } = "en";

    /// <summary>
    /// Gets or sets custom metadata.
    /// </summary>
    public Dictionary<string, object>? Metadata { get; set; }

    /// <summary>
    /// Gets or sets when the user was created.
    /// </summary>
    public DateTime CreatedAt { get; set; }

    /// <summary>
    /// Gets or sets when the user was last updated.
    /// </summary>
    public DateTime UpdatedAt { get; set; }

    /// <summary>
    /// Gets or sets when the user last logged in.
    /// </summary>
    public DateTime? LastLoginAt { get; set; }
}

/// <summary>
/// Request to create a new user.
/// </summary>
public class CreateUserRequest
{
    /// <summary>
    /// Gets or sets the username.
    /// </summary>
    public string Username { get; set; } = string.Empty;

    /// <summary>
    /// Gets or sets the email address.
    /// </summary>
    public string Email { get; set; } = string.Empty;

    /// <summary>
    /// Gets or sets the password.
    /// </summary>
    public string Password { get; set; } = string.Empty;

    /// <summary>
    /// Gets or sets the first name.
    /// </summary>
    public string FirstName { get; set; } = string.Empty;

    /// <summary>
    /// Gets or sets the last name.
    /// </summary>
    public string LastName { get; set; } = string.Empty;

    /// <summary>
    /// Gets or sets the user's role.
    /// </summary>
    public UserRole Role { get; set; }

    /// <summary>
    /// Gets or sets the unit ID to assign the user to (optional).
    /// </summary>
    public Guid? UnitId { get; set; }

    /// <summary>
    /// Gets or sets the list of queue IDs the user can serve (optional).
    /// </summary>
    public List<Guid>? QueueIds { get; set; }

    /// <summary>
    /// Gets or sets the list of service IDs the user can provide (optional).
    /// </summary>
    public List<Guid>? ServiceIds { get; set; }

    /// <summary>
    /// Gets or sets the phone number (optional).
    /// </summary>
    public string? PhoneNumber { get; set; }

    /// <summary>
    /// Gets or sets the employee ID (optional).
    /// </summary>
    public string? EmployeeId { get; set; }

    /// <summary>
    /// Gets or sets the department (optional).
    /// </summary>
    public string? Department { get; set; }

    /// <summary>
    /// Gets or sets the preferred language (default: en).
    /// </summary>
    public string PreferredLanguage { get; set; } = "en";

    /// <summary>
    /// Gets or sets custom metadata (optional).
    /// </summary>
    public Dictionary<string, object>? Metadata { get; set; }
}

/// <summary>
/// Request to update an existing user.
/// </summary>
public class UpdateUserRequest
{
    /// <summary>
    /// Gets or sets the updated email (optional).
    /// </summary>
    public string? Email { get; set; }

    /// <summary>
    /// Gets or sets the updated first name (optional).
    /// </summary>
    public string? FirstName { get; set; }

    /// <summary>
    /// Gets or sets the updated last name (optional).
    /// </summary>
    public string? LastName { get; set; }

    /// <summary>
    /// Gets or sets the updated role (optional).
    /// </summary>
    public UserRole? Role { get; set; }

    /// <summary>
    /// Gets or sets the updated unit ID (optional).
    /// </summary>
    public Guid? UnitId { get; set; }

    /// <summary>
    /// Gets or sets the updated queue IDs (optional).
    /// </summary>
    public List<Guid>? QueueIds { get; set; }

    /// <summary>
    /// Gets or sets the updated service IDs (optional).
    /// </summary>
    public List<Guid>? ServiceIds { get; set; }

    /// <summary>
    /// Gets or sets whether the user is active (optional).
    /// </summary>
    public bool? IsActive { get; set; }

    /// <summary>
    /// Gets or sets the updated phone number (optional).
    /// </summary>
    public string? PhoneNumber { get; set; }

    /// <summary>
    /// Gets or sets the updated department (optional).
    /// </summary>
    public string? Department { get; set; }

    /// <summary>
    /// Gets or sets the updated preferred language (optional).
    /// </summary>
    public string? PreferredLanguage { get; set; }

    /// <summary>
    /// Gets or sets updated metadata (optional).
    /// </summary>
    public Dictionary<string, object>? Metadata { get; set; }
}

/// <summary>
/// Request to change user password.
/// </summary>
public class ChangePasswordRequest
{
    /// <summary>
    /// Gets or sets the current password.
    /// </summary>
    public string CurrentPassword { get; set; } = string.Empty;

    /// <summary>
    /// Gets or sets the new password.
    /// </summary>
    public string NewPassword { get; set; } = string.Empty;
}