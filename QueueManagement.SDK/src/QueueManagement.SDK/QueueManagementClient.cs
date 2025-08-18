using System.Net.Http.Headers;
using Microsoft.Extensions.Logging;
using QueueManagement.SDK.Configuration;
using QueueManagement.SDK.Models.Common;
using QueueManagement.SDK.Services;
using QueueManagement.SDK.Services.Implementation;
using QueueManagement.SDK.SignalR;

namespace QueueManagement.SDK;

/// <summary>
/// Main client for interacting with the QueueManagement API.
/// </summary>
public class QueueManagementClient : IDisposable
{
    private readonly HttpClient _httpClient;
    private readonly QueueManagementOptions _options;
    private readonly ILoggerFactory? _loggerFactory;
    private readonly bool _disposeHttpClient;
    private bool _disposed;

    /// <summary>
    /// Gets the tickets client.
    /// </summary>
    public ITicketsClient Tickets { get; }

    /// <summary>
    /// Gets the queues client.
    /// </summary>
    public IQueuesClient Queues { get; }

    /// <summary>
    /// Gets the sessions client.
    /// </summary>
    public ISessionsClient Sessions { get; }

    /// <summary>
    /// Gets the units client.
    /// </summary>
    public IUnitsClient Units { get; }

    /// <summary>
    /// Gets the services client.
    /// </summary>
    public IServicesClient Services { get; }

    /// <summary>
    /// Gets the users client.
    /// </summary>
    public IUsersClient Users { get; }

    /// <summary>
    /// Gets the dashboard client.
    /// </summary>
    public IDashboardClient Dashboard { get; }

    /// <summary>
    /// Gets the webhooks client.
    /// </summary>
    public IWebhooksClient Webhooks { get; }

    /// <summary>
    /// Gets the SignalR client for real-time updates.
    /// </summary>
    public IQueueSignalRClient SignalR { get; }

