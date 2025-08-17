using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using Serilog;
using Serilog.Events;
using System.Text;
using QueueManagement.Api.Middleware;
using QueueManagement.Api.Services;
using QueueManagement.Api.Mappings;
using QueueManagement.Api.Validators.Common;
using FluentValidation.AspNetCore;
using QueueManagement.Infrastructure.Data;
using Microsoft.AspNetCore.RateLimiting;

var builder = WebApplication.CreateBuilder(args);

// Configure Serilog
Log.Logger = new LoggerConfiguration()
    .MinimumLevel.Debug()
    .MinimumLevel.Override("Microsoft", LogEventLevel.Information)
    .MinimumLevel.Override("Microsoft.AspNetCore", LogEventLevel.Warning)
    .Enrich.FromLogContext()
    .WriteTo.Console()
    .WriteTo.File("logs/queue-management-api-.txt", rollingInterval: RollingInterval.Day)
    .CreateLogger();

builder.Host.UseSerilog();

try
{
    Log.Information("Starting Queue Management API");

    // Add services to the container
    builder.Services.AddControllers(options =>
    {
        options.Filters.Add<FluentValidationActionFilter>();
    })
    .AddFluentValidation(fv =>
    {
        fv.RegisterValidatorsFromAssemblyContaining<PaginationRequestDtoValidator>();
        fv.DisableDataAnnotationsValidation = true;
    });

    // Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
    builder.Services.AddEndpointsApiExplorer();
    builder.Services.AddSwaggerGen(c =>
    {
        c.SwaggerDoc("v1", new OpenApiInfo
        {
            Title = "Queue Management API",
            Version = "v1",
            Description = "A comprehensive API for managing queue systems with multi-tenant support",
            Contact = new OpenApiContact
            {
                Name = "Queue Management Team",
                Email = "support@queuemanagement.com"
            },
            License = new OpenApiLicense
            {
                Name = "MIT",
                Url = new Uri("https://opensource.org/licenses/MIT")
            }
        });

        // Add JWT Bearer authentication
        c.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
        {
            Description = "JWT Authorization header using the Bearer scheme. Example: \"Authorization: Bearer {token}\"",
            Name = "Authorization",
            In = ParameterLocation.Header,
            Type = SecuritySchemeType.ApiKey,
            Scheme = "Bearer"
        });

        c.AddSecurityRequirement(new OpenApiSecurityRequirement
        {
            {
                new OpenApiSecurityScheme
                {
                    Reference = new OpenApiReference
                    {
                        Type = ReferenceType.SecurityScheme,
                        Id = "Bearer"
                    }
                },
                Array.Empty<string>()
            }
        });

        // Add operation filters for better documentation
        c.EnableAnnotations();
        c.IncludeXmlComments(Path.Combine(AppContext.BaseDirectory, $"{typeof(Program).Assembly.GetName().Name}.xml"));
    });

    // Add Entity Framework Core
    builder.Services.AddDbContext<QueueManagementDbContext>(options =>
        options.UseNpgsql(builder.Configuration.GetConnectionString("DefaultConnection"),
            b => b.MigrationsAssembly("QueueManagement.Infrastructure")));

    // Add MediatR
    builder.Services.AddMediatR(cfg =>
    {
        cfg.RegisterServicesFromAssembly(typeof(Program).Assembly);
        cfg.RegisterServicesFromAssembly(typeof(QueueManagement.Domain.AssemblyReference).Assembly);
    });

    // Add AutoMapper
    builder.Services.AddAutoMapper(typeof(Program).Assembly);
    builder.Services.AddAutoMapper(typeof(CommonMappingProfile).Assembly);
    builder.Services.AddAutoMapper(typeof(UnitMappingProfile).Assembly);
    builder.Services.AddAutoMapper(typeof(QueueMappingProfile).Assembly);
    builder.Services.AddAutoMapper(typeof(TicketMappingProfile).Assembly);
    builder.Services.AddAutoMapper(typeof(SessionMappingProfile).Assembly);
    builder.Services.AddAutoMapper(typeof(ServiceMappingProfile).Assembly);
    builder.Services.AddAutoMapper(typeof(UserMappingProfile).Assembly);
    builder.Services.AddAutoMapper(typeof(WebhookMappingProfile).Assembly);
    builder.Services.AddAutoMapper(typeof(DashboardMappingProfile).Assembly);
    builder.Services.AddAutoMapper(typeof(AuthMappingProfile).Assembly);

    // Add JWT Authentication
    var jwtSettings = builder.Configuration.GetSection("Jwt");
    var secretKey = jwtSettings["SecretKey"] ?? throw new InvalidOperationException("JWT SecretKey not configured");
    var issuer = jwtSettings["Issuer"] ?? "QueueManagement";
    var audience = jwtSettings["Audience"] ?? "QueueManagement";

    builder.Services.AddAuthentication(options =>
    {
        options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
        options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
        options.DefaultScheme = JwtBearerDefaults.AuthenticationScheme;
    })
    .AddJwtBearer(options =>
    {
        options.SaveToken = true;
        options.RequireHttpsMetadata = false;
        options.TokenValidationParameters = new TokenValidationParameters
        {
            ValidateIssuer = true,
            ValidateAudience = true,
            ValidateLifetime = true,
            ValidateIssuerSigningKey = true,
            ValidIssuer = issuer,
            ValidAudience = audience,
            IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(secretKey)),
            ClockSkew = TimeSpan.Zero
        };

        // Configure SignalR JWT authentication
        options.Events = new JwtBearerEvents
        {
            OnMessageReceived = context =>
            {
                var accessToken = context.Request.Query["access_token"];
                var path = context.HttpContext.Request.Path;
                if (!string.IsNullOrEmpty(accessToken) && path.StartsWithSegments("/hubs"))
                {
                    context.Token = accessToken;
                }
                return Task.CompletedTask;
            }
        };
    });

    // Add Authorization
    builder.Services.AddAuthorization(options =>
    {
        options.AddPolicy("AdminOnly", policy => policy.RequireRole("Admin"));
        options.AddPolicy("ManagerOrAdmin", policy => policy.RequireRole("Admin", "Manager"));
        options.AddPolicy("AgentOrAbove", policy => policy.RequireRole("Admin", "Manager", "Agent"));
    });

    // Add SignalR
    builder.Services.AddSignalR();

    // Add CORS
    builder.Services.AddCors(options =>
    {
        options.AddPolicy("AllowSpecificOrigin", policy =>
        {
            policy.WithOrigins(
                    "http://localhost:3000", // React dev server
                    "http://localhost:4200", // Angular dev server
                    "https://localhost:3000",
                    "https://localhost:4200"
                )
                .AllowAnyMethod()
                .AllowAnyHeader()
                .AllowCredentials();
        });
    });

    // Add Health Checks
    builder.Services.AddHealthChecks()
        .AddDbContextCheck<QueueManagementDbContext>("Database")
        .AddCheck("Self", () => Microsoft.Extensions.Diagnostics.HealthChecks.HealthCheckResult.Healthy());

    // Add Response Compression
    builder.Services.AddResponseCompression(options =>
    {
        options.EnableForHttps = true;
        options.Providers.Add<Microsoft.AspNetCore.ResponseCompression.BrotliCompressionProvider>();
        options.Providers.Add<Microsoft.AspNetCore.ResponseCompression.GzipCompressionProvider>();
    });

    // Add Memory Cache
    builder.Services.AddMemoryCache();

    // Add Distributed Cache (Redis if configured, otherwise Memory)
    if (!string.IsNullOrEmpty(builder.Configuration.GetConnectionString("Redis")))
    {
        builder.Services.AddStackExchangeRedisCache(options =>
        {
            options.Configuration = builder.Configuration.GetConnectionString("Redis");
            options.InstanceName = "QueueManagement_";
        });
    }
    else
    {
        builder.Services.AddDistributedMemoryCache();
    }

    // Add Rate Limiting
    builder.Services.AddRateLimiter(options =>
    {
        options.GlobalLimiter = PartitionedRateLimiter.Create<HttpContext, string>(context =>
            RateLimitPartition.GetFixedWindowLimiter(
                partitionKey: context.User.Identity?.Name ?? context.Request.Headers.Host.ToString(),
                factory: partition => new FixedWindowRateLimiterOptions
                {
                    AutoReplenishment = true,
                    PermitLimit = 100,
                    Window = TimeSpan.FromMinutes(1)
                }));
    });

    // Register Services
    builder.Services.AddScoped<IJwtService, JwtService>();

    // Add HTTP Context Accessor
    builder.Services.AddHttpContextAccessor();

    // Add Problem Details
    builder.Services.AddProblemDetails();

    var app = builder.Build();

    // Configure the HTTP request pipeline
    if (app.Environment.IsDevelopment())
    {
        app.UseSwagger();
        app.UseSwaggerUI(c =>
        {
            c.SwaggerEndpoint("/swagger/v1/swagger.json", "Queue Management API v1");
            c.RoutePrefix = string.Empty; // Serve Swagger UI at root
            c.DocumentTitle = "Queue Management API Documentation";
            c.DefaultModelsExpandDepth(-1); // Hide schemas section
        });

        // Add development-specific middleware
        app.UseDeveloperExceptionPage();
    }
    else
    {
        // Production error handling
        app.UseExceptionHandler("/Error");
        app.UseHsts();
    }

    // Configure middleware pipeline
    app.UseHttpsRedirection();

    // Add custom middleware
    app.UseTenantResolution();
    app.UseGlobalExceptionHandling();
    app.UseRequestLogging();

    // Add standard middleware
    app.UseResponseCompression();
    app.UseCors("AllowSpecificOrigin");
    app.UseRateLimiter();

    // Add authentication and authorization
    app.UseAuthentication();
    app.UseAuthorization();

    // Map controllers
    app.MapControllers();

    // Map SignalR hubs
    app.MapHub<QueueManagement.Api.Hubs.QueueHub>("/hubs/queue");
    app.MapHub<QueueManagement.Api.Hubs.DashboardHub>("/hubs/dashboard");

    // Map health checks
    app.MapHealthChecks("/health", new Microsoft.AspNetCore.Diagnostics.HealthChecks.HealthCheckOptions
    {
        ResponseWriter = async (context, report) =>
        {
            context.Response.ContentType = "application/json";
            var result = System.Text.Json.JsonSerializer.Serialize(new
            {
                status = report.Status.ToString(),
                checks = report.Entries.Select(e => new
                {
                    name = e.Key,
                    status = e.Value.Status.ToString(),
                    description = e.Value.Description,
                    duration = e.Value.Duration.ToString()
                })
            });
            await context.Response.WriteAsync(result);
        }
    });

    // Map root endpoint
    app.MapGet("/", () => Results.Redirect("/swagger"));

    // Ensure database is created and migrations are applied
    using (var scope = app.Services.CreateScope())
    {
        var context = scope.ServiceProvider.GetRequiredService<QueueManagementDbContext>();
        try
        {
            context.Database.Migrate();
            Log.Information("Database migrations applied successfully");
        }
        catch (Exception ex)
        {
            Log.Warning(ex, "Database migration failed. This might be expected in some environments.");
        }
    }

    Log.Information("Queue Management API started successfully");
    app.Run();
}
catch (Exception ex)
{
    Log.Fatal(ex, "Queue Management API failed to start");
}
finally
{
    Log.CloseAndFlush();
}