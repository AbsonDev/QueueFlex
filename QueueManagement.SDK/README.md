# QueueManagement.SDK

[![NuGet](https://img.shields.io/nuget/v/QueueManagement.SDK.svg)](https://www.nuget.org/packages/QueueManagement.SDK/)
[![License: MIT](https://img.shields.io/badge/License-MIT-yellow.svg)](https://opensource.org/licenses/MIT)
[![.NET](https://img.shields.io/badge/.NET-8.0-512BD4)](https://dotnet.microsoft.com)

Official .NET SDK for the QueueManagement API - Build powerful queue management applications with ease.

## 🚀 Quick Start

### Installation

```bash
dotnet add package QueueManagement.SDK
```

### Basic Usage

```csharp
using QueueManagement.SDK;

// Create client
var client = new QueueManagementClient("your-api-key");

// Create a ticket
var ticket = await client.Tickets.CreateAsync(new CreateTicketRequest
{
    QueueId = queueId,
    ServiceId = serviceId,
    CustomerName = "John Doe",
    Priority = Priority.Normal
});

Console.WriteLine($"Ticket created: {ticket.Number}");
Console.WriteLine($"Position in queue: {ticket.Position}");
```

### ASP.NET Core Integration

```csharp
// Program.cs or Startup.cs
builder.Services.AddQueueManagementClient(options =>
{
    options.ApiKey = "your-api-key";
    options.BaseUrl = "https://api.queuemanagement.io";
    options.EnableLogging = true;
});

// In your controller or minimal API
app.MapPost("/tickets", async (QueueManagementClient client, CreateTicketRequest request) =>
{
    var ticket = await client.Tickets.CreateAsync(request);
    return Results.Ok(ticket);
});
```

## ✨ Features

- ✅ **Complete API Coverage** - All endpoints implemented
- ✅ **Strongly Typed** - Full IntelliSense support
- ✅ **Async/Await** - Modern async patterns throughout
- ✅ **SignalR Real-time** - Built-in real-time updates
- ✅ **Webhook Support** - Easy webhook handling
- ✅ **Retry Policies** - Automatic retry with exponential backoff
- ✅ **ASP.NET Core Ready** - Dependency injection support
- ✅ **Comprehensive Logging** - Detailed logging support
- ✅ **Thread-Safe** - Safe for concurrent use
- ✅ **Memory Efficient** - Optimized for performance

## 📚 Documentation

- [Getting Started Guide](docs/QUICKSTART.md)
- [API Reference](docs/API_REFERENCE.md)
- [Webhook Integration](docs/WEBHOOKS.md)
- [SignalR Real-time Events](docs/SIGNALR.md)
- [Examples](examples/)

## 🔧 Configuration

### Using Options Object

```csharp
var client = new QueueManagementClient(new QueueManagementOptions
{
    ApiKey = "your-api-key",
    BaseUrl = "https://api.queuemanagement.io",
    Timeout = TimeSpan.FromSeconds(30),
    MaxRetries = 3,
    EnableLogging = true,
    AutoConnectSignalR = true
});
```

### Using Configuration File (appsettings.json)

```json
{
  "QueueManagement": {
    "ApiKey": "your-api-key",
    "BaseUrl": "https://api.queuemanagement.io",
    "TenantId": "your-tenant-id",
    "Timeout": "00:00:30",
    "MaxRetries": 3,
    "EnableLogging": true
  }
}
```

```csharp
builder.Services.AddQueueManagementClient(
    builder.Configuration,
    "QueueManagement"
);
```

## 🎯 Core Features

### Ticket Management

```csharp
// Create a ticket
var ticket = await client.Tickets.CreateAsync(new CreateTicketRequest
{
    QueueId = queueId,
    ServiceId = serviceId,
    CustomerName = "Jane Smith",
    CustomerPhone = "+1234567890",
    Priority = Priority.High,
    Notes = "VIP Customer"
});

// Get ticket status
var status = await client.Tickets.GetStatusAsync(ticket.Id);

// Call ticket for service
await client.Tickets.CallAsync(ticket.Id);

// Complete service
await client.Tickets.CompleteAsync(ticket.Id);

// Get queue position
var position = await client.Tickets.GetPositionAsync(ticket.Id);
Console.WriteLine($"Position: {position.CurrentPosition}, Wait time: {position.EstimatedWaitMinutes} min");
```

### Queue Management

```csharp
// Get all queues
var queues = await client.Queues.GetAllAsync();

// Get queue status
var status = await client.Queues.GetStatusAsync(queueId);
Console.WriteLine($"Waiting: {status.WaitingTickets}, Avg wait: {status.AverageWaitTime} min");

// Get queue metrics
var metrics = await client.Queues.GetMetricsAsync(queueId);

// Update queue status
await client.Queues.OpenAsync(queueId);
await client.Queues.PauseAsync(queueId);
await client.Queues.CloseAsync(queueId);
```

### Real-time Updates with SignalR

```csharp
// Connect to SignalR
await client.SignalR.ConnectAsync();

// Subscribe to events
client.SignalR.TicketCreated += (sender, args) =>
{
    Console.WriteLine($"New ticket: {args.Ticket.Number}");
};

client.SignalR.TicketCalled += (sender, args) =>
{
    Console.WriteLine($"Ticket {args.TicketNumber} called to counter {args.CounterNumber}");
};

client.SignalR.QueueStatusChanged += (sender, args) =>
{
    Console.WriteLine($"Queue {args.QueueName}: {args.OldStatus} → {args.NewStatus}");
};

// Join queue for updates
await client.SignalR.JoinQueueAsync(queueId);

// Join unit for all queues
await client.SignalR.JoinUnitAsync(unitId);
```

### Webhook Handling

```csharp
// Validate webhook signature
var isValid = WebhookHelpers.ValidateSignature(payload, signature, secret);

// Parse webhook event
var webhookEvent = WebhookHelpers.ParseEvent<TicketCreatedData>(payload);

// Handle event
switch (webhookEvent.Event)
{
    case "ticket.created":
        HandleTicketCreated(webhookEvent.Data);
        break;
    case "ticket.called":
        HandleTicketCalled(webhookEvent.Data);
        break;
}
```

### Dashboard & Analytics

```csharp
// Get dashboard metrics
var metrics = await client.Dashboard.GetMetricsAsync();
Console.WriteLine($"Active queues: {metrics.ActiveQueues}");
Console.WriteLine($"Waiting tickets: {metrics.TotalWaitingTickets}");
Console.WriteLine($"Avg wait time: {metrics.AverageWaitTime} min");

// Get real-time statistics
var stats = await client.Dashboard.GetRealTimeStatisticsAsync();

// Get agent performance
var performance = await client.Dashboard.GetAgentPerformanceAsync(agentId);
```

## 🔌 Dependency Injection

### Basic Setup

```csharp
services.AddQueueManagementClient(options =>
{
    options.ApiKey = configuration["QueueManagement:ApiKey"];
});
```

### With Health Checks

```csharp
services.AddHealthChecks()
    .AddQueueManagementHealthCheck("queue-api");
```

### Individual Service Registration

```csharp
// Register individual services if needed
services.AddScoped<ITicketsClient>(sp => 
    sp.GetRequiredService<QueueManagementClient>().Tickets);

services.AddScoped<IQueuesClient>(sp => 
    sp.GetRequiredService<QueueManagementClient>().Queues);
```

## 🛠️ Advanced Usage

### Custom HTTP Client

```csharp
var httpClient = new HttpClient
{
    BaseAddress = new Uri("https://api.queuemanagement.io"),
    Timeout = TimeSpan.FromSeconds(60)
};

var client = new QueueManagementClient(
    httpClient,
    options,
    loggerFactory,
    disposeHttpClient: false
);
```

### Streaming Results

```csharp
// Stream tickets in real-time
await foreach (var ticket in client.Tickets.StreamQueueTicketsAsync(queueId))
{
    Console.WriteLine($"Ticket: {ticket.Number}, Status: {ticket.Status}");
}
```

### Batch Operations

```csharp
// Create multiple tickets at once
var batchRequest = new BatchRequest<CreateTicketRequest>
{
    Items = new List<CreateTicketRequest>
    {
        new() { QueueId = queueId, CustomerName = "Customer 1" },
        new() { QueueId = queueId, CustomerName = "Customer 2" },
        new() { QueueId = queueId, CustomerName = "Customer 3" }
    }
};

var batchResponse = await client.Tickets.CreateBatchAsync(batchRequest);
Console.WriteLine($"Created {batchResponse.SuccessCount} tickets");
```

## 📊 Error Handling

```csharp
try
{
    var ticket = await client.Tickets.GetAsync(ticketId);
}
catch (QueueManagementNotFoundException ex)
{
    Console.WriteLine($"Ticket not found: {ex.Message}");
}
catch (QueueManagementAuthenticationException ex)
{
    Console.WriteLine($"Authentication failed: {ex.Message}");
}
catch (QueueManagementRateLimitException ex)
{
    Console.WriteLine($"Rate limited. Retry after: {ex.RetryAfter}");
}
catch (QueueManagementException ex)
{
    Console.WriteLine($"API error: {ex.Message}");
    if (ex.ValidationErrors?.Any() == true)
    {
        foreach (var error in ex.ValidationErrors)
        {
            Console.WriteLine($"  - {error.Field}: {error.Message}");
        }
    }
}
```

## 🧪 Testing

```csharp
// Use in-memory test client
var options = new QueueManagementOptions
{
    ApiKey = "test-key",
    BaseUrl = "http://localhost:5000"
};

var client = new QueueManagementClient(options);

// Mock HTTP responses for unit tests
var mockHttp = new MockHttpMessageHandler();
mockHttp.When("*/api/tickets")
    .Respond("application/json", "{ \"id\": \"...\", \"number\": \"A001\" }");

var httpClient = new HttpClient(mockHttp);
var testClient = new QueueManagementClient(httpClient, options);
```

## 📦 NuGet Package

```xml
<PackageReference Include="QueueManagement.SDK" Version="1.0.0" />
```

## 🤝 Contributing

We welcome contributions! Please see our [Contributing Guide](CONTRIBUTING.md) for details.

1. Fork the repository
2. Create your feature branch (`git checkout -b feature/AmazingFeature`)
3. Commit your changes (`git commit -m 'Add some AmazingFeature'`)
4. Push to the branch (`git push origin feature/AmazingFeature`)
5. Open a Pull Request

## 📝 License

This project is licensed under the MIT License - see the [LICENSE](LICENSE) file for details.

## 🆘 Support

- 📧 Email: support@queuemanagement.io
- 💬 Discord: [Join our community](https://discord.gg/queuemanagement)
- 📖 Documentation: [docs.queuemanagement.io](https://docs.queuemanagement.io)
- 🐛 Issues: [GitHub Issues](https://github.com/queuemanagement-oss/dotnet-sdk/issues)

## 🏢 About

Built with ❤️ by the QueueManagement Team

---

**Happy Queue Managing! 🎯**