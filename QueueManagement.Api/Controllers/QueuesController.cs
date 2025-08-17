using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using QueueManagement.Api.DTOs.Queues;
using QueueManagement.Api.DTOs.Common;
using MediatR;
using Swashbuckle.AspNetCore.Annotations;

namespace QueueManagement.Api.Controllers;

/// <summary>
/// Controller for managing queues
/// </summary>
[SwaggerTag("Queue management endpoints for creating, reading, updating, and deleting queues")]
public class QueuesController : BaseController
{
    public QueuesController(IMediator mediator, ILogger<QueuesController> logger) : base(mediator, logger) { }

    /// <summary>
    /// Get all queues with pagination and filtering
    /// </summary>
    /// <param name="filter">Filter parameters</param>
    /// <param name="pagination">Pagination parameters</param>
    /// <returns>Paginated list of queues</returns>
    [HttpGet]
    [ProducesResponseType(typeof(ApiResponse<List<QueueDto>>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status400BadRequest)]
    [SwaggerOperation(
        Summary = "Get all queues",
        Description = "Retrieves a paginated list of queues with optional filtering",
        OperationId = "GetQueues"
    )]
    public async Task<ActionResult<ApiResponse<List<QueueDto>>>> GetQueues(
        [FromQuery] QueueFilterDto filter,
        [FromQuery] PaginationRequestDto pagination)
    {
        try
        {
            var tenantId = GetTenantId();
            _logger.LogInformation("Getting queues for tenant: {TenantId}", tenantId);

            // TODO: Implement get queues query with MediatR
            // var query = new GetQueuesQuery { TenantId = tenantId, Filter = filter, Pagination = pagination };
            // var result = await _mediator.Send(query);

            // For now, return mock data
            var mockQueues = new List<QueueDto>
            {
                new QueueDto
                {
                    Id = Guid.NewGuid(),
                    Name = "Customer Service",
                    Code = "CS001",
                    DisplayName = "Customer Service",
                    MaxCapacity = 100,
                    Status = "Open",
                    IsActive = true,
                    UnitId = Guid.NewGuid(),
                    UnitName = "Main Office",
                    CurrentTicketCount = 15,
                    IsAtCapacity = false,
                    IsAcceptingTickets = true,
                    CreatedAt = DateTime.UtcNow.AddDays(-30),
                    UpdatedAt = DateTime.UtcNow
                },
                new QueueDto
                {
                    Id = Guid.NewGuid(),
                    Name = "Technical Support",
                    Code = "TS001",
                    DisplayName = "Technical Support",
                    MaxCapacity = 50,
                    Status = "Open",
                    IsActive = true,
                    UnitId = Guid.NewGuid(),
                    UnitName = "Main Office",
                    CurrentTicketCount = 8,
                    IsAtCapacity = false,
                    IsAcceptingTickets = true,
                    CreatedAt = DateTime.UtcNow.AddDays(-30),
                    UpdatedAt = DateTime.UtcNow
                }
            };

            var meta = new ApiMeta
            {
                Pagination = new PaginationMeta
                {
                    Page = pagination.Page,
                    PageSize = pagination.PageSize,
                    TotalItems = mockQueues.Count,
                    TotalPages = 1,
                    HasNext = false,
                    HasPrevious = false
                }
            };

            return Success(mockQueues, meta);
        }
        catch (Exception ex)
        {
            return LogAndReturnError<List<QueueDto>>(ex, "Failed to get queues");
        }
    }

    /// <summary>
    /// Get a specific queue by ID
    /// </summary>
    /// <param name="id">Queue ID</param>
    /// <returns>Queue details</returns>
    [HttpGet("{id}")]
    [ProducesResponseType(typeof(ApiResponse<QueueDto>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status404NotFound)]
    [SwaggerOperation(
        Summary = "Get queue by ID",
        Description = "Retrieves detailed information about a specific queue",
        OperationId = "GetQueue"
    )]
    public async Task<ActionResult<ApiResponse<QueueDto>>> GetQueue(Guid id)
    {
        try
        {
            var tenantId = GetTenantId();
            _logger.LogInformation("Getting queue {QueueId} for tenant: {TenantId}", id, tenantId);

            // TODO: Implement get queue query with MediatR
            // var query = new GetQueueByIdQuery { Id = id, TenantId = tenantId };
            // var result = await _mediator.Send(query);

            // For now, return mock data
            var mockQueue = new QueueDto
            {
                Id = id,
                Name = "Customer Service",
                Code = "CS001",
                DisplayName = "Customer Service",
                MaxCapacity = 100,
                Status = "Open",
                IsActive = true,
                UnitId = Guid.NewGuid(),
                UnitName = "Main Office",
                CurrentTicketCount = 15,
                IsAtCapacity = false,
                IsAcceptingTickets = true,
                CreatedAt = DateTime.UtcNow.AddDays(-30),
                UpdatedAt = DateTime.UtcNow
            };

            return Success(mockQueue);
        }
        catch (Exception ex)
        {
            return LogAndReturnError<QueueDto>(ex, "Failed to get queue");
        }
    }

    /// <summary>
    /// Create a new queue
    /// </summary>
    /// <param name="dto">Queue creation data</param>
    /// <returns>Created queue</returns>
    [HttpPost]
    [ProducesResponseType(typeof(ApiResponse<QueueDto>), StatusCodes.Status201Created)]
    [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status400BadRequest)]
    [SwaggerOperation(
        Summary = "Create queue",
        Description = "Creates a new queue for a unit",
        OperationId = "CreateQueue"
    )]
    public async Task<ActionResult<ApiResponse<QueueDto>>> CreateQueue([FromBody] CreateQueueDto dto)
    {
        try
        {
            var tenantId = GetTenantId();
            var userId = GetUserId();
            _logger.LogInformation("Creating queue for tenant: {TenantId} by user: {UserId}", tenantId, userId);

            // TODO: Implement create queue command with MediatR
            // var command = new CreateQueueCommand 
            // { 
            //     TenantId = tenantId,
            //     Name = dto.Name,
            //     Code = dto.Code,
            //     DisplayName = dto.DisplayName,
            //     MaxCapacity = dto.MaxCapacity,
            //     UnitId = dto.UnitId,
            //     CreatedBy = userId
            // };
            // var result = await _mediator.Send(command);

            // For now, return mock data
            var mockQueue = new QueueDto
            {
                Id = Guid.NewGuid(),
                Name = dto.Name,
                Code = dto.Code,
                DisplayName = dto.DisplayName,
                MaxCapacity = dto.MaxCapacity,
                Status = "Open",
                IsActive = true,
                UnitId = dto.UnitId,
                UnitName = "Main Office",
                CurrentTicketCount = 0,
                IsAtCapacity = false,
                IsAcceptingTickets = true,
                CreatedAt = DateTime.UtcNow,
                UpdatedAt = DateTime.UtcNow
            };

            _logger.LogInformation("Queue created successfully: {QueueId}", mockQueue.Id);
            return Created(mockQueue, nameof(GetQueue), new { id = mockQueue.Id });
        }
        catch (Exception ex)
        {
            return LogAndReturnError<QueueDto>(ex, "Failed to create queue");
        }
    }

    /// <summary>
    /// Update an existing queue
    /// </summary>
    /// <param name="id">Queue ID</param>
    /// <param name="dto">Queue update data</param>
    /// <returns>Updated queue</returns>
    [HttpPut("{id}")]
    [ProducesResponseType(typeof(ApiResponse<QueueDto>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status404NotFound)]
    [SwaggerOperation(
        Summary = "Update queue",
        Description = "Updates an existing queue",
        OperationId = "UpdateQueue"
    )]
    public async Task<ActionResult<ApiResponse<QueueDto>>> UpdateQueue(Guid id, [FromBody] UpdateQueueDto dto)
    {
        try
        {
            var tenantId = GetTenantId();
            var userId = GetUserId();
            _logger.LogInformation("Updating queue {QueueId} for tenant: {TenantId} by user: {UserId}", id, tenantId, userId);

            // TODO: Implement update queue command with MediatR
            // var command = new UpdateQueueCommand 
            // { 
            //     Id = id,
            //     TenantId = tenantId,
            //     Name = dto.Name,
            //     Code = dto.Code,
            //     DisplayName = dto.DisplayName,
            //     MaxCapacity = dto.MaxCapacity,
            //     IsActive = dto.IsActive,
            //     UpdatedBy = userId
            // };
            // var result = await _mediator.Send(command);

            // For now, return mock data
            var mockQueue = new QueueDto
            {
                Id = id,
                Name = dto.Name ?? "Customer Service",
                Code = dto.Code ?? "CS001",
                DisplayName = dto.DisplayName ?? "Customer Service",
                MaxCapacity = dto.MaxCapacity ?? 100,
                Status = "Open",
                IsActive = dto.IsActive ?? true,
                UnitId = Guid.NewGuid(),
                UnitName = "Main Office",
                CurrentTicketCount = 15,
                IsAtCapacity = false,
                IsAcceptingTickets = true,
                CreatedAt = DateTime.UtcNow.AddDays(-30),
                UpdatedAt = DateTime.UtcNow
            };

            _logger.LogInformation("Queue updated successfully: {QueueId}", id);
            return Success(mockQueue);
        }
        catch (Exception ex)
        {
            return LogAndReturnError<QueueDto>(ex, "Failed to update queue");
        }
    }

    /// <summary>
    /// Delete a queue
    /// </summary>
    /// <param name="id">Queue ID</param>
    /// <returns>No content</returns>
    [HttpDelete("{id}")]
    [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status204NoContent)]
    [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status404NotFound)]
    [SwaggerOperation(
        Summary = "Delete queue",
        Description = "Soft deletes a queue",
        OperationId = "DeleteQueue"
    )]
    public async Task<ActionResult<ApiResponse<object>>> DeleteQueue(Guid id)
    {
        try
        {
            var tenantId = GetTenantId();
            var userId = GetUserId();
            _logger.LogInformation("Deleting queue {QueueId} for tenant: {TenantId} by user: {UserId}", id, tenantId, userId);

            // TODO: Implement delete queue command with MediatR
            // var command = new DeleteQueueCommand { Id = id, TenantId = tenantId, DeletedBy = userId };
            // await _mediator.Send(command);

            _logger.LogInformation("Queue deleted successfully: {QueueId}", id);
            return NoContent();
        }
        catch (Exception ex)
        {
            return LogAndReturnError<object>(ex, "Failed to delete queue");
        }
    }

