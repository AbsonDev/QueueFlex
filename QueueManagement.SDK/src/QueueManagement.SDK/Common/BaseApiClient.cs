using System.Net;
using System.Net.Http.Headers;
using System.Text;
using System.Text.Json;
using System.Text.Json.Serialization;
using Microsoft.Extensions.Logging;
using Polly;
using Polly.Extensions.Http;
using QueueManagement.SDK.Configuration;
using QueueManagement.SDK.Exceptions;
using QueueManagement.SDK.Models.Common;

namespace QueueManagement.SDK.Common;

/// <summary>
/// Base class for API clients with common HTTP functionality.
/// </summary>
public abstract class BaseApiClient
{
    protected readonly HttpClient HttpClient;
    protected readonly QueueManagementOptions Options;
    protected readonly ILogger? Logger;
    protected readonly JsonSerializerOptions JsonOptions;
    protected readonly IAsyncPolicy<HttpResponseMessage> RetryPolicy;

    /// <summary>
    /// Initializes a new instance of the <see cref="BaseApiClient"/> class.
    /// </summary>
    protected BaseApiClient(HttpClient httpClient, QueueManagementOptions options, ILogger? logger = null)
    {
        HttpClient = httpClient ?? throw new ArgumentNullException(nameof(httpClient));
        Options = options ?? throw new ArgumentNullException(nameof(options));
        Logger = logger;

        // Configure JSON serialization options
        JsonOptions = new JsonSerializerOptions
        {
            PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
            DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingNull,
            Converters =
            {
                new JsonStringEnumConverter(JsonNamingPolicy.CamelCase)
            }
        };

        // Configure retry policy
        RetryPolicy = CreateRetryPolicy();

        // Configure HTTP client
        ConfigureHttpClient();
    }

    /// <summary>
    /// Configures the HTTP client with default headers and settings.
    /// </summary>
    private void ConfigureHttpClient()
    {
        if (HttpClient.BaseAddress == null)
        {
            HttpClient.BaseAddress = new Uri(Options.BaseUrl);
        }

        HttpClient.Timeout = Options.Timeout;
        
        // Set default headers
        HttpClient.DefaultRequestHeaders.Clear();
        HttpClient.DefaultRequestHeaders.Authorization = 
            new AuthenticationHeaderValue("Bearer", Options.ApiKey);
        
        if (!string.IsNullOrEmpty(Options.UserAgent))
        {
            HttpClient.DefaultRequestHeaders.UserAgent.ParseAdd(Options.UserAgent);
        }
        else
        {
            HttpClient.DefaultRequestHeaders.UserAgent.ParseAdd("QueueManagement.SDK/1.0.0");
        }

        HttpClient.DefaultRequestHeaders.Accept.Add(
            new MediaTypeWithQualityHeaderValue("application/json"));

        if (!string.IsNullOrEmpty(Options.TenantId))
        {
            HttpClient.DefaultRequestHeaders.Add("X-Tenant-Id", Options.TenantId);
        }
    }

    /// <summary>
    /// Creates the retry policy for HTTP requests.
    /// </summary>
    private IAsyncPolicy<HttpResponseMessage> CreateRetryPolicy()
    {
        if (Options.MaxRetries <= 0)
        {
            return Policy.NoOpAsync<HttpResponseMessage>();
        }

        var jitterier = new Random();

        return HttpPolicyExtensions
            .HandleTransientHttpError()
            .OrResult(msg => msg.StatusCode == HttpStatusCode.TooManyRequests)
            .WaitAndRetryAsync(
                Options.MaxRetries,
                retryAttempt =>
                {
                    var delay = Options.RetryDelay * Math.Pow(2, retryAttempt - 1);
                    var maxDelay = Options.MaxRetryDelay;
                    var actualDelay = TimeSpan.FromMilliseconds(Math.Min(delay.TotalMilliseconds, maxDelay.TotalMilliseconds));
                    
                    // Add jitter
                    var jitter = TimeSpan.FromMilliseconds(jitterier.Next(0, (int)(actualDelay.TotalMilliseconds * 0.1)));
                    return actualDelay + jitter;
                },
                onRetry: (outcome, timespan, retryCount, context) =>
                {
                    Logger?.LogWarning(
                        "Retry {RetryCount} after {Delay}ms due to: {Reason}",
                        retryCount,
                        timespan.TotalMilliseconds,
                        outcome.Exception?.Message ?? outcome.Result?.StatusCode.ToString());
                });
    }

