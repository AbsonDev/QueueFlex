using QueueManagement.SDK.Extensions;
using QueueManagement.SDK;
using QueueManagement.SDK.Models.Tickets;
using QueueManagement.SDK.Models.Common;
using Microsoft.AspNetCore.Mvc;
using QueueManagement.SDK.Webhooks;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(options =>
{
    options.SwaggerDoc("v1", new() 
    { 
        Title = "QueueManagement SDK Web Example", 
        Version = "v1",
        Description = "Example API demonstrating QueueManagement SDK integration"
    });
});

// Add QueueManagement SDK
builder.Services.AddQueueManagementClient(options =>
{
    options.ApiKey = builder.Configuration["QueueManagement:ApiKey"] 
        ?? Environment.GetEnvironmentVariable("QUEUEMANAGEMENT_API_KEY") 
        ?? "your-api-key-here";
    
    options.BaseUrl = builder.Configuration["QueueManagement:BaseUrl"] 
        ?? Environment.GetEnvironmentVariable("QUEUEMANAGEMENT_BASE_URL")
        ?? "https://api.queuemanagement.io";
    
    options.EnableLogging = true;
    options.LogLevel = LogLevel.Information;
    options.AutoConnectSignalR = true;
});

// Add health checks
builder.Services.AddHealthChecks()
    .AddQueueManagementHealthCheck("queuemanagement-api", tags: new[] { "api", "external" });

// Add CORS for demo purposes
builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowAll", policy =>
    {
        policy.AllowAnyOrigin()
              .AllowAnyMethod()
              .AllowAnyHeader();
    });
});

var app = builder.Build();

// Configure the HTTP request pipeline
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();
app.UseCors("AllowAll");

// Health check endpoint
app.MapHealthChecks("/health");

// API Info endpoint
app.MapGet("/api/info", async (QueueManagementClient client) =>
{
    var info = await client.GetApiInfoAsync();
    return Results.Ok(info);
})
.WithName("GetApiInfo")
.WithOpenApi();

// Units endpoints
app.MapGet("/api/units", async (QueueManagementClient client) =>
{
    var units = await client.Units.GetAllAsync();
    return Results.Ok(units);
})
.WithName("GetUnits")
.WithOpenApi();

app.MapGet("/api/units/{unitId}", async (Guid unitId, QueueManagementClient client) =>
{
    try
    {
        var unit = await client.Units.GetAsync(unitId);
        return Results.Ok(unit);
    }
    catch (Exception)
    {
        return Results.NotFound();
    }
})
.WithName("GetUnit")
.WithOpenApi();

// Queues endpoints
app.MapGet("/api/queues", async (QueueManagementClient client, [FromQuery] Guid? unitId) =>
{
    var queues = unitId.HasValue 
        ? await client.Queues.GetByUnitAsync(unitId.Value)
        : await client.Queues.GetAllAsync();
    return Results.Ok(queues);
})
.WithName("GetQueues")
.WithOpenApi();

app.MapGet("/api/queues/{queueId}", async (Guid queueId, QueueManagementClient client) =>
{
    try
    {
        var queue = await client.Queues.GetAsync(queueId);
        return Results.Ok(queue);
    }
    catch (Exception)
    {
        return Results.NotFound();
    }
})
.WithName("GetQueue")
.WithOpenApi();

app.MapGet("/api/queues/{queueId}/status", async (Guid queueId, QueueManagementClient client) =>
{
    try
    {
        var status = await client.Queues.GetStatusAsync(queueId);
        return Results.Ok(status);
    }
    catch (Exception)
    {
        return Results.NotFound();
    }
})
.WithName("GetQueueStatus")
.WithOpenApi();

app.MapGet("/api/queues/{queueId}/metrics", async (Guid queueId, QueueManagementClient client) =>
{
    try
    {
        var metrics = await client.Queues.GetMetricsAsync(queueId);
        return Results.Ok(metrics);
    }
    catch (Exception)
    {
        return Results.NotFound();
    }
})
.WithName("GetQueueMetrics")
.WithOpenApi();

// Tickets endpoints
app.MapPost("/api/tickets", async (CreateTicketRequest request, QueueManagementClient client) =>
{
    try
    {
        var ticket = await client.Tickets.CreateAsync(request);
        return Results.Created($"/api/tickets/{ticket.Id}", ticket);
    }
    catch (Exception ex)
    {
        return Results.BadRequest(new { error = ex.Message });
    }
})
.WithName("CreateTicket")
.WithOpenApi();

app.MapGet("/api/tickets/{ticketId}", async (Guid ticketId, QueueManagementClient client) =>
{
    try
    {
        var ticket = await client.Tickets.GetAsync(ticketId);
        return Results.Ok(ticket);
    }
    catch (Exception)
    {
        return Results.NotFound();
    }
})
.WithName("GetTicket")
.WithOpenApi();

app.MapGet("/api/tickets/{ticketId}/status", async (Guid ticketId, QueueManagementClient client) =>
{
    try
    {
        var status = await client.Tickets.GetStatusAsync(ticketId);
        return Results.Ok(status);
    }
    catch (Exception)
    {
        return Results.NotFound();
    }
})
.WithName("GetTicketStatus")
.WithOpenApi();

app.MapGet("/api/tickets/{ticketId}/position", async (Guid ticketId, QueueManagementClient client) =>
{
    try
    {
        var position = await client.Tickets.GetPositionAsync(ticketId);
        return Results.Ok(position);
    }
    catch (Exception)
    {
        return Results.NotFound();
    }
})
.WithName("GetTicketPosition")
.WithOpenApi();

