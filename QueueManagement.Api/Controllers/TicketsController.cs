using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using QueueManagement.Api.DTOs.Tickets;
using QueueManagement.Api.DTOs.Common;
using MediatR;
using Swashbuckle.AspNetCore.Annotations;

namespace QueueManagement.Api.Controllers;

/// <summary>
/// Controller for managing tickets
/// </summary>
[SwaggerTag("Ticket management endpoints for creating, reading, updating, and managing tickets")]
public class TicketsController : BaseController
{
    public TicketsController(IMediator mediator, ILogger<TicketsController> logger) : base(mediator, logger) { }

    /// <summary>
    /// Create a new ticket in the queue
    /// </summary>
    /// <param name="dto">Ticket creation data</param>
    /// <returns>Created ticket</returns>
    [HttpPost]
    [ProducesResponseType(typeof(ApiResponse<TicketDto>), StatusCodes.Status201Created)]
    [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status400BadRequest)]
    [SwaggerOperation(
        Summary = "Create ticket",
        Description = "Creates a new ticket in the specified queue",
        OperationId = "CreateTicket"
    )]
    public async Task<ActionResult<ApiResponse<TicketDto>>> CreateTicket([FromBody] CreateTicketDto dto)
    {
        try
        {
            var tenantId = GetTenantId();
            var userId = GetUserId();
            _logger.LogInformation("Creating ticket for tenant: {TenantId} by user: {UserId}", tenantId, userId);

            // TODO: Implement create ticket command with MediatR
            // var command = new CreateTicketCommand 
            // { 
            //     TenantId = tenantId,
            //     QueueId = dto.QueueId,
            //     ServiceId = dto.ServiceId,
            //     CustomerName = dto.CustomerName,
            //     CustomerDocument = dto.CustomerDocument,
            //     CustomerPhone = dto.CustomerPhone,
            //     Priority = dto.Priority,
            //     Notes = dto.Notes,
            //     CreatedBy = userId
            // };
            // var result = await _mediator.Send(command);

            // For now, return mock data
            var mockTicket = new TicketDto
            {
                Id = Guid.NewGuid(),
                Number = "A001",
                Status = "Waiting",
                Priority = dto.Priority,
                IssuedAt = DateTime.UtcNow,
                CustomerName = dto.CustomerName,
                CustomerDocument = dto.CustomerDocument,
                CustomerPhone = dto.CustomerPhone,
                Notes = dto.Notes,
                QueueId = dto.QueueId,
                QueueName = "Customer Service",
                ServiceId = dto.ServiceId,
                ServiceName = "General Service",
                UnitId = Guid.NewGuid(),
                UnitName = "Main Office",
                CreatedAt = DateTime.UtcNow,
                UpdatedAt = DateTime.UtcNow
            };

            _logger.LogInformation("Ticket created successfully: {TicketId}", mockTicket.Id);
            return Created(mockTicket, nameof(GetTicket), new { id = mockTicket.Id });
        }
        catch (Exception ex)
        {
            return LogAndReturnError<TicketDto>(ex, "Failed to create ticket");
        }
    }

    /// <summary>
    /// Get a specific ticket by ID
    /// </summary>
    /// <param name="id">Ticket ID</param>
    /// <returns>Ticket details</returns>
    [HttpGet("{id}")]
    [ProducesResponseType(typeof(ApiResponse<TicketDto>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status404NotFound)]
    [SwaggerOperation(
        Summary = "Get ticket by ID",
        Description = "Retrieves detailed information about a specific ticket",
        OperationId = "GetTicket"
    )]
    public async Task<ActionResult<ApiResponse<TicketDto>>> GetTicket(Guid id)
    {
        try
        {
            var tenantId = GetTenantId();
            _logger.LogInformation("Getting ticket {TicketId} for tenant: {TenantId}", id, tenantId);

            // TODO: Implement get ticket query with MediatR
            // var query = new GetTicketByIdQuery { Id = id, TenantId = tenantId };
            // var result = await _mediator.Send(query);

            // For now, return mock data
            var mockTicket = new TicketDto
            {
                Id = id,
                Number = "A001",
                Status = "Waiting",
                Priority = "Normal",
                IssuedAt = DateTime.UtcNow.AddMinutes(-30),
                CustomerName = "John Doe",
                CustomerDocument = "123.456.789-00",
                CustomerPhone = "+55 11 99999-9999",
                Notes = "Customer needs assistance",
                QueueId = Guid.NewGuid(),
                QueueName = "Customer Service",
                ServiceId = Guid.NewGuid(),
                ServiceName = "General Service",
                UnitId = Guid.NewGuid(),
                UnitName = "Main Office",
                WaitingTime = TimeSpan.FromMinutes(30),
                CreatedAt = DateTime.UtcNow.AddMinutes(-30),
                UpdatedAt = DateTime.UtcNow
            };

            return Success(mockTicket);
        }
        catch (Exception ex)
        {
            return LogAndReturnError<TicketDto>(ex, "Failed to get ticket");
        }
    }

    /// <summary>
    /// Update an existing ticket
    /// </summary>
    /// <param name="id">Ticket ID</param>
    /// <param name="dto">Ticket update data</param>
    /// <returns>Updated ticket</returns>
    [HttpPut("{id}")]
    [ProducesResponseType(typeof(ApiResponse<TicketDto>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status404NotFound)]
    [SwaggerOperation(
        Summary = "Update ticket",
        Description = "Updates an existing ticket",
        OperationId = "UpdateTicket"
    )]
    public async Task<ActionResult<ApiResponse<TicketDto>>> UpdateTicket(Guid id, [FromBody] UpdateTicketDto dto)
    {
        try
        {
            var tenantId = GetTenantId();
            var userId = GetUserId();
            _logger.LogInformation("Updating ticket {TicketId} for tenant: {TenantId} by user: {UserId}", id, tenantId, userId);

            // TODO: Implement update ticket command with MediatR
            // var command = new UpdateTicketCommand 
            // { 
            //     Id = id,
            //     TenantId = tenantId,
            //     CustomerName = dto.CustomerName,
            //     CustomerDocument = dto.CustomerDocument,
            //     CustomerPhone = dto.CustomerPhone,
            //     Priority = dto.Priority,
            //     Notes = dto.Notes,
            //     UpdatedBy = userId
            // };
            // var result = await _mediator.Send(command);

            // For now, return mock data
            var mockTicket = new TicketDto
            {
                Id = id,
                Number = "A001",
                Status = "Waiting",
                Priority = dto.Priority ?? "Normal",
                IssuedAt = DateTime.UtcNow.AddMinutes(-30),
                CustomerName = dto.CustomerName ?? "John Doe",
                CustomerDocument = dto.CustomerDocument ?? "123.456.789-00",
                CustomerPhone = dto.CustomerPhone ?? "+55 11 99999-9999",
                Notes = dto.Notes ?? "Customer needs assistance",
                QueueId = Guid.NewGuid(),
                QueueName = "Customer Service",
                ServiceId = Guid.NewGuid(),
                ServiceName = "General Service",
                UnitId = Guid.NewGuid(),
                UnitName = "Main Office",
                WaitingTime = TimeSpan.FromMinutes(30),
                CreatedAt = DateTime.UtcNow.AddMinutes(-30),
                UpdatedAt = DateTime.UtcNow
            };

            _logger.LogInformation("Ticket updated successfully: {TicketId}", id);
            return Success(mockTicket);
        }
        catch (Exception ex)
        {
            return LogAndReturnError<TicketDto>(ex, "Failed to update ticket");
        }
    }

    /// <summary>
    /// Cancel a ticket
    /// </summary>
    /// <param name="id">Ticket ID</param>
    /// <returns>No content</returns>
    [HttpDelete("{id}")]
    [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status204NoContent)]
    [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status404NotFound)]
    [SwaggerOperation(
        Summary = "Cancel ticket",
        Description = "Cancels a ticket (soft delete)",
        OperationId = "CancelTicket"
    )]
    public async Task<ActionResult<ApiResponse<object>>> CancelTicket(Guid id)
    {
        try
        {
            var tenantId = GetTenantId();
            var userId = GetUserId();
            _logger.LogInformation("Cancelling ticket {TicketId} for tenant: {TenantId} by user: {UserId}", id, tenantId, userId);

            // TODO: Implement cancel ticket command with MediatR
            // var command = new CancelTicketCommand { Id = id, TenantId = tenantId, CancelledBy = userId };
            // await _mediator.Send(command);

            _logger.LogInformation("Ticket cancelled successfully: {TicketId}", id);
            return NoContent();
        }
        catch (Exception ex)
        {
            return LogAndReturnError<object>(ex, "Failed to cancel ticket");
        }
    }

    /// <summary>
    /// Call a ticket for service
    /// </summary>
    /// <param name="id">Ticket ID</param>
    /// <param name="dto">Call ticket data</param>
    /// <returns>Updated ticket</returns>
    [HttpPost("{id}/call")]
    [ProducesResponseType(typeof(ApiResponse<TicketDto>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status404NotFound)]
    [SwaggerOperation(
        Summary = "Call ticket",
        Description = "Calls a ticket for service",
        OperationId = "CallTicket"
    )]
    public async Task<ActionResult<ApiResponse<TicketDto>>> CallTicket(Guid id, [FromBody] CallTicketDto dto)
    {
        try
        {
            var tenantId = GetTenantId();
            var userId = GetUserId();
            _logger.LogInformation("Calling ticket {TicketId} for tenant: {TenantId} by user: {UserId}", id, tenantId, userId);

            // TODO: Implement call ticket command with MediatR
            // var command = new CallTicketCommand 
            // { 
            //     Id = id,
            //     TenantId = tenantId,
            //     UserId = dto.UserId,
            //     ResourceId = dto.ResourceId,
            //     Notes = dto.Notes,
            //     CalledBy = userId
            // };
            // var result = await _mediator.Send(command);

            // For now, return mock data
            var mockTicket = new TicketDto
            {
                Id = id,
                Number = "A001",
                Status = "Called",
                Priority = "Normal",
                IssuedAt = DateTime.UtcNow.AddMinutes(-30),
                CalledAt = DateTime.UtcNow,
                CustomerName = "John Doe",
                CustomerDocument = "123.456.789-00",
                CustomerPhone = "+55 11 99999-9999",
                Notes = "Customer needs assistance",
                QueueId = Guid.NewGuid(),
                QueueName = "Customer Service",
                ServiceId = Guid.NewGuid(),
                ServiceName = "General Service",
                UnitId = Guid.NewGuid(),
                UnitName = "Main Office",
                WaitingTime = TimeSpan.FromMinutes(30),
                CreatedAt = DateTime.UtcNow.AddMinutes(-30),
                UpdatedAt = DateTime.UtcNow
            };

            _logger.LogInformation("Ticket called successfully: {TicketId}", id);
            return Success(mockTicket);
        }
        catch (Exception ex)
        {
            return LogAndReturnError<TicketDto>(ex, "Failed to call ticket");
        }
    }

    /// <summary>
    /// Transfer a ticket to another queue/service
    /// </summary>
    /// <param name="id">Ticket ID</param>
    /// <param name="dto">Transfer ticket data</param>
    /// <returns>Updated ticket</returns>
    [HttpPost("{id}/transfer")]
    [ProducesResponseType(typeof(ApiResponse<TicketDto>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status404NotFound)]
    [SwaggerOperation(
        Summary = "Transfer ticket",
        Description = "Transfers a ticket to another queue and service",
        OperationId = "TransferTicket"
    )]
    public async Task<ActionResult<ApiResponse<TicketDto>>> TransferTicket(Guid id, [FromBody] TransferTicketDto dto)
    {
        try
        {
            var tenantId = GetTenantId();
            var userId = GetUserId();
            _logger.LogInformation("Transferring ticket {TicketId} for tenant: {TenantId} by user: {UserId}", id, tenantId, userId);

            // TODO: Implement transfer ticket command with MediatR
            // var command = new TransferTicketCommand 
            // { 
            //     Id = id,
            //     TenantId = tenantId,
            //     NewQueueId = dto.NewQueueId,
            //     NewServiceId = dto.NewServiceId,
            //     Reason = dto.Reason,
            //     TransferredBy = userId
            // };
            // var result = await _mediator.Send(command);

            // For now, return mock data
            var mockTicket = new TicketDto
            {
                Id = id,
                Number = "A001",
                Status = "Waiting",
                Priority = "Normal",
                IssuedAt = DateTime.UtcNow.AddMinutes(-30),
                CustomerName = "John Doe",
                CustomerDocument = "123.456.789-00",
                CustomerPhone = "+55 11 99999-9999",
                Notes = "Customer needs assistance",
                QueueId = dto.NewQueueId,
                QueueName = "Technical Support",
                ServiceId = dto.NewServiceId,
                ServiceName = "Technical Service",
                UnitId = Guid.NewGuid(),
                UnitName = "Main Office",
                WaitingTime = TimeSpan.FromMinutes(30),
                CreatedAt = DateTime.UtcNow.AddMinutes(-30),
                UpdatedAt = DateTime.UtcNow
            };

            _logger.LogInformation("Ticket transferred successfully: {TicketId}", id);
            return Success(mockTicket);
        }
        catch (Exception ex)
        {
            return LogAndReturnError<TicketDto>(ex, "Failed to transfer ticket");
        }
    }

    /// <summary>
    /// Get public ticket status
    /// </summary>
    /// <param name="id">Ticket ID</param>
    /// <returns>Public ticket status</returns>
    [HttpGet("{id}/status")]
    [AllowAnonymous]
    [ProducesResponseType(typeof(ApiResponse<TicketStatusDto>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status404NotFound)]
    [SwaggerOperation(
        Summary = "Get ticket status",
        Description = "Retrieves public status information for a specific ticket",
        OperationId = "GetTicketStatus"
    )]
    public async Task<ActionResult<ApiResponse<TicketStatusDto>>> GetTicketStatus(Guid id)
    {
        try
        {
            _logger.LogInformation("Getting public status for ticket: {TicketId}", id);

            // TODO: Implement get ticket status query with MediatR
            // var query = new GetTicketStatusQuery { Id = id };
            // var result = await _mediator.Send(query);

            // For now, return mock data
            var mockStatus = new TicketStatusDto
            {
                Number = "A001",
                Status = "Waiting",
                Priority = "Normal",
                QueueName = "Customer Service",
                ServiceName = "General Service",
                IssuedAt = DateTime.UtcNow.AddMinutes(-30),
                CalledAt = null,
                EstimatedWaitingMinutes = 15
            };

            return Success(mockStatus);
        }
        catch (Exception ex)
        {
            return LogAndReturnError<TicketStatusDto>(ex, "Failed to get ticket status");
        }
    }
}