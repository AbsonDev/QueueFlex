using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Caching.StackExchangeRedis;
using QueueManagement.Infrastructure.Data.Interfaces;
using QueueManagement.Infrastructure.Data.Repositories;
using QueueManagement.Infrastructure.Data.UnitOfWork;
using QueueManagement.Infrastructure.Data.Caching;

namespace QueueManagement.Infrastructure.Extensions;

/// <summary>
/// Extension methods for registering repositories and related services
/// </summary>
public static class RepositoryExtensions
{
    /// <summary>
    /// Add repositories and related services to the service collection
    /// </summary>
    public static IServiceCollection AddRepositories(this IServiceCollection services, IConfiguration configuration)
    {
        // Register Unit of Work
        services.AddScoped<IUnitOfWork, UnitOfWork>();

        // Register Generic Repository
        services.AddScoped(typeof(IGenericRepository<>), typeof(GenericRepository<>));

        // Register Specific Repositories
        services.AddScoped<ITicketRepository, TicketRepository>();
        services.AddScoped<IQueueRepository, QueueRepository>();
        services.AddScoped<ISessionRepository, SessionRepository>();
        services.AddScoped<IUnitRepository, UnitRepository>();
        services.AddScoped<IServiceRepository, ServiceRepository>();
        services.AddScoped<IUserRepository, UserRepository>();
        services.AddScoped<ITenantRepository, TenantRepository>();
        services.AddScoped<IWebhookRepository, WebhookRepository>();

        // Register Cache Services
        services.AddMemoryCache();
        services.AddCacheServices(configuration);

        return services;
    }

    /// <summary>
    /// Add cache services to the service collection
    /// </summary>
    private static IServiceCollection AddCacheServices(this IServiceCollection services, IConfiguration configuration)
    {
        // Add memory cache
        services.AddMemoryCache();

        // Add distributed cache based on configuration
        var cacheProvider = configuration.GetValue<string>("Cache:Provider", "Memory");

        switch (cacheProvider.ToLowerInvariant())
        {
            case "redis":
                var redisConnectionString = configuration.GetConnectionString("Redis");
                if (!string.IsNullOrEmpty(redisConnectionString))
                {
                    services.AddStackExchangeRedisCache(options =>
                    {
                        options.Configuration = redisConnectionString;
                        options.InstanceName = configuration.GetValue<string>("Cache:Redis:InstanceName", "QueueManagement");
                    });
                }
                else
                {
                    // Fallback to memory cache if Redis connection string is not provided
                    services.AddDistributedMemoryCache();
                }
                break;

            case "sqlserver":
                // SQL Server distributed cache
                services.AddDistributedSqlServerCache(options =>
                {
                    options.ConnectionString = configuration.GetConnectionString("DefaultConnection");
                    options.SchemaName = configuration.GetValue<string>("Cache:SqlServer:SchemaName", "dbo");
                    options.TableName = configuration.GetValue<string>("Cache:SqlServer:TableName", "Cache");
                });
                break;

            default:
                // Default to memory cache
                services.AddDistributedMemoryCache();
                break;
        }

        // Register cache service
        services.AddScoped<ICacheService, CacheService>();

        return services;
    }
}