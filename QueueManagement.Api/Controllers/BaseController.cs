using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using QueueManagement.Api.DTOs.Common;
using MediatR;

namespace QueueManagement.Api.Controllers;

/// <summary>
/// Base controller with common functionality for all API controllers
/// </summary>
[ApiController]
[Route("api/v1/[controller]")]
[Authorize]
public abstract class BaseController : ControllerBase
{
    /// <summary>
    /// Mediator instance for handling commands and queries
    /// </summary>
    protected readonly IMediator _mediator;
    
    /// <summary>
    /// Logger instance
    /// </summary>
    protected readonly ILogger _logger;

    /// <summary>
    /// Constructor
    /// </summary>
    protected BaseController(IMediator mediator, ILogger logger)
    {
        _mediator = mediator;
        _logger = logger;
    }

    /// <summary>
    /// Gets the tenant ID from the JWT token claims
    /// </summary>
    /// <returns>Tenant ID</returns>
    /// <exception cref="UnauthorizedAccessException">Thrown when tenant ID is not found</exception>
    protected Guid GetTenantId()
    {
        var tenantIdClaim = User.FindFirst("tenant_id")?.Value;
        if (string.IsNullOrEmpty(tenantIdClaim) || !Guid.TryParse(tenantIdClaim, out var tenantId))
        {
            _logger.LogWarning("Tenant ID not found in JWT token claims");
            throw new UnauthorizedAccessException("Tenant ID not found");
        }
        return tenantId;
    }

    /// <summary>
    /// Gets the user ID from the JWT token claims
    /// </summary>
    /// <returns>User ID</returns>
    /// <exception cref="UnauthorizedAccessException">Thrown when user ID is not found</exception>
    protected string GetUserId()
    {
        var userIdClaim = User.FindFirst("user_id")?.Value;
        if (string.IsNullOrEmpty(userIdClaim))
        {
            _logger.LogWarning("User ID not found in JWT token claims");
            throw new UnauthorizedAccessException("User ID not found");
        }
        return userIdClaim;
    }

    /// <summary>
    /// Gets the user email from the JWT token claims
    /// </summary>
    /// <returns>User email</returns>
    /// <exception cref="UnauthorizedAccessException">Thrown when user email is not found</exception>
    protected string GetUserEmail()
    {
        var userEmailClaim = User.FindFirst("email")?.Value;
        if (string.IsNullOrEmpty(userEmailClaim))
        {
            _logger.LogWarning("User email not found in JWT token claims");
            throw new UnauthorizedAccessException("User email not found");
        }
        return userEmailClaim;
    }

    /// <summary>
    /// Gets the user role from the JWT token claims
    /// </summary>
    /// <returns>User role</returns>
    /// <exception cref="UnauthorizedAccessException">Thrown when user role is not found</exception>
    protected string GetUserRole()
    {
        var userRoleClaim = User.FindFirst("role")?.Value;
        if (string.IsNullOrEmpty(userRoleClaim))
        {
            _logger.LogWarning("User role not found in JWT token claims");
            throw new UnauthorizedAccessException("User role not found");
        }
        return userRoleClaim;
    }

    /// <summary>
    /// Creates a successful response
    /// </summary>
    /// <typeparam name="T">Type of the data</typeparam>
    /// <param name="data">Data to return</param>
    /// <returns>Success response</returns>
    protected ActionResult<ApiResponse<T>> Success<T>(T data)
    {
        return Ok(new ApiResponse<T> { Data = data });
    }

    /// <summary>
    /// Creates a successful response with metadata
    /// </summary>
    /// <typeparam name="T">Type of the data</typeparam>
    /// <param name="data">Data to return</param>
    /// <param name="meta">Metadata to include</param>
    /// <returns>Success response with metadata</returns>
    protected ActionResult<ApiResponse<T>> Success<T>(T data, ApiMeta meta)
    {
        return Ok(new ApiResponse<T> { Data = data, Meta = meta });
    }