    /// <summary>
    /// Get real-time queue status
    /// </summary>
    /// <param name="id">Queue ID</param>
    /// <returns>Current queue status</returns>
    [HttpGet("{id}/status")]
    [ProducesResponseType(typeof(ApiResponse<QueueStatusDto>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status404NotFound)]
    [SwaggerOperation(
        Summary = "Get queue status",
        Description = "Retrieves real-time status information for a specific queue",
        OperationId = "GetQueueStatus"
    )]
    public async Task<ActionResult<ApiResponse<QueueStatusDto>>> GetQueueStatus(Guid id)
    {
        try
        {
            var tenantId = GetTenantId();
            _logger.LogInformation("Getting status for queue {QueueId} in tenant: {TenantId}", id, tenantId);

            // TODO: Implement get queue status query with MediatR
            // var query = new GetQueueStatusQuery { Id = id, TenantId = tenantId };
            // var result = await _mediator.Send(query);

            // For now, return mock data
            var mockStatus = new QueueStatusDto
            {
                Id = id,
                Name = "Customer Service",
                Status = "Open",
                IsActive = true,
                CurrentTicketCount = 15,
                MaxCapacity = 100,
                IsAtCapacity = false,
                IsAcceptingTickets = true,
                LastUpdated = DateTime.UtcNow
            };

            return Success(mockStatus);
        }
        catch (Exception ex)
        {
            return LogAndReturnError<QueueStatusDto>(ex, "Failed to get queue status");
        }
    }

    /// <summary>
    /// Update queue status (open/close)
    /// </summary>
    /// <param name="id">Queue ID</param>
    /// <param name="dto">Status update data</param>
    /// <returns>Updated queue status</returns>
    [HttpPut("{id}/status")]
    [ProducesResponseType(typeof(ApiResponse<QueueStatusDto>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status404NotFound)]
    [SwaggerOperation(
        Summary = "Update queue status",
        Description = "Updates the status of a queue (open/close)",
        OperationId = "UpdateQueueStatus"
    )]
    public async Task<ActionResult<ApiResponse<QueueStatusDto>>> UpdateQueueStatus(Guid id, [FromBody] UpdateQueueStatusDto dto)
    {
        try
        {
            var tenantId = GetTenantId();
            var userId = GetUserId();
            _logger.LogInformation("Updating status for queue {QueueId} in tenant: {TenantId} by user: {UserId}", id, tenantId, userId);

            // TODO: Implement update queue status command with MediatR
            // var command = new UpdateQueueStatusCommand 
            // { 
            //     Id = id,
            //     TenantId = tenantId,
            //     Status = dto.Status,
            //     IsActive = dto.IsActive,
            //     UpdatedBy = userId
            // };
            // var result = await _mediator.Send(command);

            // For now, return mock data
            var mockStatus = new QueueStatusDto
            {
                Id = id,
                Name = "Customer Service",
                Status = dto.Status,
                IsActive = dto.IsActive ?? true,
                CurrentTicketCount = 15,
                MaxCapacity = 100,
                IsAtCapacity = false,
                IsAcceptingTickets = dto.Status == "Open" && (dto.IsActive ?? true),
                LastUpdated = DateTime.UtcNow
            };

            _logger.LogInformation("Queue status updated successfully: {QueueId}", id);
            return Success(mockStatus);
        }
        catch (Exception ex)
        {
            return LogAndReturnError<QueueStatusDto>(ex, "Failed to update queue status");
        }
    }

    /// <summary>
    /// Get tickets in a specific queue
    /// </summary>
    /// <param name="id">Queue ID</param>
    /// <param name="pagination">Pagination parameters</param>
    /// <returns>List of tickets in the queue</returns>
    [HttpGet("{id}/tickets")]
    [ProducesResponseType(typeof(ApiResponse<List<object>>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status404NotFound)]
    [SwaggerOperation(
        Summary = "Get queue tickets",
        Description = "Retrieves all tickets in a specific queue",
        OperationId = "GetQueueTickets"
    )]
    public async Task<ActionResult<ApiResponse<List<object>>>> GetQueueTickets(Guid id, [FromQuery] PaginationRequestDto pagination)
    {
        try
        {
            var tenantId = GetTenantId();
            _logger.LogInformation("Getting tickets for queue {QueueId} in tenant: {TenantId}", id, tenantId);

            // TODO: Implement get queue tickets query with MediatR
            // var query = new GetQueueTicketsQuery { QueueId = id, TenantId = tenantId, Pagination = pagination };
            // var result = await _mediator.Send(query);

            // For now, return mock data
            var mockTickets = new List<object>
            {
                new { Id = Guid.NewGuid(), Number = "A001", Status = "Waiting", Priority = "Normal" },
                new { Id = Guid.NewGuid(), Number = "A002", Status = "Called", Priority = "High" },
                new { Id = Guid.NewGuid(), Number = "A003", Status = "InProgress", Priority = "Normal" }
            };

            var meta = new ApiMeta
            {
                Pagination = new PaginationMeta
                {
                    Page = pagination.Page,
                    PageSize = pagination.PageSize,
                    TotalItems = mockTickets.Count,
                    TotalPages = 1,
                    HasNext = false,
                    HasPrevious = false
                }
            };

            return Success(mockTickets, meta);
        }
        catch (Exception ex)
        {
            return LogAndReturnError<List<object>>(ex, "Failed to get queue tickets");
        }
    }
}