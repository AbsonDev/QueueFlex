using System.ComponentModel.DataAnnotations;

namespace QueueManagement.Api.DTOs.Auth;

/// <summary>
/// Login request DTO
/// </summary>
public class LoginDto
{
    /// <summary>
    /// User's email address
    /// </summary>
    [Required]
    [EmailAddress]
    public string Email { get; set; } = string.Empty;

    /// <summary>
    /// User's password
    /// </summary>
    [Required]
    [MinLength(6)]
    public string Password { get; set; } = string.Empty;

    /// <summary>
    /// Tenant subdomain for multi-tenant authentication
    /// </summary>
    [Required]
    public string Subdomain { get; set; } = string.Empty;
}

/// <summary>
/// Authentication response DTO
/// </summary>
public class AuthResponseDto
{
    /// <summary>
    /// JWT access token
    /// </summary>
    public string AccessToken { get; set; } = string.Empty;

    /// <summary>
    /// JWT refresh token
    /// </summary>
    public string RefreshToken { get; set; } = string.Empty;

    /// <summary>
    /// Token expiration time in seconds
    /// </summary>
    public int ExpiresIn { get; set; }

    /// <summary>
    /// Token type (Bearer)
    /// </summary>
    public string TokenType { get; set; } = "Bearer";

    /// <summary>
    /// User information
    /// </summary>
    public UserInfoDto User { get; set; } = null!;

    /// <summary>
    /// Tenant information
    /// </summary>
    public TenantInfoDto Tenant { get; set; } = null!;
}

/// <summary>
/// User information in auth response
/// </summary>
public class UserInfoDto
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
    /// User's role
    /// </summary>
    public string Role { get; set; } = string.Empty;

    /// <summary>
    /// User's employee code
    /// </summary>
    public string EmployeeCode { get; set; } = string.Empty;
}

/// <summary>
/// Tenant information in auth response
/// </summary>
public class TenantInfoDto
{
    /// <summary>
    /// Tenant ID
    /// </summary>
    public Guid Id { get; set; }

    /// <summary>
    /// Tenant name
    /// </summary>
    public string Name { get; set; } = string.Empty;

    /// <summary>
    /// Tenant subdomain
    /// </summary>
    public string Subdomain { get; set; } = string.Empty;

    /// <summary>
    /// Tenant timezone
    /// </summary>
    public string TimeZone { get; set; } = string.Empty;
}

/// <summary>
/// Refresh token request DTO
/// </summary>
public class RefreshTokenDto
{
    /// <summary>
    /// Refresh token
    /// </summary>
    [Required]
    public string RefreshToken { get; set; } = string.Empty;
}

/// <summary>
/// Register tenant request DTO
/// </summary>
public class RegisterTenantDto
{
    /// <summary>
    /// Company name
    /// </summary>
    [Required]
    [MaxLength(200)]
    public string CompanyName { get; set; } = string.Empty;

    /// <summary>
    /// Unique subdomain
    /// </summary>
    [Required]
    [MaxLength(50)]
    [RegularExpression(@"^[a-z0-9-]+$", ErrorMessage = "Subdomain can only contain lowercase letters, numbers, and hyphens")]
    public string Subdomain { get; set; } = string.Empty;

    /// <summary>
    /// Admin user's full name
    /// </summary>
    [Required]
    [MaxLength(200)]
    public string AdminName { get; set; } = string.Empty;

    /// <summary>
    /// Admin user's email address
    /// </summary>
    [Required]
    [EmailAddress]
    public string AdminEmail { get; set; } = string.Empty;

    /// <summary>
    /// Admin user's password
    /// </summary>
    [Required]
    [MinLength(8)]
    public string AdminPassword { get; set; } = string.Empty;

    /// <summary>
    /// Tenant timezone
    /// </summary>
    [Required]
    [MaxLength(50)]
    public string TimeZone { get; set; } = string.Empty;
}