    /// <summary>
    /// Sends a GET request to the specified endpoint.
    /// </summary>
    protected async Task<T> GetAsync<T>(string endpoint, CancellationToken cancellationToken = default) where T : class
    {
        var request = new HttpRequestMessage(HttpMethod.Get, endpoint);
        return await SendRequestAsync<T>(request, cancellationToken).ConfigureAwait(false);
    }

    /// <summary>
    /// Sends a POST request to the specified endpoint.
    /// </summary>
    protected async Task<TResponse> PostAsync<TRequest, TResponse>(
        string endpoint, 
        TRequest? data, 
        CancellationToken cancellationToken = default) 
        where TRequest : class
        where TResponse : class
    {
        var request = new HttpRequestMessage(HttpMethod.Post, endpoint);
        
        if (data != null)
        {
            var json = JsonSerializer.Serialize(data, JsonOptions);
            request.Content = new StringContent(json, Encoding.UTF8, "application/json");
        }

        return await SendRequestAsync<TResponse>(request, cancellationToken).ConfigureAwait(false);
    }

    /// <summary>
    /// Sends a PUT request to the specified endpoint.
    /// </summary>
    protected async Task<TResponse> PutAsync<TRequest, TResponse>(
        string endpoint, 
        TRequest? data, 
        CancellationToken cancellationToken = default)
        where TRequest : class
        where TResponse : class
    {
        var request = new HttpRequestMessage(HttpMethod.Put, endpoint);
        
        if (data != null)
        {
            var json = JsonSerializer.Serialize(data, JsonOptions);
            request.Content = new StringContent(json, Encoding.UTF8, "application/json");
        }

        return await SendRequestAsync<TResponse>(request, cancellationToken).ConfigureAwait(false);
    }

    /// <summary>
    /// Sends a PATCH request to the specified endpoint.
    /// </summary>
    protected async Task<TResponse> PatchAsync<TRequest, TResponse>(
        string endpoint, 
        TRequest? data, 
        CancellationToken cancellationToken = default)
        where TRequest : class
        where TResponse : class
    {
        var request = new HttpRequestMessage(HttpMethod.Patch, endpoint);
        
        if (data != null)
        {
            var json = JsonSerializer.Serialize(data, JsonOptions);
            request.Content = new StringContent(json, Encoding.UTF8, "application/json");
        }

        return await SendRequestAsync<TResponse>(request, cancellationToken).ConfigureAwait(false);
    }

    /// <summary>
    /// Sends a DELETE request to the specified endpoint.
    /// </summary>
    protected async Task DeleteAsync(string endpoint, CancellationToken cancellationToken = default)
    {
        var request = new HttpRequestMessage(HttpMethod.Delete, endpoint);
        await SendRequestAsync(request, cancellationToken).ConfigureAwait(false);
    }

    /// <summary>
    /// Sends a DELETE request and returns a response.
    /// </summary>
    protected async Task<T> DeleteAsync<T>(string endpoint, CancellationToken cancellationToken = default) where T : class
    {
        var request = new HttpRequestMessage(HttpMethod.Delete, endpoint);
        return await SendRequestAsync<T>(request, cancellationToken).ConfigureAwait(false);
    }

    /// <summary>
    /// Sends an HTTP request and processes the response.
    /// </summary>
    private async Task<T> SendRequestAsync<T>(HttpRequestMessage request, CancellationToken cancellationToken) where T : class
    {
        var response = await SendRequestAsync(request, cancellationToken).ConfigureAwait(false);
        var content = await response.Content.ReadAsStringAsync(cancellationToken).ConfigureAwait(false);
        
        try
        {
            var result = JsonSerializer.Deserialize<T>(content, JsonOptions);
            if (result == null)
            {
                throw new QueueManagementException("Failed to deserialize response");
            }
            return result;
        }
        catch (JsonException ex)
        {
            Logger?.LogError(ex, "Failed to deserialize response: {Content}", content);
            throw new QueueManagementException($"Failed to deserialize response: {ex.Message}", ex);
        }
    }

    /// <summary>
    /// Sends an HTTP request without expecting a response body.
    /// </summary>
    private async Task<HttpResponseMessage> SendRequestAsync(HttpRequestMessage request, CancellationToken cancellationToken)
    {
        Logger?.LogDebug("Sending {Method} request to {Uri}", request.Method, request.RequestUri);

        try
        {
            var response = await RetryPolicy.ExecuteAsync(async () =>
                await HttpClient.SendAsync(request, cancellationToken).ConfigureAwait(false)
            ).ConfigureAwait(false);

            await EnsureSuccessStatusCodeAsync(response, request).ConfigureAwait(false);
            return response;
        }
        catch (HttpRequestException ex)
        {
            Logger?.LogError(ex, "HTTP request failed: {Method} {Uri}", request.Method, request.RequestUri);
            throw new QueueManagementException($"HTTP request failed: {ex.Message}", ex);
        }
        catch (TaskCanceledException ex)
        {
            Logger?.LogError(ex, "Request timeout: {Method} {Uri}", request.Method, request.RequestUri);
            throw new QueueManagementException("Request timeout", ex);
        }
    }

