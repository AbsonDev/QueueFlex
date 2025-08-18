using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Polly;
using Polly.Extensions.Http;
using QueueManagement.SDK.Configuration;
using QueueManagement.SDK.Services;
using QueueManagement.SDK.Services.Implementation;
using QueueManagement.SDK.SignalR;

namespace QueueManagement.SDK.Extensions;

/// <summary>
/// Extension methods for configuring QueueManagement SDK services.
/// </summary>
public static class ServiceCollectionExtensions
{
    /// <summary>
    /// Adds the QueueManagement client to the service collection.
    /// </summary>
    /// <param name="services">The service collection.</param>
    /// <param name="configure">The configuration action.</param>
    /// <returns>The service collection for chaining.</returns>
    public static IServiceCollection AddQueueManagementClient(
        this IServiceCollection services,
        Action<QueueManagementOptions> configure)
    {
        if (services == null)
        {
            throw new ArgumentNullException(nameof(services));
        }

        if (configure == null)
        {
            throw new ArgumentNullException(nameof(configure));
        }

        // Configure options
        services.Configure(configure);

        // Add core services
        AddCoreServices(services);

        return services;
    }

    /// <summary>
    /// Adds the QueueManagement client to the service collection using configuration.
    /// </summary>
    /// <param name="services">The service collection.</param>
    /// <param name="configuration">The configuration.</param>
    /// <param name="sectionName">The configuration section name (default: "QueueManagement").</param>
    /// <returns>The service collection for chaining.</returns>
    public static IServiceCollection AddQueueManagementClient(
        this IServiceCollection services,
        IConfiguration configuration,
        string sectionName = "QueueManagement")
    {
        if (services == null)
        {
            throw new ArgumentNullException(nameof(services));
        }

        if (configuration == null)
        {
            throw new ArgumentNullException(nameof(configuration));
        }

        // Bind configuration
        services.Configure<QueueManagementOptions>(configuration.GetSection(sectionName));

        // Add core services
        AddCoreServices(services);

        return services;
    }

    /// <summary>
    /// Adds the QueueManagement client to the service collection with an API key.
    /// </summary>
    /// <param name="services">The service collection.</param>
    /// <param name="apiKey">The API key.</param>
    /// <param name="baseUrl">The base URL (optional).</param>
    /// <returns>The service collection for chaining.</returns>
    public static IServiceCollection AddQueueManagementClient(
        this IServiceCollection services,
        string apiKey,
        string? baseUrl = null)
    {
        if (services == null)
        {
            throw new ArgumentNullException(nameof(services));
        }

        if (string.IsNullOrWhiteSpace(apiKey))
        {
            throw new ArgumentException("API key must not be empty.", nameof(apiKey));
        }

        return services.AddQueueManagementClient(options =>
        {
            options.ApiKey = apiKey;
            if (!string.IsNullOrWhiteSpace(baseUrl))
            {
                options.BaseUrl = baseUrl;
            }
        });
    }

    /// <summary>
    /// Adds core QueueManagement services.
    /// </summary>
    private static void AddCoreServices(IServiceCollection services)
    {
        // Add HTTP client with retry policy
        services.AddHttpClient<QueueManagementClient>((serviceProvider, client) =>
        {
            var options = serviceProvider.GetRequiredService<IOptions<QueueManagementOptions>>().Value;
            
            // Validate options
            options.Validate();

            // Configure HTTP client
            client.BaseAddress = new Uri(options.BaseUrl);
            client.Timeout = options.Timeout;
            client.DefaultRequestHeaders.Add("Authorization", $"Bearer {options.ApiKey}");
            
            if (!string.IsNullOrEmpty(options.UserAgent))
            {
                client.DefaultRequestHeaders.UserAgent.ParseAdd(options.UserAgent);
            }
            else
            {
                client.DefaultRequestHeaders.UserAgent.ParseAdd("QueueManagement.SDK/1.0.0 (.NET)");
            }

            if (!string.IsNullOrEmpty(options.TenantId))
            {
                client.DefaultRequestHeaders.Add("X-Tenant-Id", options.TenantId);
            }
        })
        .AddPolicyHandler((serviceProvider, request) =>
        {
            var options = serviceProvider.GetRequiredService<IOptions<QueueManagementOptions>>().Value;
            
            if (options.MaxRetries <= 0)
            {
                return Policy.NoOpAsync<HttpResponseMessage>();
            }

            return HttpPolicyExtensions
                .HandleTransientHttpError()
                .OrResult(msg => msg.StatusCode == System.Net.HttpStatusCode.TooManyRequests)
                .WaitAndRetryAsync(
                    options.MaxRetries,
                    retryAttempt =>
                    {
                        var delay = options.RetryDelay * Math.Pow(2, retryAttempt - 1);
                        var maxDelay = options.MaxRetryDelay;
                        return TimeSpan.FromMilliseconds(Math.Min(delay.TotalMilliseconds, maxDelay.TotalMilliseconds));
                    },
                    onRetry: (outcome, timespan, retryCount, context) =>
                    {
                        var logger = serviceProvider.GetService<ILogger<QueueManagementClient>>();
                        logger?.LogWarning(
                            "Retry {RetryCount} after {Delay}ms due to: {Reason}",
                            retryCount,
                            timespan.TotalMilliseconds,
                            outcome.Exception?.Message ?? outcome.Result?.StatusCode.ToString());
                    });
        });

        // Register main client
        services.AddScoped<QueueManagementClient>(serviceProvider =>
        {
            var httpClientFactory = serviceProvider.GetRequiredService<IHttpClientFactory>();
            var httpClient = httpClientFactory.CreateClient(nameof(QueueManagementClient));
            var options = serviceProvider.GetRequiredService<IOptions<QueueManagementOptions>>().Value;
            var loggerFactory = serviceProvider.GetService<ILoggerFactory>();

            return new QueueManagementClient(httpClient, options, loggerFactory, false);
        });

        // Register individual service clients
        services.AddScoped<ITicketsClient>(serviceProvider => serviceProvider.GetRequiredService<QueueManagementClient>().Tickets);
        services.AddScoped<IQueuesClient>(serviceProvider => serviceProvider.GetRequiredService<QueueManagementClient>().Queues);
        services.AddScoped<ISessionsClient>(serviceProvider => serviceProvider.GetRequiredService<QueueManagementClient>().Sessions);
        services.AddScoped<IUnitsClient>(serviceProvider => serviceProvider.GetRequiredService<QueueManagementClient>().Units);
        services.AddScoped<IServicesClient>(serviceProvider => serviceProvider.GetRequiredService<QueueManagementClient>().Services);
        services.AddScoped<IUsersClient>(serviceProvider => serviceProvider.GetRequiredService<QueueManagementClient>().Users);
        services.AddScoped<IDashboardClient>(serviceProvider => serviceProvider.GetRequiredService<QueueManagementClient>().Dashboard);
        services.AddScoped<IWebhooksClient>(serviceProvider => serviceProvider.GetRequiredService<QueueManagementClient>().Webhooks);

        // Register SignalR client
        services.AddScoped<IQueueSignalRClient>(serviceProvider => serviceProvider.GetRequiredService<QueueManagementClient>().SignalR);
    }

