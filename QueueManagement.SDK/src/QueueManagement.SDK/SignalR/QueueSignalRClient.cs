using Microsoft.AspNetCore.SignalR.Client;
using Microsoft.Extensions.Logging;
using QueueManagement.SDK.Configuration;
using QueueManagement.SDK.Exceptions;
using QueueManagement.SDK.SignalR.Events;

namespace QueueManagement.SDK.SignalR;

/// <summary>
/// Implementation of the SignalR client for real-time updates.
/// </summary>
internal class QueueSignalRClient : IQueueSignalRClient
{
    private readonly QueueManagementOptions _options;
    private readonly ILogger<QueueSignalRClient>? _logger;
    private HubConnection? _connection;
    private readonly SemaphoreSlim _connectionLock = new(1, 1);
    private bool _disposed;

    public bool IsConnected => _connection?.State == HubConnectionState.Connected;
    
    public ConnectionState State => _connection?.State switch
    {
        HubConnectionState.Connected => ConnectionState.Connected,
        HubConnectionState.Connecting => ConnectionState.Connecting,
        HubConnectionState.Reconnecting => ConnectionState.Reconnecting,
        _ => ConnectionState.Disconnected
    };

    // Events
    public event EventHandler<ConnectedEventArgs>? Connected;
    public event EventHandler<DisconnectedEventArgs>? Disconnected;
    public event EventHandler<ReconnectingEventArgs>? Reconnecting;
    public event EventHandler<ReconnectedEventArgs>? Reconnected;
    public event EventHandler<TicketCreatedEventArgs>? TicketCreated;
    public event EventHandler<TicketCalledEventArgs>? TicketCalled;
    public event EventHandler<TicketServingEventArgs>? TicketServing;
    public event EventHandler<TicketCompletedEventArgs>? TicketCompleted;
    public event EventHandler<TicketCancelledEventArgs>? TicketCancelled;
    public event EventHandler<TicketTransferredEventArgs>? TicketTransferred;
    public event EventHandler<QueueStatusChangedEventArgs>? QueueStatusChanged;
    public event EventHandler<QueueMetricsUpdatedEventArgs>? QueueMetricsUpdated;
    public event EventHandler<SessionStartedEventArgs>? SessionStarted;
    public event EventHandler<SessionCompletedEventArgs>? SessionCompleted;
    public event EventHandler<DashboardMetricsUpdatedEventArgs>? DashboardMetricsUpdated;
    public event EventHandler<AlertTriggeredEventArgs>? AlertTriggered;

    public QueueSignalRClient(QueueManagementOptions options, ILogger<QueueSignalRClient>? logger = null)
    {
        _options = options ?? throw new ArgumentNullException(nameof(options));
        _logger = logger;
    }

    public async Task ConnectAsync(CancellationToken cancellationToken = default)
    {
        await _connectionLock.WaitAsync(cancellationToken).ConfigureAwait(false);
        try
        {
            if (IsConnected)
            {
                _logger?.LogDebug("Already connected to SignalR hub");
                return;
            }

            if (_connection == null)
            {
                _connection = BuildConnection();
                SetupEventHandlers();
            }

            _logger?.LogInformation("Connecting to SignalR hub at {Url}", _options.GetSignalRUrl());
            
            await _connection.StartAsync(cancellationToken).ConfigureAwait(false);
            
            _logger?.LogInformation("Connected to SignalR hub with connection ID: {ConnectionId}", _connection.ConnectionId);
            
            Connected?.Invoke(this, new ConnectedEventArgs 
            { 
                ConnectionId = _connection.ConnectionId ?? string.Empty 
            });
        }
        catch (Exception ex)
        {
            _logger?.LogError(ex, "Failed to connect to SignalR hub");
            throw new QueueManagementSignalRException("Failed to connect to SignalR hub", ex);
        }
        finally
        {
            _connectionLock.Release();
        }
    }

