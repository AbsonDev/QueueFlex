using QueueManagement.Api.DTOs.Auth;

namespace QueueManagement.Api.Services;

/// <summary>
/// Service interface for JWT token management
/// </summary>
public interface IJwtService
{
    /// <summary>
    /// Generate JWT access token
    /// </summary>
    /// <param name="userId">User ID</param>
    /// <param name="email">User email</param>
    /// <param name="role">User role</param>
    /// <param name="tenantId">Tenant ID</param>
    /// <returns>JWT access token</returns>
    string GenerateAccessToken(Guid userId, string email, string role, Guid tenantId);

    /// <summary>
    /// Generate JWT refresh token
    /// </summary>
    /// <returns>JWT refresh token</returns>
    string GenerateRefreshToken();

    /// <summary>
    /// Validate JWT token
    /// </summary>
    /// <param name="token">JWT token to validate</param>
    /// <returns>True if token is valid, false otherwise</returns>
    bool ValidateToken(string token);

    /// <summary>
    /// Get user ID from JWT token
    /// </summary>
    /// <param name="token">JWT token</param>
    /// <returns>User ID from token claims</returns>
    Guid? GetUserIdFromToken(string token);

    /// <summary>
    /// Get tenant ID from JWT token
    /// </summary>
    /// <param name="token">JWT token</param>
    /// <returns>Tenant ID from token claims</returns>
    Guid? GetTenantIdFromToken(string token);

    /// <summary>
    /// Get user email from JWT token
    /// </summary>
    /// <param name="token">JWT token</param>
    /// <returns>User email from token claims</returns>
    string? GetUserEmailFromToken(string token);

    /// <summary>
    /// Get user role from JWT token
    /// </summary>
    /// <param name="token">JWT token</param>
    /// <returns>User role from token claims</returns>
    string? GetUserRoleFromToken(string token);

    /// <summary>
    /// Refresh access token using refresh token
    /// </summary>
    /// <param name="refreshToken">Refresh token</param>
    /// <returns>New access token if refresh token is valid</returns>
    string? RefreshAccessToken(string refreshToken);

    /// <summary>
    /// Revoke refresh token
    /// </summary>
    /// <param name="refreshToken">Refresh token to revoke</param>
    /// <returns>True if token was revoked, false otherwise</returns>
    bool RevokeRefreshToken(string refreshToken);
}