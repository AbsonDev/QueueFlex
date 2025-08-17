using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using QueueManagement.Api.Services;

namespace QueueManagement.Api.Services;

/// <summary>
/// Service implementation for JWT token management
/// </summary>
public class JwtService : IJwtService
{
    private readonly IConfiguration _configuration;
    private readonly ILogger<JwtService> _logger;
    private readonly string _secretKey;
    private readonly string _issuer;
    private readonly string _audience;
    private readonly int _accessTokenExpirationMinutes;
    private readonly int _refreshTokenExpirationDays;

    public JwtService(IConfiguration configuration, ILogger<JwtService> logger)
    {
        _configuration = configuration;
        _logger = logger;
        _secretKey = _configuration["Jwt:SecretKey"] ?? throw new InvalidOperationException("JWT secret key not configured");
        _issuer = _configuration["Jwt:Issuer"] ?? throw new InvalidOperationException("JWT issuer not configured");
        _audience = _configuration["Jwt:Audience"] ?? throw new InvalidOperationException("JWT audience not configured");
        _accessTokenExpirationMinutes = int.Parse(_configuration["Jwt:AccessTokenExpirationMinutes"] ?? "60");
        _refreshTokenExpirationDays = int.Parse(_configuration["Jwt:RefreshTokenExpirationDays"] ?? "7");
    }