    public async Task DisconnectAsync(CancellationToken cancellationToken = default)
    {
        await _connectionLock.WaitAsync(cancellationToken).ConfigureAwait(false);
        try
        {
            if (_connection == null || !IsConnected)
            {
                _logger?.LogDebug("Not connected to SignalR hub");
                return;
            }

            _logger?.LogInformation("Disconnecting from SignalR hub");
            
            await _connection.StopAsync(cancellationToken).ConfigureAwait(false);
            
            _logger?.LogInformation("Disconnected from SignalR hub");
            
            Disconnected?.Invoke(this, new DisconnectedEventArgs());
        }
        catch (Exception ex)
        {
            _logger?.LogError(ex, "Error disconnecting from SignalR hub");
            throw new QueueManagementSignalRException("Failed to disconnect from SignalR hub", ex);
        }
        finally
        {
            _connectionLock.Release();
        }
    }

    public async Task JoinQueueAsync(Guid queueId, CancellationToken cancellationToken = default)
    {
        EnsureConnected();
        
        try
        {
            await _connection!.InvokeAsync("JoinQueue", queueId, cancellationToken).ConfigureAwait(false);
            _logger?.LogDebug("Joined queue {QueueId}", queueId);
        }
        catch (Exception ex)
        {
            _logger?.LogError(ex, "Failed to join queue {QueueId}", queueId);
            throw new QueueManagementSignalRException($"Failed to join queue {queueId}", ex);
        }
    }

    public async Task LeaveQueueAsync(Guid queueId, CancellationToken cancellationToken = default)
    {
        EnsureConnected();
        
        try
        {
            await _connection!.InvokeAsync("LeaveQueue", queueId, cancellationToken).ConfigureAwait(false);
            _logger?.LogDebug("Left queue {QueueId}", queueId);
        }
        catch (Exception ex)
        {
            _logger?.LogError(ex, "Failed to leave queue {QueueId}", queueId);
            throw new QueueManagementSignalRException($"Failed to leave queue {queueId}", ex);
        }
    }

    public async Task JoinUnitAsync(Guid unitId, CancellationToken cancellationToken = default)
    {
        EnsureConnected();
        
        try
        {
            await _connection!.InvokeAsync("JoinUnit", unitId, cancellationToken).ConfigureAwait(false);
            _logger?.LogDebug("Joined unit {UnitId}", unitId);
        }
        catch (Exception ex)
        {
            _logger?.LogError(ex, "Failed to join unit {UnitId}", unitId);
            throw new QueueManagementSignalRException($"Failed to join unit {unitId}", ex);
        }
    }

    public async Task LeaveUnitAsync(Guid unitId, CancellationToken cancellationToken = default)
    {
        EnsureConnected();
        
        try
        {
            await _connection!.InvokeAsync("LeaveUnit", unitId, cancellationToken).ConfigureAwait(false);
            _logger?.LogDebug("Left unit {UnitId}", unitId);
        }
        catch (Exception ex)
        {
            _logger?.LogError(ex, "Failed to leave unit {UnitId}", unitId);
            throw new QueueManagementSignalRException($"Failed to leave unit {unitId}", ex);
        }
    }

    public async Task JoinDashboardAsync(CancellationToken cancellationToken = default)
    {
        EnsureConnected();
        
        try
        {
            await _connection!.InvokeAsync("JoinDashboard", cancellationToken).ConfigureAwait(false);
            _logger?.LogDebug("Joined dashboard");
        }
        catch (Exception ex)
        {
            _logger?.LogError(ex, "Failed to join dashboard");
            throw new QueueManagementSignalRException("Failed to join dashboard", ex);
        }
    }

    public async Task LeaveDashboardAsync(CancellationToken cancellationToken = default)
    {
        EnsureConnected();
        
        try
        {
            await _connection!.InvokeAsync("LeaveDashboard", cancellationToken).ConfigureAwait(false);
            _logger?.LogDebug("Left dashboard");
        }
        catch (Exception ex)
        {
            _logger?.LogError(ex, "Failed to leave dashboard");
            throw new QueueManagementSignalRException("Failed to leave dashboard", ex);
        }
    }

