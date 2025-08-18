using QueueManagement.SDK;
using QueueManagement.SDK.Configuration;
using QueueManagement.SDK.Models.Common;
using QueueManagement.SDK.Models.Tickets;
using QueueManagement.SDK.SignalR.Events;

// Configuration - Replace with your actual API key and base URL
var apiKey = Environment.GetEnvironmentVariable("QUEUEMANAGEMENT_API_KEY") ?? "your-api-key-here";
var baseUrl = Environment.GetEnvironmentVariable("QUEUEMANAGEMENT_BASE_URL") ?? "https://api.queuemanagement.io";

Console.WriteLine("===========================================");
Console.WriteLine("QueueManagement SDK - Console Example");
Console.WriteLine("===========================================\n");

// Create the client
var client = new QueueManagementClient(new QueueManagementOptions
{
    ApiKey = apiKey,
    BaseUrl = baseUrl,
    EnableLogging = true,
    AutoConnectSignalR = false // We'll connect manually for the demo
});

try
{
    // 1. Check API health
    Console.WriteLine("1. Checking API health...");
    var isHealthy = await client.HealthCheckAsync();
    Console.WriteLine($"   API Status: {(isHealthy ? "✅ Healthy" : "❌ Unhealthy")}\n");

    if (!isHealthy)
    {
        Console.WriteLine("API is not healthy. Please check your configuration.");
        return;
    }

    // 2. Get API info
    Console.WriteLine("2. Getting API information...");
    var apiInfo = await client.GetApiInfoAsync();
    Console.WriteLine($"   Version: {apiInfo.Version}");
    Console.WriteLine($"   Environment: {apiInfo.Environment}");
    Console.WriteLine($"   Server Time: {apiInfo.ServerTime:yyyy-MM-dd HH:mm:ss} UTC\n");

    // 3. List all units
    Console.WriteLine("3. Fetching units...");
    var units = await client.Units.GetAllAsync();
    if (units.Any())
    {
        Console.WriteLine($"   Found {units.Count} unit(s):");
        foreach (var unit in units.Take(3))
        {
            Console.WriteLine($"   - {unit.Name} ({unit.Code}) - {(unit.IsOpen ? "Open" : "Closed")}");
        }
        Console.WriteLine();
    }
    else
    {
        Console.WriteLine("   No units found.\n");
    }

    // 4. List queues (if units exist)
    if (units.Any())
    {
        var firstUnit = units.First();
        Console.WriteLine($"4. Fetching queues for unit '{firstUnit.Name}'...");
        var queues = await client.Queues.GetByUnitAsync(firstUnit.Id);
        
        if (queues.Any())
        {
            Console.WriteLine($"   Found {queues.Count} queue(s):");
            foreach (var queue in queues.Take(3))
            {
                Console.WriteLine($"   - {queue.Name} ({queue.Code})");
                Console.WriteLine($"     Status: {queue.Status}, Waiting: {queue.CurrentSize}, Capacity: {queue.MaxCapacity}");
            }
            Console.WriteLine();

            // 5. Create a ticket (interactive)
            var firstQueue = queues.First();
            Console.WriteLine($"5. Would you like to create a ticket in queue '{firstQueue.Name}'? (y/n)");
            var createTicket = Console.ReadLine()?.ToLower() == "y";

            if (createTicket)
            {
                Console.Write("   Enter customer name: ");
                var customerName = Console.ReadLine() ?? "Test Customer";

                Console.Write("   Enter customer phone (optional): ");
                var customerPhone = Console.ReadLine();

                Console.WriteLine("   Creating ticket...");
                
                var ticket = await client.Tickets.CreateAsync(new CreateTicketRequest
                {
                    QueueId = firstQueue.Id,
                    ServiceId = firstQueue.ServiceIds.FirstOrDefault(),
                    CustomerName = customerName,
                    CustomerPhone = customerPhone,
                    Priority = Priority.Normal,
                    Notes = "Created via SDK Console Example"
                });

                Console.WriteLine($"\n   ✅ Ticket created successfully!");
                Console.WriteLine($"   Ticket Number: {ticket.Number}");
                Console.WriteLine($"   Position in Queue: {ticket.Position}");
                Console.WriteLine($"   Estimated Wait: {ticket.EstimatedWaitMinutes} minutes");
                Console.WriteLine($"   Status: {ticket.Status}\n");

                // 6. Connect to SignalR for real-time updates
                Console.WriteLine("6. Connecting to real-time updates...");
                
                // Subscribe to events before connecting
                client.SignalR.Connected += (sender, args) =>
                {
                    Console.WriteLine($"   ✅ Connected to real-time hub (ID: {args.ConnectionId})");
                };

                client.SignalR.TicketCalled += (sender, args) =>
                {
                    Console.WriteLine($"\n   🔔 TICKET CALLED: {args.TicketNumber}");
                    Console.WriteLine($"      Please proceed to counter {args.CounterNumber}");
                    Console.WriteLine($"      Agent: {args.AgentName}");
                };

                client.SignalR.QueueStatusChanged += (sender, args) =>
                {
                    Console.WriteLine($"\n   📊 QUEUE STATUS CHANGED: {args.QueueName}");
                    Console.WriteLine($"      {args.OldStatus} → {args.NewStatus}");
                };

                client.SignalR.TicketCompleted += (sender, args) =>
                {
                    Console.WriteLine($"\n   ✅ TICKET COMPLETED: {args.TicketNumber}");
                    Console.WriteLine($"      Service Duration: {args.ServiceDurationMinutes:F1} minutes");
                };

                try
                {
                    await client.SignalR.ConnectAsync();
                    await client.SignalR.JoinQueueAsync(firstQueue.Id);
                    
                    Console.WriteLine($"   Joined queue '{firstQueue.Name}' for updates\n");
                    
                    // 7. Monitor ticket status
                    Console.WriteLine("7. Monitoring ticket status (press 'q' to quit)...");
                    Console.WriteLine("   Commands:");
                    Console.WriteLine("   - 's' to check ticket status");
                    Console.WriteLine("   - 'p' to check queue position");
                    Console.WriteLine("   - 'c' to cancel ticket");
                    Console.WriteLine("   - 'q' to quit\n");

                    bool monitoring = true;
                    while (monitoring)
                    {
                        var key = Console.ReadKey(true);
                        
                        switch (key.KeyChar)
                        {
                            case 's':
                                var status = await client.Tickets.GetStatusAsync(ticket.Id);
                                Console.WriteLine($"\n   Ticket Status: {status.Status}");
                                Console.WriteLine($"   Position: {status.Position}");
                                Console.WriteLine($"   Estimated Wait: {status.EstimatedWaitMinutes} minutes");
                                if (!string.IsNullOrEmpty(status.CounterNumber))
                                {
                                    Console.WriteLine($"   Counter: {status.CounterNumber}");
                                }
                                break;

                            case 'p':
                                var position = await client.Tickets.GetPositionAsync(ticket.Id);
                                Console.WriteLine($"\n   Current Position: {position.CurrentPosition}");
                                Console.WriteLine($"   Tickets Ahead: {position.TicketsAhead}");
                                Console.WriteLine($"   Estimated Wait: {position.EstimatedWaitMinutes} minutes");
                                Console.WriteLine($"   Active Agents: {position.ActiveAgents}");
                                break;

                            case 'c':
                                Console.Write("\n   Are you sure you want to cancel the ticket? (y/n): ");
                                if (Console.ReadLine()?.ToLower() == "y")
                                {
                                    await client.Tickets.CancelAsync(ticket.Id, "Cancelled by customer");
                                    Console.WriteLine("   Ticket cancelled.");
                                    monitoring = false;
                                }
                                break;

                            case 'q':
                                monitoring = false;
                                break;
                        }
                    }

                    // Disconnect from SignalR
                    await client.SignalR.LeaveQueueAsync(firstQueue.Id);
                    await client.SignalR.DisconnectAsync();
                    Console.WriteLine("\n   Disconnected from real-time updates.");
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"   ⚠️ SignalR connection failed: {ex.Message}");
                }
            }
        }
        else
        {
            Console.WriteLine("   No queues found in this unit.\n");
        }
    }

    // 8. Dashboard metrics (if available)
    Console.WriteLine("\n8. Fetching dashboard metrics...");
    try
    {
        var metrics = await client.Dashboard.GetMetricsAsync();
        Console.WriteLine($"   Active Units: {metrics.ActiveUnits}");
        Console.WriteLine($"   Active Queues: {metrics.ActiveQueues}");
        Console.WriteLine($"   Active Sessions: {metrics.ActiveSessions}");
        Console.WriteLine($"   Total Waiting: {metrics.TotalWaitingTickets}");
        Console.WriteLine($"   Total Serving: {metrics.TotalServingTickets}");
        Console.WriteLine($"   Tickets Today: {metrics.TicketsCreatedToday}");
        Console.WriteLine($"   Completed Today: {metrics.TicketsCompletedToday}");
        Console.WriteLine($"   Avg Wait Time: {metrics.AverageWaitTime:F1} minutes");
        Console.WriteLine($"   Avg Service Time: {metrics.AverageServiceTime:F1} minutes");
        
        if (metrics.CustomerSatisfactionScore.HasValue)
        {
            Console.WriteLine($"   Customer Satisfaction: {metrics.CustomerSatisfactionScore:F1}/5.0");
        }
    }
    catch (Exception ex)
    {
        Console.WriteLine($"   Could not fetch metrics: {ex.Message}");
    }
}
catch (Exception ex)
{
    Console.WriteLine($"\n❌ Error: {ex.Message}");
    if (ex.InnerException != null)
    {
        Console.WriteLine($"   Inner: {ex.InnerException.Message}");
    }
}
finally
{
    // Clean up
    client.Dispose();
}

Console.WriteLine("\n===========================================");
Console.WriteLine("Example completed. Press any key to exit...");
Console.ReadKey();