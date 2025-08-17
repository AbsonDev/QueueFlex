using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using System.Security.Claims;

namespace QueueManagement.Api.Middleware;

/// <summary>
/// Middleware for resolving tenant information from various sources
/// </summary>
public class TenantResolutionMiddleware
{
    private readonly RequestDelegate _next;
    private readonly ILogger<TenantResolutionMiddleware> _logger;

    public TenantResolutionMiddleware(RequestDelegate next, ILogger<TenantResolutionMiddleware> logger)
    {
        _next = next;
        _logger = logger;
    }

    /// <summary>
    /// Process the HTTP request to resolve tenant information
    /// </summary>
    /// <param name="context">HTTP context</param>
    public async Task InvokeAsync(HttpContext context)
    {
        try
        {
            var tenantId = await ResolveTenantId(context);
            
            if (!string.IsNullOrEmpty(tenantId))
            {
                // Add tenant ID to HttpContext.Items for use in controllers
                context.Items["TenantId"] = tenantId;
                
                _logger.LogDebug("Tenant ID resolved: {TenantId} for request {Path}", tenantId, context.Request.Path);
            }
            else
            {
                _logger.LogWarning("No tenant ID could be resolved for request {Path}", context.Request.Path);
            }

            await _next(context);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error resolving tenant for request {Path}", context.Request.Path);
            await _next(context);
        }
    }

    /// <summary>
    /// Resolve tenant ID from various sources
    /// </summary>
    /// <param name="context">HTTP context</param>
    /// <returns>Tenant ID if found, null otherwise</returns>
    private async Task<string?> ResolveTenantId(HttpContext context)
    {
        // Priority order for tenant resolution:
        // 1. JWT token claims (highest priority)
        // 2. Custom header
        // 3. Subdomain
        // 4. Query parameter

        // 1. Check JWT token claims first
        var tenantIdFromClaims = GetTenantIdFromClaims(context);
        if (!string.IsNullOrEmpty(tenantIdFromClaims))
        {
            return tenantIdFromClaims;
        }

        // 2. Check custom header
        var tenantIdFromHeader = GetTenantIdFromHeader(context);
        if (!string.IsNullOrEmpty(tenantIdFromHeader))
        {
            return tenantIdFromHeader;
        }

        // 3. Check subdomain
        var tenantIdFromSubdomain = GetTenantIdFromSubdomain(context);
        if (!string.IsNullOrEmpty(tenantIdFromSubdomain))
        {
            return tenantIdFromSubdomain;
        }

        // 4. Check query parameter
        var tenantIdFromQuery = GetTenantIdFromQuery(context);
        if (!string.IsNullOrEmpty(tenantIdFromQuery))
        {
            return tenantIdFromQuery;
        }

        return null;
    }

    /// <summary>
    /// Get tenant ID from JWT token claims
    /// </summary>
    /// <param name="context">HTTP context</param>
    /// <returns>Tenant ID if found in claims, null otherwise</returns>
    private string? GetTenantIdFromClaims(HttpContext context)
    {
        try
        {
            var tenantIdClaim = context.User?.FindFirst("tenant_id")?.Value;
            return !string.IsNullOrEmpty(tenantIdClaim) ? tenantIdClaim : null;
        }
        catch (Exception ex)
        {
            _logger.LogDebug(ex, "Error getting tenant ID from claims");
            return null;
        }
    }

    /// <summary>
    /// Get tenant ID from custom header
    /// </summary>
    /// <param name="context">HTTP context</param>
    /// <returns>Tenant ID if found in header, null otherwise</returns>
    private string? GetTenantIdFromHeader(HttpContext context)
    {
        try
        {
            // Check for custom tenant header
            var tenantHeader = context.Request.Headers["X-Tenant-ID"].FirstOrDefault();
            if (!string.IsNullOrEmpty(tenantHeader))
            {
                return tenantHeader;
            }

            // Check for tenant header
            var tenantHeader2 = context.Request.Headers["Tenant-ID"].FirstOrDefault();
            if (!string.IsNullOrEmpty(tenantHeader2))
            {
                return tenantHeader2;
            }

            return null;
        }
        catch (Exception ex)
        {
            _logger.LogDebug(ex, "Error getting tenant ID from header");
            return null;
        }
    }