    public async Task RequestQueueStatusAsync(Guid queueId, CancellationToken cancellationToken = default)
    {
        EnsureConnected();
        
        try
        {
            await _connection!.InvokeAsync("RequestQueueStatus", queueId, cancellationToken).ConfigureAwait(false);
            _logger?.LogDebug("Requested queue status for {QueueId}", queueId);
        }
        catch (Exception ex)
        {
            _logger?.LogError(ex, "Failed to request queue status for {QueueId}", queueId);
            throw new QueueManagementSignalRException($"Failed to request queue status for {queueId}", ex);
        }
    }

    public async Task RequestDashboardMetricsAsync(CancellationToken cancellationToken = default)
    {
        EnsureConnected();
        
        try
        {
            await _connection!.InvokeAsync("RequestDashboardMetrics", cancellationToken).ConfigureAwait(false);
            _logger?.LogDebug("Requested dashboard metrics");
        }
        catch (Exception ex)
        {
            _logger?.LogError(ex, "Failed to request dashboard metrics");
            throw new QueueManagementSignalRException("Failed to request dashboard metrics", ex);
        }
    }

    private HubConnection BuildConnection()
    {
        var builder = new HubConnectionBuilder()
            .WithUrl(_options.GetSignalRUrl(), options =>
            {
                options.AccessTokenProvider = () => Task.FromResult<string?>(_options.ApiKey);
                
                if (!_options.ValidateSslCertificate)
                {
                    options.HttpMessageHandlerFactory = (handler) =>
                    {
                        if (handler is HttpClientHandler clientHandler)
                        {
                            clientHandler.ServerCertificateCustomValidationCallback = 
                                HttpClientHandler.DangerousAcceptAnyServerCertificateValidator;
                        }
                        return handler;
                    };
                }
            })
            .WithAutomaticReconnect(new[] 
            { 
                TimeSpan.Zero,
                TimeSpan.FromSeconds(2),
                TimeSpan.FromSeconds(5),
                TimeSpan.FromSeconds(10),
                TimeSpan.FromSeconds(30)
            });

        if (_logger != null)
        {
            builder.ConfigureLogging(logging =>
            {
                logging.SetMinimumLevel(_options.LogLevel);
            });
        }

        return builder.Build();
    }

