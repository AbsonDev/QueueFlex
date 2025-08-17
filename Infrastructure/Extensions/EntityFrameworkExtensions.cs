using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Configuration;
using QueueManagement.Infrastructure.Data;

namespace QueueManagement.Infrastructure.Extensions;

/// <summary>
/// Extension methods for Entity Framework Core configuration
/// </summary>
public static class EntityFrameworkExtensions
{
    /// <summary>
    /// Adds the Queue Management database context to the service collection
    /// </summary>
    public static IServiceCollection AddQueueManagementDbContext(
        this IServiceCollection services,
        IConfiguration configuration)
    {
        var connectionString = configuration.GetConnectionString("DefaultConnection");

        if (string.IsNullOrEmpty(connectionString))
        {
            throw new InvalidOperationException("Connection string 'DefaultConnection' not found.");
        }

        services.AddDbContext<QueueManagementDbContext>(options =>
        {
            options.UseNpgsql(connectionString, npgsqlOptions =>
            {
                npgsqlOptions.MigrationsAssembly(typeof(QueueManagementDbContext).Assembly.FullName);
                
                // Enable retry on failure
                npgsqlOptions.EnableRetryOnFailure(
                    maxRetryCount: 3,
                    maxRetryDelay: TimeSpan.FromSeconds(30),
                    errorCodesToAdd: null);
                
                // Enable sensitive data logging in development
                if (configuration.GetValue<bool>("EnableSensitiveDataLogging"))
                {
                    npgsqlOptions.EnableSensitiveDataLogging();
                }
            });

            // Enable detailed errors in development
            if (configuration.GetValue<bool>("EnableDetailedErrors"))
            {
                options.EnableDetailedErrors();
            }

            // Enable sensitive data logging in development
            if (configuration.GetValue<bool>("EnableSensitiveDataLogging"))
            {
                options.EnableSensitiveDataLogging();
            }
        });

        return services;
    }

    /// <summary>
    /// Configures the database context with PostgreSQL-specific optimizations
    /// </summary>
    public static DbContextOptionsBuilder ConfigurePostgreSQL(
        this DbContextOptionsBuilder optionsBuilder,
        string connectionString)
    {
        return optionsBuilder.UseNpgsql(connectionString, npgsqlOptions =>
        {
            // Configure PostgreSQL-specific options
            npgsqlOptions.UseQuerySplittingBehavior(QuerySplittingBehavior.SplitQuery);
            
            // Enable retry on failure
            npgsqlOptions.EnableRetryOnFailure(
                maxRetryCount: 3,
                maxRetryDelay: TimeSpan.FromSeconds(30),
                errorCodesToAdd: null);
        });
    }

    /// <summary>
    /// Applies database migrations
    /// </summary>
    public static async Task ApplyMigrationsAsync(this IServiceProvider serviceProvider)
    {
        using var scope = serviceProvider.CreateScope();
        var context = scope.ServiceProvider.GetRequiredService<QueueManagementDbContext>();

        if (context.Database.GetPendingMigrations().Any())
        {
            await context.Database.MigrateAsync();
        }
    }

    /// <summary>
    /// Seeds the database with initial data
    /// </summary>
    public static async Task SeedDatabaseAsync(this IServiceProvider serviceProvider)
    {
        using var scope = serviceProvider.CreateScope();
        var context = scope.ServiceProvider.GetRequiredService<QueueManagementDbContext>();

        // Check if data already exists
        if (await context.Tenants.AnyAsync())
        {
            return; // Database already seeded
        }

        // Seed initial data
        await SeedInitialDataAsync(context);
    }