    /// <summary>
    /// Ensures the response has a success status code.
    /// </summary>
    private async Task EnsureSuccessStatusCodeAsync(HttpResponseMessage response, HttpRequestMessage request)
    {
        if (response.IsSuccessStatusCode)
        {
            return;
        }

        var content = await response.Content.ReadAsStringAsync().ConfigureAwait(false);
        var requestId = response.Headers.TryGetValues("X-Request-Id", out var values) 
            ? values.FirstOrDefault() 
            : null;

        Logger?.LogError(
            "API request failed: {Method} {Uri} - Status: {StatusCode}, Content: {Content}",
            request.Method,
            request.RequestUri,
            response.StatusCode,
            content);

        // Try to parse error response
        ApiResponse<object>? errorResponse = null;
        try
        {
            errorResponse = JsonSerializer.Deserialize<ApiResponse<object>>(content, JsonOptions);
        }
        catch
        {
            // Ignore deserialization errors
        }

        // Handle specific status codes
        switch (response.StatusCode)
        {
            case HttpStatusCode.Unauthorized:
                throw new QueueManagementAuthenticationException(
                    errorResponse?.Error ?? "Authentication failed",
                    requestId);

            case HttpStatusCode.Forbidden:
                throw new QueueManagementAuthorizationException(
                    errorResponse?.Error ?? "Authorization failed",
                    requestId);

            case HttpStatusCode.NotFound:
                throw new QueueManagementNotFoundException(
                    errorResponse?.Error ?? "Resource not found",
                    requestId: requestId);

            case HttpStatusCode.TooManyRequests:
                var retryAfter = response.Headers.RetryAfter?.Delta;
                throw new QueueManagementRateLimitException(
                    errorResponse?.Error ?? "Rate limit exceeded",
                    retryAfter,
                    requestId: requestId);

            case HttpStatusCode.BadRequest when errorResponse?.ValidationErrors?.Any() == true:
                throw new QueueManagementException(
                    errorResponse.Error ?? "Validation failed",
                    errorResponse.ValidationErrors,
                    requestId);

            default:
                throw new QueueManagementApiException(
                    errorResponse?.Error ?? $"API request failed with status {response.StatusCode}",
                    response.StatusCode,
                    content,
                    errorResponse?.ErrorCode,
                    requestId,
                    request.RequestUri?.ToString(),
                    request.Method.ToString());
        }
    }

    /// <summary>
    /// Builds a query string from parameters.
    /// </summary>
    protected static string BuildQueryString(Dictionary<string, string?> parameters)
    {
        var query = parameters
            .Where(p => !string.IsNullOrEmpty(p.Value))
            .Select(p => $"{Uri.EscapeDataString(p.Key)}={Uri.EscapeDataString(p.Value!)}")
            .ToList();

        return query.Any() ? "?" + string.Join("&", query) : string.Empty;
    }

    /// <summary>
    /// Streams results using IAsyncEnumerable.
    /// </summary>
    protected async IAsyncEnumerable<T> StreamAsync<T>(
        string endpoint,
        [System.Runtime.CompilerServices.EnumeratorCancellation] CancellationToken cancellationToken = default) 
        where T : class
    {
        var request = new HttpRequestMessage(HttpMethod.Get, endpoint);
        
        using var response = await HttpClient.SendAsync(request, HttpCompletionOption.ResponseHeadersRead, cancellationToken).ConfigureAwait(false);
        await EnsureSuccessStatusCodeAsync(response, request).ConfigureAwait(false);

        using var stream = await response.Content.ReadAsStreamAsync(cancellationToken).ConfigureAwait(false);
        using var reader = new StreamReader(stream);
        
        string? line;
        while ((line = await reader.ReadLineAsync(cancellationToken).ConfigureAwait(false)) != null)
        {
            if (string.IsNullOrWhiteSpace(line))
                continue;

            T? item = null;
            try
            {
                item = JsonSerializer.Deserialize<T>(line, JsonOptions);
            }
            catch (JsonException ex)
            {
                Logger?.LogWarning(ex, "Failed to deserialize streaming item: {Line}", line);
            }

            if (item != null)
            {
                yield return item;
            }
        }
    }
}