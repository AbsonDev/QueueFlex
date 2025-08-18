using Microsoft.Extensions.Logging;
using QueueManagement.SDK.Common;
using QueueManagement.SDK.Configuration;
using QueueManagement.SDK.Models.Common;
using QueueManagement.SDK.Models.Tickets;

namespace QueueManagement.SDK.Services.Implementation;

/// <summary>
/// Implementation of the tickets client.
/// </summary>
internal class TicketsClient : BaseApiClient, ITicketsClient
{
    private const string BasePath = "api/tickets";

    public TicketsClient(HttpClient httpClient, QueueManagementOptions options, ILogger<TicketsClient>? logger = null)
        : base(httpClient, options, logger)
    {
    }

    public async Task<TicketResponse> CreateAsync(CreateTicketRequest request, CancellationToken cancellationToken = default)
    {
        return await PostAsync<CreateTicketRequest, TicketResponse>(BasePath, request, cancellationToken).ConfigureAwait(false);
    }

    public async Task<TicketResponse> GetAsync(Guid ticketId, CancellationToken cancellationToken = default)
    {
        return await GetAsync<TicketResponse>($"{BasePath}/{ticketId}", cancellationToken).ConfigureAwait(false);
    }

    public async Task<TicketResponse> UpdateAsync(Guid ticketId, UpdateTicketRequest request, CancellationToken cancellationToken = default)
    {
        return await PutAsync<UpdateTicketRequest, TicketResponse>($"{BasePath}/{ticketId}", request, cancellationToken).ConfigureAwait(false);
    }

    public async Task DeleteAsync(Guid ticketId, CancellationToken cancellationToken = default)
    {
        await DeleteAsync($"{BasePath}/{ticketId}", cancellationToken).ConfigureAwait(false);
    }

    public async Task<TicketResponse> CallAsync(Guid ticketId, CancellationToken cancellationToken = default)
    {
        return await PostAsync<object, TicketResponse>($"{BasePath}/{ticketId}/call", null, cancellationToken).ConfigureAwait(false);
    }

    public async Task<TicketResponse> StartServingAsync(Guid ticketId, CancellationToken cancellationToken = default)
    {
        return await PostAsync<object, TicketResponse>($"{BasePath}/{ticketId}/serve", null, cancellationToken).ConfigureAwait(false);
    }

    public async Task<TicketResponse> CompleteAsync(Guid ticketId, CancellationToken cancellationToken = default)
    {
        return await PostAsync<object, TicketResponse>($"{BasePath}/{ticketId}/complete", null, cancellationToken).ConfigureAwait(false);
    }

    public async Task<TicketResponse> CancelAsync(Guid ticketId, string? reason = null, CancellationToken cancellationToken = default)
    {
        var request = new { reason };
        return await PostAsync<object, TicketResponse>($"{BasePath}/{ticketId}/cancel", request, cancellationToken).ConfigureAwait(false);
    }

    public async Task<TicketResponse> MarkNoShowAsync(Guid ticketId, CancellationToken cancellationToken = default)
    {
        return await PostAsync<object, TicketResponse>($"{BasePath}/{ticketId}/no-show", null, cancellationToken).ConfigureAwait(false);
    }

    public async Task<TicketResponse> TransferAsync(Guid ticketId, TransferTicketRequest request, CancellationToken cancellationToken = default)
    {
        return await PostAsync<TransferTicketRequest, TicketResponse>($"{BasePath}/{ticketId}/transfer", request, cancellationToken).ConfigureAwait(false);
    }

    public async Task<TicketStatusResponse> GetStatusAsync(Guid ticketId, CancellationToken cancellationToken = default)
    {
        return await GetAsync<TicketStatusResponse>($"{BasePath}/{ticketId}/status", cancellationToken).ConfigureAwait(false);
    }

    public async Task<QueuePositionResponse> GetPositionAsync(Guid ticketId, CancellationToken cancellationToken = default)
    {
        return await GetAsync<QueuePositionResponse>($"{BasePath}/{ticketId}/position", cancellationToken).ConfigureAwait(false);
    }

    public async Task<List<TicketResponse>> GetByQueueAsync(Guid queueId, TicketStatus? status = null, CancellationToken cancellationToken = default)
    {
        var query = status.HasValue ? $"?status={status.Value}" : "";
        return await GetAsync<List<TicketResponse>>($"{BasePath}/queue/{queueId}{query}", cancellationToken).ConfigureAwait(false);
    }

    public async Task<List<TicketResponse>> GetByCustomerAsync(string customerDocument, CancellationToken cancellationToken = default)
    {
        return await GetAsync<List<TicketResponse>>($"{BasePath}/customer/{Uri.EscapeDataString(customerDocument)}", cancellationToken).ConfigureAwait(false);
    }

    public async Task<List<TicketResponse>> GetByServiceAsync(Guid serviceId, TicketStatus? status = null, CancellationToken cancellationToken = default)
    {
        var query = status.HasValue ? $"?status={status.Value}" : "";
        return await GetAsync<List<TicketResponse>>($"{BasePath}/service/{serviceId}{query}", cancellationToken).ConfigureAwait(false);
    }

    public async Task<PagedResult<TicketResponse>> GetPagedAsync(PagedRequest request, CancellationToken cancellationToken = default)
    {
        var queryParams = new Dictionary<string, string?>
        {
            ["pageNumber"] = request.PageNumber.ToString(),
            ["pageSize"] = request.PageSize.ToString(),
            ["sortBy"] = request.SortBy,
            ["sortDescending"] = request.SortDescending.ToString()
        };

        var query = BuildQueryString(queryParams);
        return await GetAsync<PagedResult<TicketResponse>>($"{BasePath}{query}", cancellationToken).ConfigureAwait(false);
    }

    public IAsyncEnumerable<TicketResponse> StreamQueueTicketsAsync(Guid queueId, CancellationToken cancellationToken = default)
    {
        return StreamAsync<TicketResponse>($"{BasePath}/stream/queue/{queueId}", cancellationToken);
    }

    public async Task<BatchResponse<TicketResponse>> CreateBatchAsync(BatchRequest<CreateTicketRequest> request, CancellationToken cancellationToken = default)
    {
        return await PostAsync<BatchRequest<CreateTicketRequest>, BatchResponse<TicketResponse>>($"{BasePath}/batch", request, cancellationToken).ConfigureAwait(false);
    }

    public async Task<TicketResponse> RecallAsync(Guid ticketId, CancellationToken cancellationToken = default)
    {
        return await PostAsync<object, TicketResponse>($"{BasePath}/{ticketId}/recall", null, cancellationToken).ConfigureAwait(false);
    }

    public async Task<TicketResponse?> GetNextAsync(Guid queueId, CancellationToken cancellationToken = default)
    {
        return await GetAsync<TicketResponse?>($"{BasePath}/queue/{queueId}/next", cancellationToken).ConfigureAwait(false);
    }
}