    /// <summary>
    /// Initializes a new instance of the <see cref="QueueManagementClient"/> class with an API key.
    /// </summary>
    /// <param name="apiKey">The API key for authentication.</param>
    /// <param name="options">Optional configuration options.</param>
    public QueueManagementClient(string apiKey, QueueManagementOptions? options = null)
        : this(options ?? new QueueManagementOptions { ApiKey = apiKey })
    {
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="QueueManagementClient"/> class with options.
    /// </summary>
    /// <param name="options">The configuration options.</param>
    public QueueManagementClient(QueueManagementOptions options)
        : this(new HttpClient(), options, null, true)
    {
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="QueueManagementClient"/> class with a custom HttpClient.
    /// </summary>
    /// <param name="httpClient">The HTTP client to use.</param>
    /// <param name="options">The configuration options.</param>
    /// <param name="loggerFactory">Optional logger factory.</param>
    /// <param name="disposeHttpClient">Whether to dispose the HTTP client on disposal.</param>
    public QueueManagementClient(
        HttpClient httpClient,
        QueueManagementOptions options,
        ILoggerFactory? loggerFactory = null,
        bool disposeHttpClient = false)
    {
        _httpClient = httpClient ?? throw new ArgumentNullException(nameof(httpClient));
        _options = options ?? throw new ArgumentNullException(nameof(options));
        _loggerFactory = loggerFactory;
        _disposeHttpClient = disposeHttpClient;

        // Validate options
        _options.Validate();

        // Configure HTTP client
        ConfigureHttpClient();

        // Initialize service clients
        Tickets = new TicketsClient(_httpClient, _options, CreateLogger<TicketsClient>());
        Queues = new QueuesClient(_httpClient, _options, CreateLogger<QueuesClient>());
        Sessions = new SessionsClient(_httpClient, _options, CreateLogger<SessionsClient>());
        Units = new UnitsClient(_httpClient, _options, CreateLogger<UnitsClient>());
        Services = new ServicesClient(_httpClient, _options, CreateLogger<ServicesClient>());
        Users = new UsersClient(_httpClient, _options, CreateLogger<UsersClient>());
        Dashboard = new DashboardClient(_httpClient, _options, CreateLogger<DashboardClient>());
        Webhooks = new WebhooksClient(_httpClient, _options, CreateLogger<WebhooksClient>());

        // Initialize SignalR client
        SignalR = new QueueSignalRClient(_options, CreateLogger<QueueSignalRClient>());

        // Auto-connect SignalR if configured
        if (_options.AutoConnectSignalR)
        {
            _ = SignalR.ConnectAsync().ConfigureAwait(false);
        }
    }

    /// <summary>
    /// Checks the health of the API.
    /// </summary>
    /// <param name="cancellationToken">The cancellation token.</param>
    /// <returns>True if the API is healthy; otherwise, false.</returns>
    public async Task<bool> HealthCheckAsync(CancellationToken cancellationToken = default)
    {
        try
        {
            var response = await _httpClient.GetAsync("health", cancellationToken).ConfigureAwait(false);
            return response.IsSuccessStatusCode;
        }
        catch
        {
            return false;
        }
    }

    /// <summary>
    /// Gets information about the API.
    /// </summary>
    /// <param name="cancellationToken">The cancellation token.</param>
    /// <returns>The API information.</returns>
    public async Task<ApiInfo> GetApiInfoAsync(CancellationToken cancellationToken = default)
    {
        var response = await _httpClient.GetAsync("api/info", cancellationToken).ConfigureAwait(false);
        response.EnsureSuccessStatusCode();

        var content = await response.Content.ReadAsStringAsync(cancellationToken).ConfigureAwait(false);
        var apiInfo = System.Text.Json.JsonSerializer.Deserialize<ApiInfo>(content, new System.Text.Json.JsonSerializerOptions
        {
            PropertyNamingPolicy = System.Text.Json.JsonNamingPolicy.CamelCase
        });

        return apiInfo ?? new ApiInfo();
    }

    /// <summary>
    /// Configures the HTTP client with default settings.
    /// </summary>
    private void ConfigureHttpClient()
    {
        if (_httpClient.BaseAddress == null)
        {
            _httpClient.BaseAddress = new Uri(_options.BaseUrl);
        }

        _httpClient.Timeout = _options.Timeout;
        
        // Set default headers
        _httpClient.DefaultRequestHeaders.Clear();
        _httpClient.DefaultRequestHeaders.Authorization = 
            new AuthenticationHeaderValue("Bearer", _options.ApiKey);
        
        if (!string.IsNullOrEmpty(_options.UserAgent))
        {
            _httpClient.DefaultRequestHeaders.UserAgent.ParseAdd(_options.UserAgent);
        }
        else
        {
            _httpClient.DefaultRequestHeaders.UserAgent.ParseAdd("QueueManagement.SDK/1.0.0 (.NET)");
        }

        _httpClient.DefaultRequestHeaders.Accept.Add(
            new MediaTypeWithQualityHeaderValue("application/json"));

        if (!string.IsNullOrEmpty(_options.TenantId))
        {
            _httpClient.DefaultRequestHeaders.Add("X-Tenant-Id", _options.TenantId);
        }
    }

    /// <summary>
    /// Creates a logger for the specified type.
    /// </summary>
    private ILogger<T>? CreateLogger<T>()
    {
        return _options.EnableLogging ? _loggerFactory?.CreateLogger<T>() : null;
    }

    /// <summary>
    /// Disposes the client and its resources.
    /// </summary>
    public void Dispose()
    {
        Dispose(true);
        GC.SuppressFinalize(this);
    }

    /// <summary>
    /// Disposes the client and its resources.
    /// </summary>
    protected virtual void Dispose(bool disposing)
    {
        if (_disposed)
            return;

        if (disposing)
        {
            // Dispose SignalR client
            SignalR?.Dispose();

            // Dispose HTTP client if we own it
            if (_disposeHttpClient)
            {
                _httpClient?.Dispose();
            }
        }

        _disposed = true;
    }
}

// Placeholder implementations for other clients (simplified for space)
namespace QueueManagement.SDK.Services.Implementation
{
    internal class QueuesClient : BaseApiClient, IQueuesClient
    {
        public QueuesClient(HttpClient httpClient, QueueManagementOptions options, ILogger<QueuesClient>? logger = null)
            : base(httpClient, options, logger) { }

        public Task<Models.Queues.QueueResponse> CreateAsync(Models.Queues.CreateQueueRequest request, CancellationToken cancellationToken = default)
            => PostAsync<Models.Queues.CreateQueueRequest, Models.Queues.QueueResponse>("api/queues", request, cancellationToken);

        public Task<Models.Queues.QueueResponse> GetAsync(Guid queueId, CancellationToken cancellationToken = default)
            => GetAsync<Models.Queues.QueueResponse>($"api/queues/{queueId}", cancellationToken);

        public Task<Models.Queues.QueueResponse> UpdateAsync(Guid queueId, Models.Queues.UpdateQueueRequest request, CancellationToken cancellationToken = default)
            => PutAsync<Models.Queues.UpdateQueueRequest, Models.Queues.QueueResponse>($"api/queues/{queueId}", request, cancellationToken);

        public Task DeleteAsync(Guid queueId, CancellationToken cancellationToken = default)
            => DeleteAsync($"api/queues/{queueId}", cancellationToken);

        public Task<Models.Queues.QueueStatusResponse> GetStatusAsync(Guid queueId, CancellationToken cancellationToken = default)
            => GetAsync<Models.Queues.QueueStatusResponse>($"api/queues/{queueId}/status", cancellationToken);

        public Task<Models.Queues.QueueResponse> UpdateStatusAsync(Guid queueId, Models.Common.QueueStatus status, CancellationToken cancellationToken = default)
            => PostAsync<object, Models.Queues.QueueResponse>($"api/queues/{queueId}/status", new { status }, cancellationToken);

        public Task<Models.Queues.QueueMetricsResponse> GetMetricsAsync(Guid queueId, CancellationToken cancellationToken = default)
            => GetAsync<Models.Queues.QueueMetricsResponse>($"api/queues/{queueId}/metrics", cancellationToken);

        public Task<List<Models.Queues.QueueResponse>> GetByUnitAsync(Guid unitId, CancellationToken cancellationToken = default)
            => GetAsync<List<Models.Queues.QueueResponse>>($"api/queues/unit/{unitId}", cancellationToken);

        public Task<List<Models.Queues.QueueResponse>> GetAllAsync(CancellationToken cancellationToken = default)
            => GetAsync<List<Models.Queues.QueueResponse>>("api/queues", cancellationToken);

        public Task<PagedResult<Models.Queues.QueueResponse>> GetPagedAsync(PagedRequest request, CancellationToken cancellationToken = default)
            => throw new NotImplementedException();

        public Task<Models.Queues.QueueResponse> OpenAsync(Guid queueId, CancellationToken cancellationToken = default)
            => PostAsync<object, Models.Queues.QueueResponse>($"api/queues/{queueId}/open", null, cancellationToken);

        public Task<Models.Queues.QueueResponse> CloseAsync(Guid queueId, CancellationToken cancellationToken = default)
            => PostAsync<object, Models.Queues.QueueResponse>($"api/queues/{queueId}/close", null, cancellationToken);

        public Task<Models.Queues.QueueResponse> PauseAsync(Guid queueId, CancellationToken cancellationToken = default)
            => PostAsync<object, Models.Queues.QueueResponse>($"api/queues/{queueId}/pause", null, cancellationToken);

        public Task<Models.Queues.QueueResponse> ResumeAsync(Guid queueId, CancellationToken cancellationToken = default)
            => PostAsync<object, Models.Queues.QueueResponse>($"api/queues/{queueId}/resume", null, cancellationToken);

        public Task ClearAsync(Guid queueId, CancellationToken cancellationToken = default)
            => PostAsync<object, object>($"api/queues/{queueId}/clear", null, cancellationToken);

        public async Task<TimeSpan> GetEstimatedWaitTimeAsync(Guid queueId, CancellationToken cancellationToken = default)
        {
            var response = await GetAsync<dynamic>($"api/queues/{queueId}/wait-time", cancellationToken);
            return TimeSpan.FromMinutes((double)response);
        }
    }

    internal class SessionsClient : BaseApiClient, ISessionsClient
    {
        public SessionsClient(HttpClient httpClient, QueueManagementOptions options, ILogger<SessionsClient>? logger = null)
            : base(httpClient, options, logger) { }

        public Task<Models.Sessions.SessionResponse> StartAsync(Models.Sessions.StartSessionRequest request, CancellationToken cancellationToken = default)
            => PostAsync<Models.Sessions.StartSessionRequest, Models.Sessions.SessionResponse>("api/sessions", request, cancellationToken);

        public Task<Models.Sessions.SessionResponse> GetAsync(Guid sessionId, CancellationToken cancellationToken = default)
            => GetAsync<Models.Sessions.SessionResponse>($"api/sessions/{sessionId}", cancellationToken);

        // Implement other methods similarly...
        public Task<Models.Sessions.SessionResponse> CompleteAsync(Guid sessionId, Models.Sessions.CompleteSessionRequest? request = null, CancellationToken cancellationToken = default)
            => throw new NotImplementedException();

        public Task<Models.Sessions.SessionResponse> PauseAsync(Guid sessionId, Models.Sessions.PauseSessionRequest? request = null, CancellationToken cancellationToken = default)
            => throw new NotImplementedException();

        public Task<Models.Sessions.SessionResponse> ResumeAsync(Guid sessionId, CancellationToken cancellationToken = default)
            => throw new NotImplementedException();

        public Task<Models.Sessions.SessionResponse?> GetActiveByUserAsync(Guid userId, CancellationToken cancellationToken = default)
            => throw new NotImplementedException();

        public Task<List<Models.Sessions.SessionResponse>> GetActiveSessionsAsync(Guid? unitId = null, CancellationToken cancellationToken = default)
            => throw new NotImplementedException();

        public Task<Models.Sessions.SessionStatistics> GetStatisticsAsync(Guid sessionId, CancellationToken cancellationToken = default)
            => throw new NotImplementedException();

        public Task<List<Models.Sessions.SessionResponse>> GetByUnitAsync(Guid unitId, Models.Common.SessionStatus? status = null, CancellationToken cancellationToken = default)
            => throw new NotImplementedException();

        public Task<List<Models.Sessions.SessionResponse>> GetByUserAsync(Guid userId, DateTime? fromDate = null, DateTime? toDate = null, CancellationToken cancellationToken = default)
            => throw new NotImplementedException();

        public Task<PagedResult<Models.Sessions.SessionResponse>> GetPagedAsync(PagedRequest request, CancellationToken cancellationToken = default)
            => throw new NotImplementedException();

        public Task<Models.Tickets.TicketResponse?> CallNextTicketAsync(Guid sessionId, CancellationToken cancellationToken = default)
            => throw new NotImplementedException();

        public Task<Models.Tickets.TicketResponse> CompleteCurrentTicketAsync(Guid sessionId, CancellationToken cancellationToken = default)
            => throw new NotImplementedException();

        public Task<Models.Tickets.TicketResponse> TransferCurrentTicketAsync(Guid sessionId, Models.Tickets.TransferTicketRequest request, CancellationToken cancellationToken = default)
            => throw new NotImplementedException();

        public Task<Models.Sessions.SessionResponse> UpdateQueuesAsync(Guid sessionId, List<Guid> queueIds, CancellationToken cancellationToken = default)
            => throw new NotImplementedException();

        public Task<Models.Sessions.SessionResponse> UpdateServicesAsync(Guid sessionId, List<Guid> serviceIds, CancellationToken cancellationToken = default)
            => throw new NotImplementedException();
    }

    // Similar placeholder implementations for other clients
    internal class UnitsClient : BaseApiClient, IUnitsClient
    {
        public UnitsClient(HttpClient httpClient, QueueManagementOptions options, ILogger<UnitsClient>? logger = null)
            : base(httpClient, options, logger) { }

        // Implement interface methods...
        public Task<Models.Units.UnitResponse> CreateAsync(Models.Units.CreateUnitRequest request, CancellationToken cancellationToken = default)
            => throw new NotImplementedException();

        public Task<Models.Units.UnitResponse> GetAsync(Guid unitId, CancellationToken cancellationToken = default)
            => throw new NotImplementedException();

        public Task<Models.Units.UnitResponse> UpdateAsync(Guid unitId, Models.Units.UpdateUnitRequest request, CancellationToken cancellationToken = default)
            => throw new NotImplementedException();

        public Task DeleteAsync(Guid unitId, CancellationToken cancellationToken = default)
            => throw new NotImplementedException();

        public Task<List<Models.Units.UnitResponse>> GetAllAsync(CancellationToken cancellationToken = default)
            => throw new NotImplementedException();

        public Task<PagedResult<Models.Units.UnitResponse>> GetPagedAsync(PagedRequest request, CancellationToken cancellationToken = default)
            => throw new NotImplementedException();

        public Task<Models.Units.UnitResponse> OpenAsync(Guid unitId, CancellationToken cancellationToken = default)
            => throw new NotImplementedException();

        public Task<Models.Units.UnitResponse> CloseAsync(Guid unitId, CancellationToken cancellationToken = default)
            => throw new NotImplementedException();

        public Task<List<Models.Units.UnitResponse>> GetByCityAsync(string city, CancellationToken cancellationToken = default)
            => throw new NotImplementedException();

        public Task<List<Models.Units.UnitResponse>> GetByStateAsync(string state, CancellationToken cancellationToken = default)
            => throw new NotImplementedException();

        public Task<bool> IsOpenAsync(Guid unitId, CancellationToken cancellationToken = default)
            => throw new NotImplementedException();

        public Task<Dictionary<string, Models.Units.WorkingHours>> GetWorkingHoursAsync(Guid unitId, CancellationToken cancellationToken = default)
            => throw new NotImplementedException();

        public Task<Models.Units.UnitResponse> UpdateWorkingHoursAsync(Guid unitId, Dictionary<string, Models.Units.WorkingHours> workingHours, CancellationToken cancellationToken = default)
            => throw new NotImplementedException();
    }

    internal class ServicesClient : BaseApiClient, IServicesClient
    {
        public ServicesClient(HttpClient httpClient, QueueManagementOptions options, ILogger<ServicesClient>? logger = null)
            : base(httpClient, options, logger) { }

        // Implement interface methods...
        public Task<Models.Services.ServiceResponse> CreateAsync(Models.Services.CreateServiceRequest request, CancellationToken cancellationToken = default)
            => throw new NotImplementedException();

        public Task<Models.Services.ServiceResponse> GetAsync(Guid serviceId, CancellationToken cancellationToken = default)
            => throw new NotImplementedException();

        public Task<Models.Services.ServiceResponse> UpdateAsync(Guid serviceId, Models.Services.UpdateServiceRequest request, CancellationToken cancellationToken = default)
            => throw new NotImplementedException();

        public Task DeleteAsync(Guid serviceId, CancellationToken cancellationToken = default)
            => throw new NotImplementedException();

        public Task<List<Models.Services.ServiceResponse>> GetAllAsync(CancellationToken cancellationToken = default)
            => throw new NotImplementedException();

        public Task<List<Models.Services.ServiceResponse>> GetActiveAsync(CancellationToken cancellationToken = default)
            => throw new NotImplementedException();

        public Task<List<Models.Services.ServiceResponse>> GetByTypeAsync(Models.Common.ServiceType type, CancellationToken cancellationToken = default)
            => throw new NotImplementedException();

        public Task<PagedResult<Models.Services.ServiceResponse>> GetPagedAsync(PagedRequest request, CancellationToken cancellationToken = default)
            => throw new NotImplementedException();

        public Task<Models.Services.ServiceResponse> ActivateAsync(Guid serviceId, CancellationToken cancellationToken = default)
            => throw new NotImplementedException();

        public Task<Models.Services.ServiceResponse> DeactivateAsync(Guid serviceId, CancellationToken cancellationToken = default)
            => throw new NotImplementedException();

        public Task<List<Models.Services.ServiceResponse>> GetByTagAsync(string tag, CancellationToken cancellationToken = default)
            => throw new NotImplementedException();

        public Task<List<Models.Services.ServiceResponse>> SearchAsync(string query, CancellationToken cancellationToken = default)
            => throw new NotImplementedException();
    }

    internal class UsersClient : BaseApiClient, IUsersClient
    {
        public UsersClient(HttpClient httpClient, QueueManagementOptions options, ILogger<UsersClient>? logger = null)
            : base(httpClient, options, logger) { }

        // Implement interface methods...
        public Task<Models.Users.UserResponse> CreateAsync(Models.Users.CreateUserRequest request, CancellationToken cancellationToken = default)
            => throw new NotImplementedException();

        public Task<Models.Users.UserResponse> GetAsync(Guid userId, CancellationToken cancellationToken = default)
            => throw new NotImplementedException();

        public Task<Models.Users.UserResponse> UpdateAsync(Guid userId, Models.Users.UpdateUserRequest request, CancellationToken cancellationToken = default)
            => throw new NotImplementedException();

        public Task DeleteAsync(Guid userId, CancellationToken cancellationToken = default)
            => throw new NotImplementedException();

        public Task<List<Models.Users.UserResponse>> GetAllAsync(CancellationToken cancellationToken = default)
            => throw new NotImplementedException();

        public Task<List<Models.Users.UserResponse>> GetByRoleAsync(Models.Common.UserRole role, CancellationToken cancellationToken = default)
            => throw new NotImplementedException();

        public Task<List<Models.Users.UserResponse>> GetByUnitAsync(Guid unitId, CancellationToken cancellationToken = default)
            => throw new NotImplementedException();

        public Task<PagedResult<Models.Users.UserResponse>> GetPagedAsync(PagedRequest request, CancellationToken cancellationToken = default)
            => throw new NotImplementedException();

        public Task<Models.Users.UserResponse> GetCurrentAsync(CancellationToken cancellationToken = default)
            => throw new NotImplementedException();

        public Task ChangePasswordAsync(Guid userId, Models.Users.ChangePasswordRequest request, CancellationToken cancellationToken = default)
            => throw new NotImplementedException();

        public Task<Models.Users.UserResponse> ActivateAsync(Guid userId, CancellationToken cancellationToken = default)
            => throw new NotImplementedException();

        public Task<Models.Users.UserResponse> DeactivateAsync(Guid userId, CancellationToken cancellationToken = default)
            => throw new NotImplementedException();

        public Task<List<Models.Users.UserResponse>> GetOnlineAsync(CancellationToken cancellationToken = default)
            => throw new NotImplementedException();

        public Task<List<Models.Users.UserResponse>> SearchAsync(string query, CancellationToken cancellationToken = default)
            => throw new NotImplementedException();

        public Task<Models.Users.UserResponse> AssignToUnitAsync(Guid userId, Guid unitId, CancellationToken cancellationToken = default)
            => throw new NotImplementedException();

        public Task<Models.Users.UserResponse> UpdateQueuesAsync(Guid userId, List<Guid> queueIds, CancellationToken cancellationToken = default)
            => throw new NotImplementedException();

        public Task<Models.Users.UserResponse> UpdateServicesAsync(Guid userId, List<Guid> serviceIds, CancellationToken cancellationToken = default)
            => throw new NotImplementedException();
    }

    internal class DashboardClient : BaseApiClient, IDashboardClient
    {
        public DashboardClient(HttpClient httpClient, QueueManagementOptions options, ILogger<DashboardClient>? logger = null)
            : base(httpClient, options, logger) { }

        // Implement interface methods...
        public Task<Models.Dashboard.DashboardMetricsResponse> GetMetricsAsync(Models.Dashboard.DashboardMetricsRequest? request = null, CancellationToken cancellationToken = default)
            => throw new NotImplementedException();

        public Task<Models.Dashboard.RealTimeStatistics> GetRealTimeStatisticsAsync(Guid? unitId = null, CancellationToken cancellationToken = default)
            => throw new NotImplementedException();

        public Task<Models.Dashboard.UnitMetrics> GetUnitMetricsAsync(Guid unitId, CancellationToken cancellationToken = default)
            => throw new NotImplementedException();

        public Task<Models.Dashboard.QueueMetrics> GetQueueMetricsAsync(Guid queueId, CancellationToken cancellationToken = default)
            => throw new NotImplementedException();

        public Task<Models.Dashboard.AgentPerformance> GetAgentPerformanceAsync(Guid agentId, DateTime? date = null, CancellationToken cancellationToken = default)
            => throw new NotImplementedException();

        public Task<List<Models.Dashboard.AgentPerformance>> GetTopAgentsAsync(int count = 10, DateTime? date = null, CancellationToken cancellationToken = default)
            => throw new NotImplementedException();

        public Task<List<Models.Dashboard.Alert>> GetAlertsAsync(string? severity = null, CancellationToken cancellationToken = default)
            => throw new NotImplementedException();

        public Task<Models.Dashboard.Alert> AcknowledgeAlertAsync(Guid alertId, CancellationToken cancellationToken = default)
            => throw new NotImplementedException();

        public Task<Models.Dashboard.DashboardMetricsResponse> GetHistoricalMetricsAsync(DateTime fromDate, DateTime toDate, Guid? unitId = null, CancellationToken cancellationToken = default)
            => throw new NotImplementedException();

        public Task<Stream> ExportMetricsAsync(DateTime fromDate, DateTime toDate, string format = "csv", CancellationToken cancellationToken = default)
            => throw new NotImplementedException();

        public Task<Dictionary<string, double>> GetServiceLevelMetricsAsync(Guid? unitId = null, CancellationToken cancellationToken = default)
            => throw new NotImplementedException();

        public Task<Dictionary<string, double>> GetCustomerSatisfactionMetricsAsync(Guid? unitId = null, DateTime? date = null, CancellationToken cancellationToken = default)
            => throw new NotImplementedException();
    }

    internal class WebhooksClient : BaseApiClient, IWebhooksClient
    {
        public WebhooksClient(HttpClient httpClient, QueueManagementOptions options, ILogger<WebhooksClient>? logger = null)
            : base(httpClient, options, logger) { }

        // Implement interface methods...
        public Task<Models.Webhooks.WebhookResponse> CreateAsync(Models.Webhooks.CreateWebhookRequest request, CancellationToken cancellationToken = default)
            => throw new NotImplementedException();

        public Task<Models.Webhooks.WebhookResponse> GetAsync(Guid webhookId, CancellationToken cancellationToken = default)
            => throw new NotImplementedException();

        public Task<Models.Webhooks.WebhookResponse> UpdateAsync(Guid webhookId, Models.Webhooks.UpdateWebhookRequest request, CancellationToken cancellationToken = default)
            => throw new NotImplementedException();

        public Task DeleteAsync(Guid webhookId, CancellationToken cancellationToken = default)
            => throw new NotImplementedException();

        public Task<List<Models.Webhooks.WebhookResponse>> GetAllAsync(CancellationToken cancellationToken = default)
            => throw new NotImplementedException();

        public Task<PagedResult<Models.Webhooks.WebhookResponse>> GetPagedAsync(PagedRequest request, CancellationToken cancellationToken = default)
            => throw new NotImplementedException();

        public Task<Models.Webhooks.WebhookTestResponse> TestAsync(Guid webhookId, CancellationToken cancellationToken = default)
            => throw new NotImplementedException();

        public Task<Models.Webhooks.WebhookResponse> RegenerateSecretAsync(Guid webhookId, CancellationToken cancellationToken = default)
            => throw new NotImplementedException();

        public Task<Models.Webhooks.WebhookResponse> ActivateAsync(Guid webhookId, CancellationToken cancellationToken = default)
            => throw new NotImplementedException();

        public Task<Models.Webhooks.WebhookResponse> DeactivateAsync(Guid webhookId, CancellationToken cancellationToken = default)
            => throw new NotImplementedException();

        public Task<List<Models.Webhooks.WebhookDeliveryAttempt>> GetDeliveryAttemptsAsync(Guid webhookId, int? limit = null, CancellationToken cancellationToken = default)
            => throw new NotImplementedException();

        public Task<Models.Webhooks.WebhookDeliveryAttempt> RetryDeliveryAsync(Guid webhookId, Guid attemptId, CancellationToken cancellationToken = default)
            => throw new NotImplementedException();

        public Task<List<Models.Webhooks.WebhookResponse>> GetByEventTypeAsync(Models.Common.WebhookEventType eventType, CancellationToken cancellationToken = default)
            => throw new NotImplementedException();

        public bool ValidateSignature(string payload, string signature, string secret)
        {
            using var hmac = new System.Security.Cryptography.HMACSHA256(System.Text.Encoding.UTF8.GetBytes(secret));
            var hash = hmac.ComputeHash(System.Text.Encoding.UTF8.GetBytes(payload));
            var expectedSignature = Convert.ToBase64String(hash);
            return signature.Equals(expectedSignature, StringComparison.OrdinalIgnoreCase);
        }

        public Models.Webhooks.WebhookEvent<T> ParseEvent<T>(string payload) where T : class
        {
            return System.Text.Json.JsonSerializer.Deserialize<Models.Webhooks.WebhookEvent<T>>(payload, new System.Text.Json.JsonSerializerOptions
            {
                PropertyNamingPolicy = System.Text.Json.JsonNamingPolicy.CamelCase
            }) ?? throw new InvalidOperationException("Failed to parse webhook payload");
        }
    }
}