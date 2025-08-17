using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using System.Diagnostics;
using System.Text;

namespace QueueManagement.Api.Middleware;

/// <summary>
/// Middleware for logging HTTP requests and responses
/// </summary>
public class RequestLoggingMiddleware
{
    private readonly RequestDelegate _next;
    private readonly ILogger<RequestLoggingMiddleware> _logger;

    public RequestLoggingMiddleware(RequestDelegate next, ILogger<RequestLoggingMiddleware> logger)
    {
        _next = next;
        _logger = logger;
    }

    /// <summary>
    /// Process the HTTP request and log it
    /// </summary>
    /// <param name="context">HTTP context</param>
    public async Task InvokeAsync(HttpContext context)
    {
        var stopwatch = Stopwatch.StartNew();
        var requestBody = await GetRequestBody(context.Request);
        var originalBodyStream = context.Response.Body;

        try
        {
            using var memoryStream = new MemoryStream();
            context.Response.Body = memoryStream;

            await _next(context);

            stopwatch.Stop();
            var responseBody = await GetResponseBody(memoryStream);
            await memoryStream.CopyToAsync(originalBodyStream);

            LogRequestResponse(context, requestBody, responseBody, stopwatch.ElapsedMilliseconds);
        }
        catch (Exception ex)
        {
            stopwatch.Stop();
            LogRequestError(context, requestBody, ex, stopwatch.ElapsedMilliseconds);
            throw;
        }
        finally
        {
            context.Response.Body = originalBodyStream;
        }
    }

    /// <summary>
    /// Get request body content
    /// </summary>
    private async Task<string> GetRequestBody(HttpRequest request)
    {
        try
        {
            if (request.Body.CanSeek)
            {
                request.Body.Seek(0, SeekOrigin.Begin);
                using var reader = new StreamReader(request.Body, Encoding.UTF8, leaveOpen: true);
                var body = await reader.ReadToEndAsync();
                request.Body.Seek(0, SeekOrigin.Begin);
                return body;
            }
            return string.Empty;
        }
        catch
        {
            return string.Empty;
        }
    }

    /// <summary>
    /// Get response body content
    /// </summary>
    private async Task<string> GetResponseBody(Stream responseBody)
    {
        try
        {
            responseBody.Seek(0, SeekOrigin.Begin);
            using var reader = new StreamReader(responseBody, Encoding.UTF8, leaveOpen: true);
            var body = await reader.ReadToEndAsync();
            responseBody.Seek(0, SeekOrigin.Begin);
            return body;
        }
        catch
        {
            return string.Empty;
        }
    }

    /// <summary>
    /// Log successful request and response
    /// </summary>
    private void LogRequestResponse(HttpContext context, string requestBody, string responseBody, long elapsedMs)
    {
        var tenantId = context.Items.ContainsKey("TenantId") ? context.Items["TenantId"]?.ToString() : "Unknown";
        var userId = context.User?.FindFirst("sub")?.Value ?? "Anonymous";
        var userEmail = context.User?.FindFirst("email")?.Value ?? "Unknown";

        var logData = new
        {
            TenantId = tenantId,
            UserId = userId,
            UserEmail = userEmail,
            Method = context.Request.Method,
            Path = context.Request.Path,
            QueryString = context.Request.QueryString.ToString(),
            StatusCode = context.Response.StatusCode,
            ElapsedMs = elapsedMs,
            RequestSize = requestBody.Length,
            ResponseSize = responseBody.Length,
            UserAgent = context.Request.Headers["User-Agent"].ToString(),
            RemoteIpAddress = context.Connection.RemoteIpAddress?.ToString(),
            Timestamp = DateTime.UtcNow
        };

        if (context.Response.StatusCode >= 400)
        {
            _logger.LogWarning("HTTP {Method} {Path} - {StatusCode} - {ElapsedMs}ms - Tenant: {TenantId}, User: {UserEmail}",
                logData.Method, logData.Path, logData.StatusCode, logData.ElapsedMs, logData.TenantId, logData.UserEmail);
        }
        else
        {
            _logger.LogInformation("HTTP {Method} {Path} - {StatusCode} - {ElapsedMs}ms - Tenant: {TenantId}, User: {UserEmail}",
                logData.Method, logData.Path, logData.StatusCode, logData.ElapsedMs, logData.TenantId, logData.UserEmail);
        }

        // Log detailed request/response for debugging (only in development)
        if (_logger.IsEnabled(LogLevel.Debug))
        {
            _logger.LogDebug("Request Body: {RequestBody}", requestBody);
            _logger.LogDebug("Response Body: {ResponseBody}", responseBody);
        }
    }

    /// <summary>
    /// Log request error
    /// </summary>
    private void LogRequestError(HttpContext context, string requestBody, Exception exception, long elapsedMs)
    {
        var tenantId = context.Items.ContainsKey("TenantId") ? context.Items["TenantId"]?.ToString() : "Unknown";
        var userId = context.User?.FindFirst("sub")?.Value ?? "Anonymous";
        var userEmail = context.User?.FindFirst("email")?.Value ?? "Unknown";

        _logger.LogError(exception, "HTTP {Method} {Path} - ERROR - {ElapsedMs}ms - Tenant: {TenantId}, User: {UserEmail}",
            context.Request.Method, context.Request.Path, elapsedMs, tenantId, userEmail);

        if (_logger.IsEnabled(LogLevel.Debug))
        {
            _logger.LogDebug("Request Body: {RequestBody}", requestBody);
        }
    }
}

/// <summary>
/// Extension methods for RequestLoggingMiddleware
/// </summary>
public static class RequestLoggingMiddlewareExtensions
{
    /// <summary>
    /// Add request logging middleware to the application pipeline
    /// </summary>
    /// <param name="builder">Application builder</param>
    /// <returns>Application builder</returns>
    public static IApplicationBuilder UseRequestLogging(this IApplicationBuilder builder)
    {
        return builder.UseMiddleware<RequestLoggingMiddleware>();
    }
}