    /// <summary>
    /// Creates an error response
    /// </summary>
    /// <typeparam name="T">Type of the data</typeparam>
    /// <param name="message">Error message</param>
    /// <param name="code">Error code</param>
    /// <returns>Error response</returns>
    protected ActionResult<ApiResponse<T>> Error<T>(string message, string code = "ERROR")
    {
        return BadRequest(new ApiResponse<T> 
        { 
            Success = false, 
            Error = new ApiError { Code = code, Message = message } 
        });
    }

    /// <summary>
    /// Creates an error response with validation details
    /// </summary>
    /// <typeparam name="T">Type of the data</typeparam>
    /// <param name="message">Error message</param>
    /// <param name="code">Error code</param>
    /// <param name="validationErrors">Validation error details</param>
    /// <returns>Error response with validation details</returns>
    protected ActionResult<ApiResponse<T>> Error<T>(string message, string code, List<ValidationError> validationErrors)
    {
        return BadRequest(new ApiResponse<T> 
        { 
            Success = false, 
            Error = new ApiError 
            { 
                Code = code, 
                Message = message,
                Details = validationErrors
            } 
        });
    }

    /// <summary>
    /// Creates a not found response
    /// </summary>
    /// <typeparam name="T">Type of the data</typeparam>
    /// <param name="message">Not found message</param>
    /// <returns>Not found response</returns>
    protected ActionResult<ApiResponse<T>> NotFound<T>(string message = "Resource not found")
    {
        return NotFound(new ApiResponse<T> 
        { 
            Success = false, 
            Error = new ApiError { Code = "NOT_FOUND", Message = message } 
        });
    }

    /// <summary>
    /// Creates an unauthorized response
    /// </summary>
    /// <typeparam name="T">Type of the data</typeparam>
    /// <param name="message">Unauthorized message</param>
    /// <returns>Unauthorized response</returns>
    protected ActionResult<ApiResponse<T>> Unauthorized<T>(string message = "Unauthorized access")
    {
        return Unauthorized(new ApiResponse<T> 
        { 
            Success = false, 
            Error = new ApiError { Code = "UNAUTHORIZED", Message = message } 
        });
    }

    /// <summary>
    /// Creates a forbidden response
    /// </summary>
    /// <typeparam name="T">Type of the data</typeparam>
    /// <param name="message">Forbidden message</param>
    /// <returns>Forbidden response</returns>
    protected ActionResult<ApiResponse<T>> Forbidden<T>(string message = "Access forbidden")
    {
        return StatusCode(403, new ApiResponse<T> 
        { 
            Success = false, 
            Error = new ApiError { Code = "FORBIDDEN", Message = message } 
        });
    }

    /// <summary>
    /// Creates a conflict response
    /// </summary>
    /// <typeparam name="T">Type of the data</typeparam>
    /// <param name="message">Conflict message</param>
    /// <returns>Conflict response</returns>
    protected ActionResult<ApiResponse<T>> Conflict<T>(string message = "Resource conflict")
    {
        return Conflict(new ApiResponse<T> 
        { 
            Success = false, 
            Error = new ApiError { Code = "CONFLICT", Message = message } 
        });
    }

    /// <summary>
    /// Creates a created response
    /// </summary>
    /// <typeparam name="T">Type of the data</typeparam>
    /// <param name="data">Data to return</param>
    /// <param name="actionName">Action name for the location header</param>
    /// <param name="routeValues">Route values for the location header</param>
    /// <returns>Created response</returns>
    protected ActionResult<ApiResponse<T>> Created<T>(T data, string actionName, object routeValues)
    {
        return CreatedAtAction(actionName, routeValues, new ApiResponse<T> { Data = data });
    }

    /// <summary>
    /// Creates a no content response
    /// </summary>
    /// <returns>No content response</returns>
    protected ActionResult<ApiResponse<object>> NoContent()
    {
        return NoContent();
    }

    /// <summary>
    /// Logs an error and returns an error response
    /// </summary>
    /// <typeparam name="T">Type of the data</typeparam>
    /// <param name="ex">Exception that occurred</param>
    /// <param name="message">Error message</param>
    /// <returns>Error response</returns>
    protected ActionResult<ApiResponse<T>> LogAndReturnError<T>(Exception ex, string message = "An error occurred")
    {
        _logger.LogError(ex, message);
        return Error<T>(message, "INTERNAL_ERROR");
    }
}