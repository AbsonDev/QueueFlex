using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using QueueManagement.Api.DTOs.Auth;
using QueueManagement.Api.DTOs.Common;
using MediatR;
using Swashbuckle.AspNetCore.Annotations;

namespace QueueManagement.Api.Controllers;

/// <summary>
/// Authentication controller for user login, registration, and token management
/// </summary>
[ApiController]
[Route("api/v1/[controller]")]
[AllowAnonymous]
[SwaggerTag("Authentication endpoints for user login, registration, and token management")]
public class AuthController : ControllerBase
{
    private readonly IMediator _mediator;
    private readonly ILogger<AuthController> _logger;

    public AuthController(IMediator mediator, ILogger<AuthController> logger)
    {
        _mediator = mediator;
        _logger = logger;
    }

    /// <summary>
    /// Authenticate user and get JWT token
    /// </summary>
    /// <param name="dto">Login credentials</param>
    /// <returns>JWT token and user information</returns>
    [HttpPost("login")]
    [ProducesResponseType(typeof(ApiResponse<AuthResponseDto>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status401Unauthorized)]
    [SwaggerOperation(
        Summary = "User login",
        Description = "Authenticates user credentials and returns JWT token with user information",
        OperationId = "Login"
    )]
    public async Task<ActionResult<ApiResponse<AuthResponseDto>>> Login([FromBody] LoginDto dto)
    {
        try
        {
            _logger.LogInformation("Login attempt for user: {Email} in tenant: {Subdomain}", dto.Email, dto.Subdomain);

            // TODO: Implement login command/query with MediatR
            // var command = new LoginCommand { Email = dto.Email, Password = dto.Password, Subdomain = dto.Subdomain };
            // var result = await _mediator.Send(command);

            // For now, return a mock response
            var mockResponse = new AuthResponseDto
            {
                AccessToken = "mock_jwt_token",
                RefreshToken = "mock_refresh_token",
                ExpiresIn = 3600,
                TokenType = "Bearer",
                User = new UserInfoDto
                {
                    Id = Guid.NewGuid(),
                    Name = "Mock User",
                    Email = dto.Email,
                    Role = "Admin",
                    EmployeeCode = "EMP001"
                },
                Tenant = new TenantInfoDto
                {
                    Id = Guid.NewGuid(),
                    Name = "Mock Company",
                    Subdomain = dto.Subdomain,
                    TimeZone = "UTC"
                }
            };

            _logger.LogInformation("Login successful for user: {Email}", dto.Email);
            return Ok(new ApiResponse<AuthResponseDto> { Data = mockResponse });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Login failed for user: {Email}", dto.Email);
            return BadRequest(new ApiResponse<AuthResponseDto>
            {
                Success = false,
                Error = new ApiError { Code = "LOGIN_FAILED", Message = "Invalid credentials" }
            });
        }
    }

    /// <summary>
    /// Register a new tenant with admin user
    /// </summary>
    /// <param name="dto">Tenant registration information</param>
    /// <returns>Registration result</returns>
    [HttpPost("register")]
    [ProducesResponseType(typeof(ApiResponse<AuthResponseDto>), StatusCodes.Status201Created)]
    [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status409Conflict)]
    [SwaggerOperation(
        Summary = "Register new tenant",
        Description = "Registers a new tenant with company information and creates an admin user",
        OperationId = "RegisterTenant"
    )]
    public async Task<ActionResult<ApiResponse<AuthResponseDto>>> Register([FromBody] RegisterTenantDto dto)
    {
        try
        {
            _logger.LogInformation("Tenant registration attempt for subdomain: {Subdomain}", dto.Subdomain);

            // TODO: Implement tenant registration command with MediatR
            // var command = new RegisterTenantCommand 
            // { 
            //     CompanyName = dto.CompanyName,
            //     Subdomain = dto.Subdomain,
            //     AdminName = dto.AdminName,
            //     AdminEmail = dto.AdminEmail,
            //     AdminPassword = dto.AdminPassword,
            //     TimeZone = dto.TimeZone
            // };
            // var result = await _mediator.Send(command);

            // For now, return a mock response
            var mockResponse = new AuthResponseDto
            {
                AccessToken = "mock_jwt_token",
                RefreshToken = "mock_refresh_token",
                ExpiresIn = 3600,
                TokenType = "Bearer",
                User = new UserInfoDto
                {
                    Id = Guid.NewGuid(),
                    Name = dto.AdminName,
                    Email = dto.AdminEmail,
                    Role = "Admin",
                    EmployeeCode = "ADMIN001"
                },
                Tenant = new TenantInfoDto
                {
                    Id = Guid.NewGuid(),
                    Name = dto.CompanyName,
                    Subdomain = dto.Subdomain,
                    TimeZone = dto.TimeZone
                }
            };

            _logger.LogInformation("Tenant registration successful for subdomain: {Subdomain}", dto.Subdomain);
            return CreatedAtAction(nameof(Login), new { }, new ApiResponse<AuthResponseDto> { Data = mockResponse });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Tenant registration failed for subdomain: {Subdomain}", dto.Subdomain);
            return BadRequest(new ApiResponse<AuthResponseDto>
            {
                Success = false,
                Error = new ApiError { Code = "REGISTRATION_FAILED", Message = "Registration failed" }
            });
        }
    }

