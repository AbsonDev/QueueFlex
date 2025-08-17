using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using QueueManagement.Api.DTOs.Common;
using System.Net;
using System.Text.Json;

namespace QueueManagement.Api.Middleware;

/// <summary>
/// Middleware for global exception handling
/// </summary>
public class ExceptionHandlingMiddleware
{
    private readonly RequestDelegate _next;
    private readonly ILogger<ExceptionHandlingMiddleware> _logger;
    private readonly IHostEnvironment _environment;

    public ExceptionHandlingMiddleware(
        RequestDelegate next,
        ILogger<ExceptionHandlingMiddleware> logger,
        IHostEnvironment environment)
    {
        _next = next;
        _logger = logger;
        _environment = environment;
    }

    /// <summary>
    /// Process the HTTP request and handle any exceptions
    /// </summary>
    /// <param name="context">HTTP context</param>
    public async Task InvokeAsync(HttpContext context)
    {
        try
        {
            await _next(context);
        }
        catch (Exception ex)
        {
            await HandleExceptionAsync(context, ex);
        }
    }

    /// <summary>
    /// Handle the exception and return appropriate response
    /// </summary>
    /// <param name="context">HTTP context</param>
    /// <param name="exception">Exception that occurred</param>
    private async Task HandleExceptionAsync(HttpContext context, Exception exception)
    {
        var (statusCode, message, errorCode) = GetExceptionDetails(exception);

        // Log the exception
        LogException(exception, context, statusCode);

        // Set response properties
        context.Response.ContentType = "application/json";
        context.Response.StatusCode = statusCode;

        // Create error response
        var errorResponse = CreateErrorResponse(exception, message, errorCode, statusCode);

        // Serialize and send response
        var jsonResponse = JsonSerializer.Serialize(errorResponse, new JsonSerializerOptions
        {
            PropertyNamingPolicy = JsonNamingPolicy.CamelCase
        });

        await context.Response.WriteAsync(jsonResponse);
    }

    /// <summary>
    /// Get exception details for response
    /// </summary>
    /// <param name="exception">Exception that occurred</param>
    /// <returns>Tuple of status code, message, and error code</returns>
    private (int statusCode, string message, string errorCode) GetExceptionDetails(Exception exception)
    {
        return exception switch
        {
            UnauthorizedAccessException => (StatusCodes.Status401Unauthorized, "Unauthorized access", "UNAUTHORIZED"),
            ArgumentException => (StatusCodes.Status400BadRequest, "Invalid argument provided", "INVALID_ARGUMENT"),
            ArgumentNullException => (StatusCodes.Status400BadRequest, "Required argument is missing", "MISSING_ARGUMENT"),
            ArgumentOutOfRangeException => (StatusCodes.Status400BadRequest, "Argument value is out of valid range", "INVALID_RANGE"),
            InvalidOperationException => (StatusCodes.Status400BadRequest, "Invalid operation", "INVALID_OPERATION"),
            NotSupportedException => (StatusCodes.Status405MethodNotAllowed, "Operation not supported", "NOT_SUPPORTED"),
            TimeoutException => (StatusCodes.Status408RequestTimeout, "Request timed out", "TIMEOUT"),
            KeyNotFoundException => (StatusCodes.Status404NotFound, "Resource not found", "NOT_FOUND"),
            FileNotFoundException => (StatusCodes.Status404NotFound, "File not found", "FILE_NOT_FOUND"),
            DirectoryNotFoundException => (StatusCodes.Status404NotFound, "Directory not found", "DIRECTORY_NOT_FOUND"),
            HttpRequestException => (StatusCodes.Status502BadGateway, "External service error", "EXTERNAL_SERVICE_ERROR"),
            TaskCanceledException => (StatusCodes.Status408RequestTimeout, "Request was cancelled", "CANCELLED"),
            OperationCanceledException => (StatusCodes.Status408RequestTimeout, "Operation was cancelled", "CANCELLED"),
            JsonException => (StatusCodes.Status400BadRequest, "Invalid JSON format", "INVALID_JSON"),
            FormatException => (StatusCodes.Status400BadRequest, "Invalid format", "INVALID_FORMAT"),
            _ => (StatusCodes.Status500InternalServerError, "An unexpected error occurred", "INTERNAL_ERROR")
        };
    }

    /// <summary>
    /// Log the exception with appropriate level
    /// </summary>
    /// <param name="exception">Exception that occurred</param>
    /// <param name="context">HTTP context</param>
    /// <param name="statusCode">HTTP status code</param>
    private void LogException(Exception exception, HttpContext context, int statusCode)
    {
        var logLevel = statusCode >= 500 ? LogLevel.Error : LogLevel.Warning;

        _logger.Log(logLevel, exception, 
            "Exception occurred while processing request {Method} {Path}. Status: {StatusCode}. User: {User}. Tenant: {Tenant}",
            context.Request.Method,
            context.Request.Path,
            statusCode,
            context.User?.Identity?.Name ?? "Anonymous",
            context.Items["TenantId"] ?? "Unknown");
    }

    /// <summary>
    /// Create standardized error response
    /// </summary>
    /// <param name="exception">Exception that occurred</param>
    /// <param name="message">User-friendly message</param>
    /// <param name="errorCode">Error code</param>
    /// <param name="statusCode">HTTP status code</param>
    /// <returns>Error response object</returns>
    private ApiResponse<object> CreateErrorResponse(Exception exception, string message, string errorCode, int statusCode)
    {
        var errorResponse = new ApiResponse<object>
        {
            Success = false,
            Error = new ApiError
            {
                Code = errorCode,
                Message = message
            }
        };

        // Add additional details in development environment
        if (_environment.IsDevelopment())
        {
            errorResponse.Error.Details = new List<ValidationError>
            {
                new ValidationError
                {
                    Field = "ExceptionType",
                    Message = exception.GetType().Name
                },
                new ValidationError
                {
                    Field = "StackTrace",
                    Message = exception.StackTrace ?? "No stack trace available"
                }
            };

            // Add inner exception details if available
            if (exception.InnerException != null)
            {
                errorResponse.Error.Details.Add(new ValidationError
                {
                    Field = "InnerException",
                    Message = exception.InnerException.Message
                });
            }
        }

        // Add correlation ID if available
        if (context.Items.ContainsKey("CorrelationId"))
        {
            errorResponse.Meta = new ApiMeta
            {
                Timestamp = DateTime.UtcNow
            };
        }

        return errorResponse;
    }
}

/// <summary>
/// Extension methods for ExceptionHandlingMiddleware
/// </summary>
public static class ExceptionHandlingMiddlewareExtensions
{
    /// <summary>
    /// Add global exception handling middleware to the application pipeline
    /// </summary>
    /// <param name="builder">Application builder</param>
    /// <returns>Application builder</returns>
    public static IApplicationBuilder UseGlobalExceptionHandling(this IApplicationBuilder builder)
    {
        return builder.UseMiddleware<ExceptionHandlingMiddleware>();
    }
}