    private void SetupEventHandlers()
    {
        if (_connection == null) return;

        // Connection lifecycle events
        _connection.Closed += OnClosed;
        _connection.Reconnecting += OnReconnecting;
        _connection.Reconnected += OnReconnected;

        // Ticket events
        _connection.On<TicketCreatedEventArgs>("TicketCreated", args =>
        {
            _logger?.LogDebug("Received TicketCreated event for ticket {TicketNumber}", args.Ticket.Number);
            TicketCreated?.Invoke(this, args);
        });

        _connection.On<TicketCalledEventArgs>("TicketCalled", args =>
        {
            _logger?.LogDebug("Received TicketCalled event for ticket {TicketNumber}", args.TicketNumber);
            TicketCalled?.Invoke(this, args);
        });

        _connection.On<TicketServingEventArgs>("TicketServing", args =>
        {
            _logger?.LogDebug("Received TicketServing event for ticket {TicketNumber}", args.TicketNumber);
            TicketServing?.Invoke(this, args);
        });

        _connection.On<TicketCompletedEventArgs>("TicketCompleted", args =>
        {
            _logger?.LogDebug("Received TicketCompleted event for ticket {TicketNumber}", args.TicketNumber);
            TicketCompleted?.Invoke(this, args);
        });

        _connection.On<TicketCancelledEventArgs>("TicketCancelled", args =>
        {
            _logger?.LogDebug("Received TicketCancelled event for ticket {TicketNumber}", args.TicketNumber);
            TicketCancelled?.Invoke(this, args);
        });

        _connection.On<TicketTransferredEventArgs>("TicketTransferred", args =>
        {
            _logger?.LogDebug("Received TicketTransferred event for ticket {TicketNumber}", args.TicketNumber);
            TicketTransferred?.Invoke(this, args);
        });

        // Queue events
        _connection.On<QueueStatusChangedEventArgs>("QueueStatusChanged", args =>
        {
            _logger?.LogDebug("Received QueueStatusChanged event for queue {QueueName}", args.QueueName);
            QueueStatusChanged?.Invoke(this, args);
        });

        _connection.On<QueueMetricsUpdatedEventArgs>("QueueMetricsUpdated", args =>
        {
            _logger?.LogDebug("Received QueueMetricsUpdated event");
            QueueMetricsUpdated?.Invoke(this, args);
        });

        // Session events
        _connection.On<SessionStartedEventArgs>("SessionStarted", args =>
        {
            _logger?.LogDebug("Received SessionStarted event for session {SessionId}", args.Session.Id);
            SessionStarted?.Invoke(this, args);
        });

        _connection.On<SessionCompletedEventArgs>("SessionCompleted", args =>
        {
            _logger?.LogDebug("Received SessionCompleted event for session {SessionId}", args.SessionId);
            SessionCompleted?.Invoke(this, args);
        });

        // Dashboard events
        _connection.On<DashboardMetricsUpdatedEventArgs>("DashboardMetricsUpdated", args =>
        {
            _logger?.LogDebug("Received DashboardMetricsUpdated event");
            DashboardMetricsUpdated?.Invoke(this, args);
        });

        _connection.On<AlertTriggeredEventArgs>("AlertTriggered", args =>
        {
            _logger?.LogInformation("Received AlertTriggered event: {AlertMessage}", args.Alert.Message);
            AlertTriggered?.Invoke(this, args);
        });
    }

    private async Task OnClosed(Exception? exception)
    {
        _logger?.LogWarning(exception, "SignalR connection closed");
        
        await Task.Run(() =>
        {
            Disconnected?.Invoke(this, new DisconnectedEventArgs
            {
                Exception = exception,
                Reason = exception?.Message
            });
        }).ConfigureAwait(false);
    }

    private async Task OnReconnecting(Exception? exception)
    {
        _logger?.LogInformation("SignalR connection reconnecting");
        
        await Task.Run(() =>
        {
            Reconnecting?.Invoke(this, new ReconnectingEventArgs
            {
                RetryAttempt = 1,
                RetryDelay = TimeSpan.FromSeconds(2)
            });
        }).ConfigureAwait(false);
    }

    private async Task OnReconnected(string? connectionId)
    {
        _logger?.LogInformation("SignalR connection reconnected with ID: {ConnectionId}", connectionId);
        
        await Task.Run(() =>
        {
            Reconnected?.Invoke(this, new ReconnectedEventArgs
            {
                ConnectionId = connectionId ?? string.Empty,
                ReconnectionTime = TimeSpan.Zero
            });
        }).ConfigureAwait(false);
    }

    private void EnsureConnected()
    {
        if (!IsConnected)
        {
            throw new QueueManagementSignalRException("SignalR client is not connected. Call ConnectAsync() first.");
        }
    }

    public void Dispose()
    {
        Dispose(true);
        GC.SuppressFinalize(this);
    }

    protected virtual void Dispose(bool disposing)
    {
        if (_disposed)
            return;

        if (disposing)
        {
            try
            {
                if (_connection != null)
                {
                    _connection.Closed -= OnClosed;
                    _connection.Reconnecting -= OnReconnecting;
                    _connection.Reconnected -= OnReconnected;
                    
                    if (IsConnected)
                    {
                        _connection.StopAsync().GetAwaiter().GetResult();
                    }
                    
                    _connection.DisposeAsync().GetAwaiter().GetResult();
                    _connection = null;
                }

                _connectionLock?.Dispose();
            }
            catch (Exception ex)
            {
                _logger?.LogError(ex, "Error disposing SignalR client");
            }
        }

        _disposed = true;
    }
}