using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using QueueManagement.Api.DTOs.Webhooks;
using QueueManagement.Api.DTOs.Common;
using MediatR;
using Swashbuckle.AspNetCore.Annotations;

namespace QueueManagement.Api.Controllers;

/// <summary>
/// Controller for managing webhooks
/// </summary>
[SwaggerTag("Webhook management endpoints for creating, reading, updating, and testing webhooks")]
public class WebhooksController : BaseController
{
    public WebhooksController(IMediator mediator, ILogger<WebhooksController> logger) : base(mediator, logger) { }

    /// <summary>
    /// Get all webhooks
    /// </summary>
    /// <returns>List of webhooks</returns>
    [HttpGet]
    [ProducesResponseType(typeof(ApiResponse<List<WebhookDto>>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status400BadRequest)]
    [SwaggerOperation(
        Summary = "Get all webhooks",
        Description = "Retrieves all webhooks for the tenant",
        OperationId = "GetWebhooks"
    )]
    public async Task<ActionResult<ApiResponse<List<WebhookDto>>>> GetWebhooks()
    {
        try
        {
            var tenantId = GetTenantId();
            _logger.LogInformation("Getting webhooks for tenant: {TenantId}", tenantId);

            // TODO: Implement get webhooks query with MediatR
            // var query = new GetWebhooksQuery { TenantId = tenantId };
            // var result = await _mediator.Send(query);

            // For now, return mock data
            var mockWebhooks = new List<WebhookDto>
            {
                new WebhookDto
                {
                    Id = Guid.NewGuid(),
                    Name = "Ticket Created",
                    Url = "https://example.com/webhooks/ticket-created",
                    Events = new List<string> { "ticket.created", "ticket.updated" },
                    IsActive = true,
                    RetryCount = 3,
                    LastTriggeredAt = DateTime.UtcNow.AddMinutes(-30),
                    CreatedAt = DateTime.UtcNow.AddDays(-7),
                    UpdatedAt = DateTime.UtcNow.AddMinutes(-30)
                },
                new WebhookDto
                {
                    Id = Guid.NewGuid(),
                    Name = "Queue Status Change",
                    Url = "https://example.com/webhooks/queue-status",
                    Events = new List<string> { "queue.status_changed", "queue.ticket_called" },
                    IsActive = true,
                    RetryCount = 3,
                    LastTriggeredAt = DateTime.UtcNow.AddHours(-2),
                    CreatedAt = DateTime.UtcNow.AddDays(-14),
                    UpdatedAt = DateTime.UtcNow.AddHours(-2)
                }
            };

            return Success(mockWebhooks);
        }
        catch (Exception ex)
        {
            return LogAndReturnError<List<WebhookDto>>(ex, "Failed to get webhooks");
        }
    }

    /// <summary>
    /// Get a webhook by ID
    /// </summary>
    /// <param name="id">Webhook ID</param>
    /// <returns>Webhook details</returns>
    [HttpGet("{id:guid}")]
    [ProducesResponseType(typeof(ApiResponse<WebhookDto>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status404NotFound)]
    [SwaggerOperation(
        Summary = "Get webhook by ID",
        Description = "Retrieves a specific webhook by its ID",
        OperationId = "GetWebhook"
    )]
    public async Task<ActionResult<ApiResponse<WebhookDto>>> GetWebhook(Guid id)
    {
        try
        {
            var tenantId = GetTenantId();
            _logger.LogInformation("Getting webhook {WebhookId} for tenant: {TenantId}", id, tenantId);

            // TODO: Implement get webhook query with MediatR
            // var query = new GetWebhookByIdQuery { WebhookId = id, TenantId = tenantId };
            // var result = await _mediator.Send(query);

            // For now, return mock data
            var mockWebhook = new WebhookDto
            {
                Id = id,
                Name = "Ticket Created",
                Url = "https://example.com/webhooks/ticket-created",
                Events = new List<string> { "ticket.created", "ticket.updated" },
                IsActive = true,
                RetryCount = 3,
                LastTriggeredAt = DateTime.UtcNow.AddMinutes(-30),
                CreatedAt = DateTime.UtcNow.AddDays(-7),
                UpdatedAt = DateTime.UtcNow.AddMinutes(-30)
            };

            return Success(mockWebhook);
        }
        catch (Exception ex)
        {
            return LogAndReturnError<WebhookDto>(ex, "Failed to get webhook");
        }
    }

    /// <summary>
    /// Create a new webhook
    /// </summary>
    /// <param name="dto">Webhook creation data</param>
    /// <returns>Created webhook</returns>
    [HttpPost]
    [ProducesResponseType(typeof(ApiResponse<WebhookDto>), StatusCodes.Status201Created)]
    [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status400BadRequest)]
    [SwaggerOperation(
        Summary = "Create webhook",
        Description = "Creates a new webhook",
        OperationId = "CreateWebhook"
    )]
    public async Task<ActionResult<ApiResponse<WebhookDto>>> CreateWebhook([FromBody] CreateWebhookDto dto)
    {
        try
        {
            var tenantId = GetTenantId();
            var userId = GetUserId();
            _logger.LogInformation("Creating webhook for tenant: {TenantId} by user: {UserId}", tenantId, userId);

            // TODO: Implement create webhook command with MediatR
            // var command = new CreateWebhookCommand 
            // { 
            //     TenantId = tenantId,
            //     Name = dto.Name,
            //     Url = dto.Url,
            //     Events = dto.Events,
            //     RetryCount = dto.RetryCount,
            //     CreatedBy = userId
            // };
            // var result = await _mediator.Send(command);

            // For now, return mock data
            var mockWebhook = new WebhookDto
            {
                Id = Guid.NewGuid(),
                Name = dto.Name,
                Url = dto.Url,
                Events = dto.Events,
                IsActive = true,
                RetryCount = dto.RetryCount,
                LastTriggeredAt = null,
                CreatedAt = DateTime.UtcNow,
                UpdatedAt = DateTime.UtcNow
            };

            _logger.LogInformation("Webhook created successfully: {WebhookId}", mockWebhook.Id);
            return Created(mockWebhook, nameof(GetWebhook), new { id = mockWebhook.Id });
        }
        catch (Exception ex)
        {
            return LogAndReturnError<WebhookDto>(ex, "Failed to create webhook");
        }
    }

    /// <summary>
    /// Update a webhook
    /// </summary>
    /// <param name="id">Webhook ID</param>
    /// <param name="dto">Webhook update data</param>
    /// <returns>Updated webhook</returns>
    [HttpPut("{id:guid}")]
    [ProducesResponseType(typeof(ApiResponse<WebhookDto>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status404NotFound)]
    [SwaggerOperation(
        Summary = "Update webhook",
        Description = "Updates an existing webhook",
        OperationId = "UpdateWebhook"
    )]
    public async Task<ActionResult<ApiResponse<WebhookDto>>> UpdateWebhook(Guid id, [FromBody] UpdateWebhookDto dto)
    {
        try
        {
            var tenantId = GetTenantId();
            var userId = GetUserId();
            _logger.LogInformation("Updating webhook {WebhookId} for tenant: {TenantId} by user: {UserId}", id, tenantId, userId);

            // TODO: Implement update webhook command with MediatR
            // var command = new UpdateWebhookCommand 
            // { 
            //     WebhookId = id,
            //     TenantId = tenantId,
            //     Name = dto.Name,
            //     Url = dto.Url,
            //     Events = dto.Events,
            //     IsActive = dto.IsActive,
            //     RetryCount = dto.RetryCount,
            //     UpdatedBy = userId
            // };
            // var result = await _mediator.Send(command);

            // For now, return mock data
            var mockWebhook = new WebhookDto
            {
                Id = id,
                Name = dto.Name ?? "Ticket Created",
                Url = dto.Url ?? "https://example.com/webhooks/ticket-created",
                Events = dto.Events ?? new List<string> { "ticket.created" },
                IsActive = dto.IsActive ?? true,
                RetryCount = dto.RetryCount ?? 3,
                LastTriggeredAt = DateTime.UtcNow.AddMinutes(-30),
                CreatedAt = DateTime.UtcNow.AddDays(-7),
                UpdatedAt = DateTime.UtcNow
            };

            _logger.LogInformation("Webhook updated successfully: {WebhookId}", id);
            return Success(mockWebhook);
        }
        catch (Exception ex)
        {
            return LogAndReturnError<WebhookDto>(ex, "Failed to update webhook");
        }
    }

    /// <summary>
    /// Delete a webhook
    /// </summary>
    /// <param name="id">Webhook ID</param>
    /// <returns>Success response</returns>
    [HttpDelete("{id:guid}")]
    [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status404NotFound)]
    [SwaggerOperation(
        Summary = "Delete webhook",
        Description = "Deletes a webhook",
        OperationId = "DeleteWebhook"
    )]
    public async Task<ActionResult<ApiResponse<object>>> DeleteWebhook(Guid id)
    {
        try
        {
            var tenantId = GetTenantId();
            var userId = GetUserId();
            _logger.LogInformation("Deleting webhook {WebhookId} for tenant: {TenantId} by user: {UserId}", id, tenantId, userId);

            // TODO: Implement delete webhook command with MediatR
            // var command = new DeleteWebhookCommand { WebhookId = id, TenantId = tenantId, DeletedBy = userId };
            // await _mediator.Send(command);

            _logger.LogInformation("Webhook deleted successfully: {WebhookId}", id);
            return Success<object>(null, "Webhook deleted successfully");
        }
        catch (Exception ex)
        {
            return LogAndReturnError<object>(ex, "Failed to delete webhook");
        }
    }

    /// <summary>
    /// Test a webhook
    /// </summary>
    /// <param name="id">Webhook ID</param>
    /// <param name="dto">Test data</param>
    /// <returns>Test results</returns>
    [HttpPost("{id:guid}/test")]
    [ProducesResponseType(typeof(ApiResponse<WebhookTestResponseDto>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status404NotFound)]
    [SwaggerOperation(
        Summary = "Test webhook",
        Description = "Tests a webhook by sending a test payload",
        OperationId = "TestWebhook"
    )]
    public async Task<ActionResult<ApiResponse<WebhookTestResponseDto>>> TestWebhook(Guid id, [FromBody] TestWebhookDto dto)
    {
        try
        {
            var tenantId = GetTenantId();
            var userId = GetUserId();
            _logger.LogInformation("Testing webhook {WebhookId} for tenant: {TenantId} by user: {UserId}", id, tenantId, userId);

            // TODO: Implement test webhook command with MediatR
            // var command = new TestWebhookCommand 
            // { 
            //     WebhookId = id,
            //     TenantId = tenantId,
            //     EventType = dto.EventType,
            //     TestPayload = dto.TestPayload,
            //     TestedBy = userId
            // };
            // var result = await _mediator.Send(command);

            // For now, return mock data
            var mockTestResponse = new WebhookTestResponseDto
            {
                Success = true,
                StatusCode = 200,
                Message = "Webhook test successful",
                ResponseTimeMs = 245,
                ResponseHeaders = new Dictionary<string, string>
                {
                    { "Content-Type", "application/json" },
                    { "X-Webhook-Id", id.ToString() }
                },
                ResponseBody = "{\"status\":\"ok\",\"message\":\"Webhook received successfully\"}"
            };

            _logger.LogInformation("Webhook test completed successfully: {WebhookId}", id);
            return Success(mockTestResponse);
        }
        catch (Exception ex)
        {
            return LogAndReturnError<WebhookTestResponseDto>(ex, "Failed to test webhook");
        }
    }

    /// <summary>
    /// Get webhook delivery history
    /// </summary>
    /// <param name="id">Webhook ID</param>
    /// <returns>Webhook delivery history</returns>
    [HttpGet("{id:guid}/deliveries")]
    [ProducesResponseType(typeof(ApiResponse<List<object>>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status404NotFound)]
    [SwaggerOperation(
        Summary = "Get webhook delivery history",
        Description = "Retrieves the delivery history for a specific webhook",
        OperationId = "GetWebhookDeliveries"
    )]
    public async Task<ActionResult<ApiResponse<List<object>>>> GetWebhookDeliveries(Guid id)
    {
        try
        {
            var tenantId = GetTenantId();
            _logger.LogInformation("Getting delivery history for webhook {WebhookId} in tenant: {TenantId}", id, tenantId);

            // TODO: Implement get webhook deliveries query with MediatR
            // var query = new GetWebhookDeliveriesQuery { WebhookId = id, TenantId = tenantId };
            // var result = await _mediator.Send(query);

            // For now, return mock data
            var mockDeliveries = new List<object>
            {
                new
                {
                    Id = Guid.NewGuid(),
                    Timestamp = DateTime.UtcNow.AddMinutes(-30),
                    EventType = "ticket.created",
                    Status = "Delivered",
                    StatusCode = 200,
                    ResponseTimeMs = 245,
                    RetryCount = 0
                },
                new
                {
                    Id = Guid.NewGuid(),
                    Timestamp = DateTime.UtcNow.AddHours(-2),
                    EventType = "queue.status_changed",
                    Status = "Delivered",
                    StatusCode = 200,
                    ResponseTimeMs = 189,
                    RetryCount = 0
                }
            };

            return Success(mockDeliveries);
        }
        catch (Exception ex)
        {
            return LogAndReturnError<List<object>>(ex, "Failed to get webhook deliveries");
        }
    }

    /// <summary>
    /// Retry failed webhook delivery
    /// </summary>
    /// <param name="id">Webhook ID</param>
    /// <param name="deliveryId">Delivery ID</param>
    /// <returns>Success response</returns>
    [HttpPost("{id:guid}/deliveries/{deliveryId:guid}/retry")]
    [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status404NotFound)]
    [SwaggerOperation(
        Summary = "Retry webhook delivery",
        Description = "Retries a failed webhook delivery",
        OperationId = "RetryWebhookDelivery"
    )]
    public async Task<ActionResult<ApiResponse<object>>> RetryWebhookDelivery(Guid id, Guid deliveryId)
    {
        try
        {
            var tenantId = GetTenantId();
            var userId = GetUserId();
            _logger.LogInformation("Retrying webhook delivery {DeliveryId} for webhook {WebhookId} in tenant: {TenantId}", 
                deliveryId, id, tenantId);

            // TODO: Implement retry webhook delivery command with MediatR
            // var command = new RetryWebhookDeliveryCommand 
            // { 
            //     WebhookId = id,
            //     DeliveryId = deliveryId,
            //     TenantId = tenantId,
            //     RetriedBy = userId
            // };
            // await _mediator.Send(command);

            _logger.LogInformation("Webhook delivery retry initiated: {DeliveryId}", deliveryId);
            return Success<object>(null, "Webhook delivery retry initiated");
        }
        catch (Exception ex)
        {
            return LogAndReturnError<object>(ex, "Failed to retry webhook delivery");
        }
    }

    /// <summary>
    /// Get webhook event types
    /// </summary>
    /// <returns>Available webhook event types</returns>
    [HttpGet("events")]
    [ProducesResponseType(typeof(ApiResponse<List<string>>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status400BadRequest)]
    [SwaggerOperation(
        Summary = "Get webhook event types",
        Description = "Retrieves all available webhook event types",
        OperationId = "GetWebhookEventTypes"
    )]
    public async Task<ActionResult<ApiResponse<List<string>>>> GetWebhookEventTypes()
    {
        try
        {
            var tenantId = GetTenantId();
            _logger.LogInformation("Getting webhook event types for tenant: {TenantId}", tenantId);

            // TODO: Implement get webhook event types query with MediatR
            // var query = new GetWebhookEventTypesQuery { TenantId = tenantId };
            // var result = await _mediator.Send(query);

            // For now, return mock data
            var mockEventTypes = new List<string>
            {
                "ticket.created",
                "ticket.updated",
                "ticket.called",
                "ticket.completed",
                "ticket.cancelled",
                "queue.status_changed",
                "queue.ticket_called",
                "session.started",
                "session.completed",
                "session.paused",
                "session.resumed",
                "user.status_changed",
                "unit.status_changed"
            };

            return Success(mockEventTypes);
        }
        catch (Exception ex)
        {
            return LogAndReturnError<List<string>>(ex, "Failed to get webhook event types");
        }
    }
}