    /// <summary>
    /// Generate JWT access token
    /// </summary>
    public string GenerateAccessToken(Guid userId, string email, string role, Guid tenantId)
    {
        try
        {
            var tokenHandler = new JwtSecurityTokenHandler();
            var key = Encoding.ASCII.GetBytes(_secretKey);

            var claims = new List<Claim>
            {
                new Claim(ClaimTypes.NameIdentifier, userId.ToString()),
                new Claim(ClaimTypes.Email, email),
                new Claim(ClaimTypes.Role, role),
                new Claim("tenant_id", tenantId.ToString()),
                new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()),
                new Claim(JwtRegisteredClaimNames.Iat, DateTimeOffset.UtcNow.ToUnixTimeSeconds().ToString(), ClaimValueTypes.Integer64)
            };

            var tokenDescriptor = new SecurityTokenDescriptor
            {
                Subject = new ClaimsIdentity(claims),
                Expires = DateTime.UtcNow.AddMinutes(_accessTokenExpirationMinutes),
                Issuer = _issuer,
                Audience = _audience,
                SigningCredentials = new SigningCredentials(new SymmetricSecurityKey(key), SecurityAlgorithms.HmacSha256Signature)
            };

            var token = tokenHandler.CreateToken(tokenDescriptor);
            var tokenString = tokenHandler.WriteToken(token);

            _logger.LogDebug("Generated access token for user {UserId} in tenant {TenantId}", userId, tenantId);
            return tokenString;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error generating access token for user {UserId}", userId);
            throw;
        }
    }

    /// <summary>
    /// Generate JWT refresh token
    /// </summary>
    public string GenerateRefreshToken()
    {
        try
        {
            var randomNumber = new byte[64];
            using var rng = RandomNumberGenerator.Create();
            rng.GetBytes(randomNumber);
            var refreshToken = Convert.ToBase64String(randomNumber);

            _logger.LogDebug("Generated refresh token");
            return refreshToken;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error generating refresh token");
            throw;
        }
    }

    /// <summary>
    /// Validate JWT token
    /// </summary>
    public bool ValidateToken(string token)
    {
        try
        {
            var tokenHandler = new JwtSecurityTokenHandler();
            var key = Encoding.ASCII.GetBytes(_secretKey);

            var validationParameters = new TokenValidationParameters
            {
                ValidateIssuerSigningKey = true,
                IssuerSigningKey = new SymmetricSecurityKey(key),
                ValidateIssuer = true,
                ValidIssuer = _issuer,
                ValidateAudience = true,
                ValidAudience = _audience,
                ValidateLifetime = true,
                ClockSkew = TimeSpan.Zero
            };

            var principal = tokenHandler.ValidateToken(token, validationParameters, out var validatedToken);
            var isValid = principal != null && validatedToken != null;

            _logger.LogDebug("Token validation result: {IsValid}", isValid);
            return isValid;
        }
        catch (Exception ex)
        {
            _logger.LogWarning(ex, "Token validation failed");
            return false;
        }
    }

    /// <summary>
    /// Get user ID from JWT token
    /// </summary>
    public Guid? GetUserIdFromToken(string token)
    {
        try
        {
            var claims = GetClaimsFromToken(token);
            var userIdClaim = claims.FirstOrDefault(c => c.Type == ClaimTypes.NameIdentifier)?.Value;
            
            if (Guid.TryParse(userIdClaim, out var userId))
            {
                return userId;
            }

            return null;
        }
        catch (Exception ex)
        {
            _logger.LogWarning(ex, "Error extracting user ID from token");
            return null;
        }
    }

    /// <summary>
    /// Get tenant ID from JWT token
    /// </summary>
    public Guid? GetTenantIdFromToken(string token)
    {
        try
        {
            var claims = GetClaimsFromToken(token);
            var tenantIdClaim = claims.FirstOrDefault(c => c.Type == "tenant_id")?.Value;
            
            if (Guid.TryParse(tenantIdClaim, out var tenantId))
            {
                return tenantId;
            }

            return null;
        }
        catch (Exception ex)
        {
            _logger.LogWarning(ex, "Error extracting tenant ID from token");
            return null;
        }
    }

    /// <summary>
    /// Get user email from JWT token
    /// </summary>
    public string? GetUserEmailFromToken(string token)
    {
        try
        {
            var claims = GetClaimsFromToken(token);
            return claims.FirstOrDefault(c => c.Type == ClaimTypes.Email)?.Value;
        }
 catch (Exception ex)
        {
            _logger.LogWarning(ex, "Error extracting user email from token");
            return null;
        }
    }

    /// <summary>
    /// Get user role from JWT token
    /// </summary>
    public string? GetUserRoleFromToken(string token)
    {
        try
        {
            var claims = GetClaimsFromToken(token);
            return claims.FirstOrDefault(c => c.Type == ClaimTypes.Role)?.Value;
        }
        catch (Exception ex)
        {
            _logger.LogWarning(ex, "Error extracting user role from token");
            return null;
        }
    }

    /// <summary>
    /// Refresh access token using refresh token
    /// </summary>
    public string? RefreshAccessToken(string refreshToken)
    {
        try
        {
            // TODO: Implement refresh token validation logic
            // This would typically involve:
            // 1. Validating the refresh token against stored refresh tokens
            // 2. Checking if the refresh token is expired
            // 3. Checking if the refresh token has been revoked
            // 4. Generating a new access token if validation passes

            _logger.LogWarning("Refresh token functionality not yet implemented");
            return null;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error refreshing access token");
            return null;
        }
    }

    /// <summary>
    /// Revoke refresh token
    /// </summary>
    public bool RevokeRefreshToken(string refreshToken)
    {
        try
        {
            // TODO: Implement refresh token revocation logic
            // This would typically involve:
            // 1. Adding the refresh token to a blacklist
            // 2. Removing the refresh token from the database
            // 3. Updating the token's status to revoked

            _logger.LogWarning("Refresh token revocation not yet implemented");
            return false;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error revoking refresh token");
            return false;
        }
    }

    /// <summary>
    /// Extract claims from JWT token without validation
    /// </summary>
    private IEnumerable<Claim> GetClaimsFromToken(string token)
    {
        try
        {
            var tokenHandler = new JwtSecurityTokenHandler();
            var jwtToken = tokenHandler.ReadJwtToken(token);
            return jwtToken.Claims;
        }
        catch (Exception ex)
        {
            _logger.LogWarning(ex, "Error reading JWT token");
            return Enumerable.Empty<Claim>();
        }
    }
}