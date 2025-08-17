using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using QueueManagement.Api.DTOs.Services;
using QueueManagement.Api.DTOs.Common;
using MediatR;
using Swashbuckle.AspNetCore.Annotations;

namespace QueueManagement.Api.Controllers;

/// <summary>
/// Controller for managing services
/// </summary>
[SwaggerTag("Service management endpoints for creating, reading, updating, and deleting services")]
public class ServicesController : BaseController
{
    public ServicesController(IMediator mediator, ILogger<ServicesController> logger) : base(mediator, logger) { }

    /// <summary>
    /// Get all services with pagination and filtering
    /// </summary>
    /// <param name="filter">Filter parameters</param>
    /// <param name="pagination">Pagination parameters</param>
    /// <returns>Paginated list of services</returns>
    [HttpGet]
    [ProducesResponseType(typeof(ApiResponse<List<ServiceDto>>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status400BadRequest)]
    [SwaggerOperation(
        Summary = "Get all services",
        Description = "Retrieves a paginated list of services with optional filtering",
        OperationId = "GetServices"
    )]
    public async Task<ActionResult<ApiResponse<List<ServiceDto>>>> GetServices(
        [FromQuery] ServiceFilterDto filter,
        [FromQuery] PaginationRequestDto pagination)
    {
        try
        {
            var tenantId = GetTenantId();
            _logger.LogInformation("Getting services for tenant: {TenantId}", tenantId);

            // TODO: Implement get services query with MediatR
            // var query = new GetServicesQuery { TenantId = tenantId, Filter = filter, Pagination = pagination };
            // var result = await _mediator.Send(query);

            // For now, return mock data
            var mockServices = new List<ServiceDto>
            {
                new ServiceDto
                {
                    Id = Guid.NewGuid(),
                    Name = "Customer Service",
                    Code = "CS001",
                    Description = "General customer service and support",
                    EstimatedDurationMinutes = 15,
                    Color = "#007bff",
                    RequiresResource = false,
                    IsActive = true,
                    QueueCount = 3,
                    ActiveTicketCount = 12,
                    CreatedAt = DateTime.UtcNow.AddDays(-30),
                    UpdatedAt = DateTime.UtcNow
                },
                new ServiceDto
                {
                    Id = Guid.NewGuid(),
                    Name = "Technical Support",
                    Code = "TS001",
                    Description = "Technical support and troubleshooting",
                    EstimatedDurationMinutes = 30,
                    Color = "#28a745",
                    RequiresResource = true,
                    IsActive = true,
                    QueueCount = 2,
                    ActiveTicketCount = 8,
                    CreatedAt = DateTime.UtcNow.AddDays(-30),
                    UpdatedAt = DateTime.UtcNow
                },
                new ServiceDto
                {
                    Id = Guid.NewGuid(),
                    Name = "Document Processing",
                    Code = "DP001",
                    Description = "Document review and processing",
                    EstimatedDurationMinutes = 45,
                    Color = "#ffc107",
                    RequiresResource = false,
                    IsActive = true,
                    QueueCount = 1,
                    ActiveTicketCount = 5,
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
                    TotalItems = mockServices.Count,
                    TotalPages = 1,
                    HasNext = false,
                    HasPrevious = false
                }
            };

            return Success(mockServices, meta);
        }
        catch (Exception ex)
        {
            return LogAndReturnError<List<ServiceDto>>(ex, "Failed to get services");
        }
    }

    /// <summary>
    /// Get a service by ID
    /// </summary>
    /// <param name="id">Service ID</param>
    /// <returns>Service details</returns>
    [HttpGet("{id:guid}")]
    [ProducesResponseType(typeof(ApiResponse<ServiceDto>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status404NotFound)]
    [SwaggerOperation(
        Summary = "Get service by ID",
        Description = "Retrieves a specific service by its ID",
        OperationId = "GetService"
    )]
    public async Task<ActionResult<ApiResponse<ServiceDto>>> GetService(Guid id)
    {
        try
        {
            var tenantId = GetTenantId();
            _logger.LogInformation("Getting service {ServiceId} for tenant: {TenantId}", id, tenantId);

            // TODO: Implement get service query with MediatR
            // var query = new GetServiceByIdQuery { ServiceId = id, TenantId = tenantId };
            // var result = await _mediator.Send(query);

            // For now, return mock data
            var mockService = new ServiceDto
            {
                Id = id,
                Name = "Customer Service",
                Code = "CS001",
                Description = "General customer service and support",
                EstimatedDurationMinutes = 15,
                Color = "#007bff",
                RequiresResource = false,
                IsActive = true,
                QueueCount = 3,
                ActiveTicketCount = 12,
                CreatedAt = DateTime.UtcNow.AddDays(-30),
                UpdatedAt = DateTime.UtcNow
            };

            return Success(mockService);
        }
        catch (Exception ex)
        {
            return LogAndReturnError<ServiceDto>(ex, "Failed to get service");
        }
    }

    /// <summary>
    /// Create a new service
    /// </summary>
    /// <param name="dto">Service creation data</param>
    /// <returns>Created service</returns>
    [HttpPost]
    [ProducesResponseType(typeof(ApiResponse<ServiceDto>), StatusCodes.Status201Created)]
    [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status400BadRequest)]
    [SwaggerOperation(
        Summary = "Create service",
        Description = "Creates a new service",
        OperationId = "CreateService"
    )]
    public async Task<ActionResult<ApiResponse<ServiceDto>>> CreateService([FromBody] CreateServiceDto dto)
    {
        try
        {
            var tenantId = GetTenantId();
            var userId = GetUserId();
            _logger.LogInformation("Creating service for tenant: {TenantId} by user: {UserId}", tenantId, userId);

            // TODO: Implement create service command with MediatR
            // var command = new CreateServiceCommand 
            // { 
            //     TenantId = tenantId,
            //     Name = dto.Name,
            //     Code = dto.Code,
            //     Description = dto.Description,
            //     EstimatedDurationMinutes = dto.EstimatedDurationMinutes,
            //     Color = dto.Color,
            //     RequiresResource = dto.RequiresResource,
            //     CreatedBy = userId
            // };
            // var result = await _mediator.Send(command);

            // For now, return mock data
            var mockService = new ServiceDto
            {
                Id = Guid.NewGuid(),
                Name = dto.Name,
                Code = dto.Code,
                Description = dto.Description,
                EstimatedDurationMinutes = dto.EstimatedDurationMinutes,
                Color = dto.Color,
                RequiresResource = dto.RequiresResource,
                IsActive = true,
                QueueCount = 0,
                ActiveTicketCount = 0,
                CreatedAt = DateTime.UtcNow,
                UpdatedAt = DateTime.UtcNow
            };

            _logger.LogInformation("Service created successfully: {ServiceId}", mockService.Id);
            return Created(mockService, nameof(GetService), new { id = mockService.Id });
        }
        catch (Exception ex)
        {
            return LogAndReturnError<ServiceDto>(ex, "Failed to create service");
        }
    }

    /// <summary>
    /// Update a service
    /// </summary>
    /// <param name="id">Service ID</param>
    /// <param name="dto">Service update data</param>
    /// <returns>Updated service</returns>
    [HttpPut("{id:guid}")]
    [ProducesResponseType(typeof(ApiResponse<ServiceDto>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status404NotFound)]
    [SwaggerOperation(
        Summary = "Update service",
        Description = "Updates an existing service",
        OperationId = "UpdateService"
    )]
    public async Task<ActionResult<ApiResponse<ServiceDto>>> UpdateService(Guid id, [FromBody] UpdateServiceDto dto)
    {
        try
        {
            var tenantId = GetTenantId();
            var userId = GetUserId();
            _logger.LogInformation("Updating service {ServiceId} for tenant: {TenantId} by user: {UserId}", id, tenantId, userId);

            // TODO: Implement update service command with MediatR
            // var command = new UpdateServiceCommand 
            // { 
            //     ServiceId = id,
            //     TenantId = tenantId,
            //     Name = dto.Name,
            //     Code = dto.Code,
            //     Description = dto.Description,
            //     EstimatedDurationMinutes = dto.EstimatedDurationMinutes,
            //     Color = dto.Color,
            //     RequiresResource = dto.RequiresResource,
            //     IsActive = dto.IsActive,
            //     UpdatedBy = userId
            // };
            // var result = await _mediator.Send(command);

            // For now, return mock data
            var mockService = new ServiceDto
            {
                Id = id,
                Name = dto.Name ?? "Customer Service",
                Code = dto.Code ?? "CS001",
                Description = dto.Description,
                EstimatedDurationMinutes = dto.EstimatedDurationMinutes ?? 15,
                Color = dto.Color ?? "#007bff",
                RequiresResource = dto.RequiresResource ?? false,
                IsActive = dto.IsActive ?? true,
                QueueCount = 3,
                ActiveTicketCount = 12,
                CreatedAt = DateTime.UtcNow.AddDays(-30),
                UpdatedAt = DateTime.UtcNow
            };

            _logger.LogInformation("Service updated successfully: {ServiceId}", id);
            return Success(mockService);
        }
        catch (Exception ex)
        {
            return LogAndReturnError<ServiceDto>(ex, "Failed to update service");
        }
    }

    /// <summary>
    /// Delete a service (soft delete)
    /// </summary>
    /// <param name="id">Service ID</param>
    /// <returns>Success response</returns>
    [HttpDelete("{id:guid}")]
    [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status404NotFound)]
    [SwaggerOperation(
        Summary = "Delete service",
        Description = "Soft deletes a service",
        OperationId = "DeleteService"
    )]
    public async Task<ActionResult<ApiResponse<object>>> DeleteService(Guid id)
    {
        try
        {
            var tenantId = GetTenantId();
            var userId = GetUserId();
            _logger.LogInformation("Deleting service {ServiceId} for tenant: {TenantId} by user: {UserId}", id, tenantId, userId);

            // TODO: Implement delete service command with MediatR
            // var command = new DeleteServiceCommand { ServiceId = id, TenantId = tenantId, DeletedBy = userId };
            // await _mediator.Send(command);

            _logger.LogInformation("Service deleted successfully: {ServiceId}", id);
            return Success<object>(null, "Service deleted successfully");
        }
        catch (Exception ex)
        {
            return LogAndReturnError<object>(ex, "Failed to delete service");
        }
    }

    /// <summary>
    /// Get services by queue ID
    /// </summary>
    /// <param name="queueId">Queue ID</param>
    /// <returns>List of services offered by the queue</returns>
    [HttpGet("queue/{queueId:guid}")]
    [ProducesResponseType(typeof(ApiResponse<List<ServiceDto>>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status400BadRequest)]
    [SwaggerOperation(
        Summary = "Get services by queue",
        Description = "Retrieves all services offered by a specific queue",
        OperationId = "GetServicesByQueue"
    )]
    public async Task<ActionResult<ApiResponse<List<ServiceDto>>>> GetServicesByQueue(Guid queueId)
    {
        try
        {
            var tenantId = GetTenantId();
            _logger.LogInformation("Getting services for queue {QueueId} in tenant: {TenantId}", queueId, tenantId);

            // TODO: Implement get services by queue query with MediatR
            // var query = new GetServicesByQueueQuery { QueueId = queueId, TenantId = tenantId };
            // var result = await _mediator.Send(query);

            // For now, return mock data
            var mockServices = new List<ServiceDto>
            {
                new ServiceDto
                {
                    Id = Guid.NewGuid(),
                    Name = "Customer Service",
                    Code = "CS001",
                    Description = "General customer service and support",
                    EstimatedDurationMinutes = 15,
                    Color = "#007bff",
                    RequiresResource = false,
                    IsActive = true,
                    QueueCount = 1,
                    ActiveTicketCount = 5,
                    CreatedAt = DateTime.UtcNow.AddDays(-30),
                    UpdatedAt = DateTime.UtcNow
                }
            };

            return Success(mockServices);
        }
        catch (Exception ex)
        {
            return LogAndReturnError<List<ServiceDto>>(ex, "Failed to get services by queue");
        }
    }

    /// <summary>
    /// Get active services
    /// </summary>
    /// <returns>List of active services</returns>
    [HttpGet("active")]
    [ProducesResponseType(typeof(ApiResponse<List<ServiceDto>>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status400BadRequest)]
    [SwaggerOperation(
        Summary = "Get active services",
        Description = "Retrieves all active services",
        OperationId = "GetActiveServices"
    )]
    public async Task<ActionResult<ApiResponse<List<ServiceDto>>>> GetActiveServices()
    {
        try
        {
            var tenantId = GetTenantId();
            _logger.LogInformation("Getting active services for tenant: {TenantId}", tenantId);

            // TODO: Implement get active services query with MediatR
            // var query = new GetActiveServicesQuery { TenantId = tenantId };
            // var result = await _mediator.Send(query);

            // For now, return mock data
            var mockServices = new List<ServiceDto>
            {
                new ServiceDto
                {
                    Id = Guid.NewGuid(),
                    Name = "Customer Service",
                    Code = "CS001",
                    Description = "General customer service and support",
                    EstimatedDurationMinutes = 15,
                    Color = "#007bff",
                    RequiresResource = false,
                    IsActive = true,
                    QueueCount = 3,
                    ActiveTicketCount = 12,
                    CreatedAt = DateTime.UtcNow.AddDays(-30),
                    UpdatedAt = DateTime.UtcNow
                },
                new ServiceDto
                {
                    Id = Guid.NewGuid(),
                    Name = "Technical Support",
                    Code = "TS001",
                    Description = "Technical support and troubleshooting",
                    EstimatedDurationMinutes = 30,
                    Color = "#28a745",
                    RequiresResource = true,
                    IsActive = true,
                    QueueCount = 2,
                    ActiveTicketCount = 8,
                    CreatedAt = DateTime.UtcNow.AddDays(-30),
                    UpdatedAt = DateTime.UtcNow
                }
            };

            return Success(mockServices);
        }
        catch (Exception ex)
        {
            return LogAndReturnError<List<ServiceDto>>(ex, "Failed to get active services");
        }
    }
}