    /// <summary>
    /// Get tenant ID from subdomain
    /// </summary>
    /// <param name="context">HTTP context</param>
    /// <returns>Tenant ID if found in subdomain, null otherwise</returns>
    private string? GetTenantIdFromSubdomain(HttpContext context)
    {
        try
        {
            var host = context.Request.Host.Host;
            if (string.IsNullOrEmpty(host))
            {
                return null;
            }

            // Extract subdomain from host
            var subdomain = ExtractSubdomain(host);
            if (string.IsNullOrEmpty(subdomain))
            {
                return null;
            }

            // TODO: Implement subdomain to tenant ID mapping
            // This would typically query a database or cache to map subdomain to tenant ID
            // For now, return the subdomain as-is (assuming it's the tenant ID)

            _logger.LogDebug("Extracted subdomain: {Subdomain} from host: {Host}", subdomain, host);
            return subdomain;
        }
        catch (Exception ex)
        {
            _logger.LogDebug(ex, "Error getting tenant ID from subdomain");
            return null;
        }
    }

    /// <summary>
    /// Get tenant ID from query parameter
    /// </summary>
    /// <param name="context">HTTP context</param>
    /// <returns>Tenant ID if found in query, null otherwise</returns>
    private string? GetTenantIdFromQuery(HttpContext context)
    {
        try
        {
            var tenantQuery = context.Request.Query["tenant"].FirstOrDefault();
            if (!string.IsNullOrEmpty(tenantQuery))
            {
                return tenantQuery;
            }

            var tenantIdQuery = context.Request.Query["tenantId"].FirstOrDefault();
            if (!string.IsNullOrEmpty(tenantIdQuery))
            {
                return tenantIdQuery;
            }

            return null;
        }
        catch (Exception ex)
        {
            _logger.LogDebug(ex, "Error getting tenant ID from query");
            return null;
        }
    }

    /// <summary>
    /// Extract subdomain from host
    /// </summary>
    /// <param name="host">Host string</param>
    /// <returns>Subdomain if found, null otherwise</returns>
    private string? ExtractSubdomain(string host)
    {
        try
        {
            // Handle localhost and IP addresses
            if (host.Equals("localhost", StringComparison.OrdinalIgnoreCase) ||
                host.Equals("127.0.0.1", StringComparison.OrdinalIgnoreCase) ||
                host.Equals("::1", StringComparison.OrdinalIgnoreCase))
            {
                return null;
            }

            // Split host by dots
            var parts = host.Split('.');
            
            // If we have at least 2 parts and the last part is a TLD, extract subdomain
            if (parts.Length >= 2)
            {
                // Check if the last part looks like a TLD (2-3 characters, no numbers)
                var lastPart = parts[^1];
                if (lastPart.Length >= 2 && lastPart.Length <= 3 && !lastPart.Any(char.IsDigit))
                {
                    // Extract subdomain (first part)
                    var subdomain = parts[0];
                    
                    // Skip common subdomains that aren't tenant-specific
                    var commonSubdomains = new[] { "www", "api", "admin", "app", "portal" };
                    if (!commonSubdomains.Contains(subdomain, StringComparer.OrdinalIgnoreCase))
                    {
                        return subdomain;
                    }
                }
            }

            return null;
        }
        catch (Exception ex)
        {
            _logger.LogDebug(ex, "Error extracting subdomain from host: {Host}", host);
            return null;
        }
    }
}

/// <summary>
/// Extension methods for TenantResolutionMiddleware
/// </summary>
public static class TenantResolutionMiddlewareExtensions
{
    /// <summary>
    /// Add tenant resolution middleware to the application pipeline
    /// </summary>
    /// <param name="builder">Application builder</param>
    /// <returns>Application builder</returns>
    public static IApplicationBuilder UseTenantResolution(this IApplicationBuilder builder)
    {
        return builder.UseMiddleware<TenantResolutionMiddleware>();
    }
}