    /// <summary>
    /// Seeds initial data for development
    /// </summary>
    private static async Task SeedInitialDataAsync(QueueManagementDbContext context)
    {
        // Create a demo tenant
        var demoTenant = new Domain.Entities.Tenant(
            "Demo Company",
            "demo",
            "America/Sao_Paulo",
            "system");

        context.Tenants.Add(demoTenant);
        await context.SaveChangesAsync();

        // Create a demo unit
        var demoAddress = new Domain.ValueObjects.Address(
            "Rua das Flores",
            "123",
            "Centro",
            "São Paulo",
            "SP",
            "01234-567",
            "Brasil");

        var demoUnit = new Domain.Entities.Unit(
            "Unidade Central",
            "UC001",
            demoAddress,
            demoTenant.Id,
            "system");

        context.Units.Add(demoUnit);
        await context.SaveChangesAsync();

        // Create demo services
        var atendimentoService = new Domain.Entities.Service(
            "Atendimento Geral",
            "AG001",
            30,
            demoTenant.Id,
            "system");

        var consultaService = new Domain.Entities.Service(
            "Consulta Especializada",
            "CE001",
            60,
            demoTenant.Id,
            "system");

        context.Services.AddRange(atendimentoService, consultaService);
        await context.SaveChangesAsync();

        // Create demo queue
        var demoQueue = new Domain.Entities.Queue(
            "Fila Principal",
            "FP001",
            "Fila Principal - Atendimento",
            demoUnit.Id,
            demoTenant.Id,
            "system");

        context.Queues.Add(demoQueue);
        await context.SaveChangesAsync();

        // Create demo users
        var adminUser = new Domain.Entities.User(
            "Administrador",
            "admin@demo.com",
            "EMP001",
            Domain.Enums.UserRole.Admin,
            demoTenant.Id,
            "system");

        var attendantUser = new Domain.Entities.User(
            "Atendente",
            "atendente@demo.com",
            "EMP002",
            Domain.Enums.UserRole.Attendant,
            demoTenant.Id,
            "system");

        context.Users.AddRange(adminUser, attendantUser);
        await context.SaveChangesAsync();

        // Create demo resources
        var counter1 = new Domain.Entities.Resource(
            "Guichê 1",
            "GC001",
            Domain.Enums.ResourceType.Counter,
            demoUnit.Id,
            demoTenant.Id,
            "system",
            "Sala Principal");

        var counter2 = new Domain.Entities.Resource(
            "Guichê 2",
            "GC002",
            Domain.Enums.ResourceType.Counter,
            demoUnit.Id,
            demoTenant.Id,
            "system",
            "Sala Principal");

        context.Resources.AddRange(counter1, counter2);
        await context.SaveChangesAsync();

        // Create operating hours
        var mondayHours = new Domain.Entities.UnitOperatingHour(
            demoUnit.Id,
            DayOfWeek.Monday,
            TimeSpan.FromHours(8),
            TimeSpan.FromHours(18),
            TimeSpan.FromHours(12),
            TimeSpan.FromHours(13),
            "system");

        var tuesdayHours = new Domain.Entities.UnitOperatingHour(
            demoUnit.Id,
            DayOfWeek.Tuesday,
            TimeSpan.FromHours(8),
            TimeSpan.FromHours(18),
            TimeSpan.FromHours(12),
            TimeSpan.FromHours(13),
            "system");

        var wednesdayHours = new Domain.Entities.UnitOperatingHour(
            demoUnit.Id,
            DayOfWeek.Wednesday,
            TimeSpan.FromHours(8),
            TimeSpan.FromHours(18),
            TimeSpan.FromHours(12),
            TimeSpan.FromHours(13),
            "system");

        var thursdayHours = new Domain.Entities.UnitOperatingHour(
            demoUnit.Id,
            DayOfWeek.Thursday,
            TimeSpan.FromHours(8),
            TimeSpan.FromHours(18),
            TimeSpan.FromHours(12),
            TimeSpan.FromHours(13),
            "system");

        var fridayHours = new Domain.Entities.UnitOperatingHour(
            demoUnit.Id,
            DayOfWeek.Friday,
            TimeSpan.FromHours(8),
            TimeSpan.FromHours(18),
            TimeSpan.FromHours(12),
            TimeSpan.FromHours(13),
            "system");

        context.UnitOperatingHours.AddRange(
            mondayHours, tuesdayHours, wednesdayHours, thursdayHours, fridayHours);
        await context.SaveChangesAsync();

        // Create queue-service relationships
        var queueService1 = new Domain.Entities.JunctionTables.QueueService(
            demoQueue.Id,
            atendimentoService.Id,
            demoTenant.Id,
            "system",
            1);

        var queueService2 = new Domain.Entities.JunctionTables.QueueService(
            demoQueue.Id,
            consultaService.Id,
            demoTenant.Id,
            "system",
            2);

        context.QueueServices.AddRange(queueService1, queueService2);
        await context.SaveChangesAsync();

        // Create unit-user assignments
        var unitUser1 = new Domain.Entities.JunctionTables.UnitUser(
            demoUnit.Id,
            adminUser.Id,
            demoTenant.Id,
            "system");

        var unitUser2 = new Domain.Entities.JunctionTables.UnitUser(
            demoUnit.Id,
            attendantUser.Id,
            demoTenant.Id,
            "system");

        context.UnitUsers.AddRange(unitUser1, unitUser2);
        await context.SaveChangesAsync();

        Console.WriteLine("Database seeded successfully with demo data!");
    }
}