    /// <summary>
    /// Adds QueueManagement SignalR client only.
    /// </summary>
    /// <param name="services">The service collection.</param>
    /// <param name="configure">The configuration action.</param>
    /// <returns>The service collection for chaining.</returns>
    public static IServiceCollection AddQueueManagementSignalR(
        this IServiceCollection services,
        Action<QueueManagementOptions> configure)
    {
        if (services == null)
        {
            throw new ArgumentNullException(nameof(services));
        }

        if (configure == null)
        {
            throw new ArgumentNullException(nameof(configure));
        }

        services.Configure(configure);

        services.AddSingleton<IQueueSignalRClient>(serviceProvider =>
        {
            var options = serviceProvider.GetRequiredService<IOptions<QueueManagementOptions>>().Value;
            var logger = serviceProvider.GetService<ILogger<QueueSignalRClient>>();
            
            var client = new QueueSignalRClient(options, logger);
            
            if (options.AutoConnectSignalR)
            {
                _ = client.ConnectAsync().ConfigureAwait(false);
            }

            return client;
        });

        return services;
    }

    /// <summary>
    /// Configures health checks for QueueManagement API.
    /// </summary>
    /// <param name="builder">The health checks builder.</param>
    /// <param name="name">The health check name.</param>
    /// <param name="tags">The health check tags.</param>
    /// <returns>The health checks builder for chaining.</returns>
    public static IHealthChecksBuilder AddQueueManagementHealthCheck(
        this IHealthChecksBuilder builder,
        string name = "queuemanagement",
        params string[] tags)
    {
        if (builder == null)
        {
            throw new ArgumentNullException(nameof(builder));
        }

        return builder.AddTypeActivatedCheck<QueueManagementHealthCheck>(
            name,
            tags: tags ?? Array.Empty<string>());
    }
}

/// <summary>
/// Health check for QueueManagement API.
/// </summary>
internal class QueueManagementHealthCheck : Microsoft.Extensions.Diagnostics.HealthChecks.IHealthCheck
{
    private readonly QueueManagementClient _client;

    public QueueManagementHealthCheck(QueueManagementClient client)
    {
        _client = client ?? throw new ArgumentNullException(nameof(client));
    }

    public async Task<Microsoft.Extensions.Diagnostics.HealthChecks.HealthCheckResult> CheckHealthAsync(
        Microsoft.Extensions.Diagnostics.HealthChecks.HealthCheckContext context,
        CancellationToken cancellationToken = default)
    {
        try
        {
            var isHealthy = await _client.HealthCheckAsync(cancellationToken).ConfigureAwait(false);
            
            return isHealthy
                ? Microsoft.Extensions.Diagnostics.HealthChecks.HealthCheckResult.Healthy("QueueManagement API is healthy")
                : Microsoft.Extensions.Diagnostics.HealthChecks.HealthCheckResult.Unhealthy("QueueManagement API is unhealthy");
        }
        catch (Exception ex)
        {
            return Microsoft.Extensions.Diagnostics.HealthChecks.HealthCheckResult.Unhealthy(
                "Failed to check QueueManagement API health",
                ex);
        }
    }
}