app.MapPost("/api/tickets/{ticketId}/call", async (Guid ticketId, QueueManagementClient client) =>
{
    try
    {
        var ticket = await client.Tickets.CallAsync(ticketId);
        return Results.Ok(ticket);
    }
    catch (Exception ex)
    {
        return Results.BadRequest(new { error = ex.Message });
    }
})
.WithName("CallTicket")
.WithOpenApi();

app.MapPost("/api/tickets/{ticketId}/complete", async (Guid ticketId, QueueManagementClient client) =>
{
    try
    {
        var ticket = await client.Tickets.CompleteAsync(ticketId);
        return Results.Ok(ticket);
    }
    catch (Exception ex)
    {
        return Results.BadRequest(new { error = ex.Message });
    }
})
.WithName("CompleteTicket")
.WithOpenApi();

app.MapPost("/api/tickets/{ticketId}/cancel", async (Guid ticketId, [FromBody] CancelRequest? request, QueueManagementClient client) =>
{
    try
    {
        var ticket = await client.Tickets.CancelAsync(ticketId, request?.Reason);
        return Results.Ok(ticket);
    }
    catch (Exception ex)
    {
        return Results.BadRequest(new { error = ex.Message });
    }
})
.WithName("CancelTicket")
.WithOpenApi();

// Dashboard endpoints
app.MapGet("/api/dashboard/metrics", async (QueueManagementClient client) =>
{
    try
    {
        var metrics = await client.Dashboard.GetMetricsAsync();
        return Results.Ok(metrics);
    }
    catch (Exception ex)
    {
        return Results.BadRequest(new { error = ex.Message });
    }
})
.WithName("GetDashboardMetrics")
.WithOpenApi();

app.MapGet("/api/dashboard/realtime", async (QueueManagementClient client, [FromQuery] Guid? unitId) =>
{
    try
    {
        var stats = await client.Dashboard.GetRealTimeStatisticsAsync(unitId);
        return Results.Ok(stats);
    }
    catch (Exception ex)
    {
        return Results.BadRequest(new { error = ex.Message });
    }
})
.WithName("GetRealTimeStatistics")
.WithOpenApi();

// Webhook endpoint
app.MapPost("/webhooks/queuemanagement", async (HttpRequest request, ILogger<Program> logger) =>
{
    // Read the request body
    using var reader = new StreamReader(request.Body);
    var payload = await reader.ReadToEndAsync();

    // Get the signature from headers
    var signature = request.Headers["X-Webhook-Signature"].FirstOrDefault();
    
    if (string.IsNullOrEmpty(signature))
    {
        logger.LogWarning("Webhook request received without signature");
        return Results.BadRequest("Missing signature");
    }

    // Get the webhook secret from configuration
    var secret = app.Configuration["QueueManagement:WebhookSecret"] ?? "your-webhook-secret";

    // Validate the signature
    if (!WebhookHelpers.ValidateSignature(payload, signature, secret))
    {
        logger.LogWarning("Webhook signature validation failed");
        return Results.Unauthorized();
    }

    // Parse the event
    try
    {
        var webhookEvent = WebhookHelpers.ParseEvent<dynamic>(payload);
        
        logger.LogInformation("Webhook event received: {EventType} (ID: {EventId})", 
            webhookEvent.Event, webhookEvent.Id);

        // Process the event based on type
        switch (webhookEvent.Event.ToLower())
        {
            case "ticket.created":
                logger.LogInformation("New ticket created");
                break;
                
            case "ticket.called":
                logger.LogInformation("Ticket called for service");
                break;
                
            case "ticket.completed":
                logger.LogInformation("Ticket service completed");
                break;
                
            case "queue.status_changed":
                logger.LogInformation("Queue status changed");
                break;
                
            default:
                logger.LogInformation("Unhandled event type: {EventType}", webhookEvent.Event);
                break;
        }

        return Results.Ok();
    }
    catch (Exception ex)
    {
        logger.LogError(ex, "Failed to process webhook");
        return Results.BadRequest("Invalid payload");
    }
})
.WithName("WebhookEndpoint")
.WithOpenApi();

// SignalR connection status endpoint
app.MapGet("/api/signalr/status", (QueueManagementClient client) =>
{
    return Results.Ok(new
    {
        isConnected = client.SignalR.IsConnected,
        state = client.SignalR.State.ToString()
    });
})
.WithName("GetSignalRStatus")
.WithOpenApi();

// Connect to SignalR on startup
var serviceProvider = app.Services.CreateScope().ServiceProvider;
var queueClient = serviceProvider.GetRequiredService<QueueManagementClient>();

if (queueClient.SignalR != null && !queueClient.SignalR.IsConnected)
{
    try
    {
        await queueClient.SignalR.ConnectAsync();
        app.Logger.LogInformation("Connected to SignalR hub");
        
        // Subscribe to events
        queueClient.SignalR.TicketCreated += (sender, args) =>
        {
            app.Logger.LogInformation("SignalR: Ticket created - {TicketNumber}", args.Ticket.Number);
        };
        
        queueClient.SignalR.TicketCalled += (sender, args) =>
        {
            app.Logger.LogInformation("SignalR: Ticket called - {TicketNumber} to counter {Counter}", 
                args.TicketNumber, args.CounterNumber);
        };
        
        queueClient.SignalR.QueueStatusChanged += (sender, args) =>
        {
            app.Logger.LogInformation("SignalR: Queue status changed - {Queue}: {OldStatus} → {NewStatus}", 
                args.QueueName, args.OldStatus, args.NewStatus);
        };
    }
    catch (Exception ex)
    {
        app.Logger.LogError(ex, "Failed to connect to SignalR hub");
    }
}

app.Run();

// Request DTOs
record CancelRequest(string? Reason);

// Make Program class accessible for integration tests
public partial class Program { }