    /// <summary>
    /// Refresh JWT token using refresh token
    /// </summary>
    /// <param name="dto">Refresh token</param>
    /// <returns>New JWT token</returns>
    [HttpPost("refresh")]
    [ProducesResponseType(typeof(ApiResponse<AuthResponseDto>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status401Unauthorized)]
    [SwaggerOperation(
        Summary = "Refresh token",
        Description = "Refreshes JWT token using a valid refresh token",
        OperationId = "RefreshToken"
    )]
    public async Task<ActionResult<ApiResponse<AuthResponseDto>>> Refresh([FromBody] RefreshTokenDto dto)
    {
        try
        {
            _logger.LogInformation("Token refresh attempt");

            // TODO: Implement token refresh command with MediatR
            // var command = new RefreshTokenCommand { RefreshToken = dto.RefreshToken };
            // var result = await _mediator.Send(command);

            // For now, return a mock response
            var mockResponse = new AuthResponseDto
            {
                AccessToken = "new_mock_jwt_token",
                RefreshToken = "new_mock_refresh_token",
                ExpiresIn = 3600,
                TokenType = "Bearer",
                User = new UserInfoDto
                {
                    Id = Guid.NewGuid(),
                    Name = "Mock User",
                    Email = "user@example.com",
                    Role = "Admin",
                    EmployeeCode = "EMP001"
                },
                Tenant = new TenantInfoDto
                {
                    Id = Guid.NewGuid(),
                    Name = "Mock Company",
                    Subdomain = "mock",
                    TimeZone = "UTC"
                }
            };

            _logger.LogInformation("Token refresh successful");
            return Ok(new ApiResponse<AuthResponseDto> { Data = mockResponse });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Token refresh failed");
            return BadRequest(new ApiResponse<AuthResponseDto>
            {
                Success = false,
                Error = new ApiError { Code = "REFRESH_FAILED", Message = "Token refresh failed" }
            });
        }
    }

    /// <summary>
    /// Validate JWT token
    /// </summary>
    /// <returns>Token validation result</returns>
    [HttpPost("validate")]
    [Authorize]
    [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status401Unauthorized)]
    [SwaggerOperation(
        Summary = "Validate token",
        Description = "Validates the current JWT token and returns user information",
        OperationId = "ValidateToken"
    )]
    public ActionResult<ApiResponse<object>> ValidateToken()
    {
        try
        {
            var userId = User.FindFirst("user_id")?.Value;
            var userEmail = User.FindFirst("email")?.Value;
            var userRole = User.FindFirst("role")?.Value;
            var tenantId = User.FindFirst("tenant_id")?.Value;

            if (string.IsNullOrEmpty(userId) || string.IsNullOrEmpty(userEmail))
            {
                return Unauthorized(new ApiResponse<object>
                {
                    Success = false,
                    Error = new ApiError { Code = "INVALID_TOKEN", Message = "Invalid token" }
                });
            }

            var userInfo = new
            {
                UserId = userId,
                Email = userEmail,
                Role = userRole,
                TenantId = tenantId,
                Valid = true
            };

            return Ok(new ApiResponse<object> { Data = userInfo });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Token validation failed");
            return BadRequest(new ApiResponse<object>
            {
                Success = false,
                Error = new ApiError { Code = "VALIDATION_FAILED", Message = "Token validation failed" }
            });
        }
    }
}