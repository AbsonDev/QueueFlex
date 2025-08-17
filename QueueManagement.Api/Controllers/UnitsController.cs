using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using QueueManagement.Api.DTOs.Units;
using QueueManagement.Api.DTOs.Common;
using MediatR;
using Swashbuckle.AspNetCore.Annotations;

namespace QueueManagement.Api.Controllers;

/// <summary>
/// Controller for managing units (branches/offices)
/// </summary>
[SwaggerTag("Unit management endpoints for creating, reading, updating, and deleting units")]
public class UnitsController : BaseController
{
    public UnitsController(IMediator mediator, ILogger<UnitsController> logger) : base(mediator, logger) { }

    /// <summary>
    /// Get all units with pagination and filtering
    /// </summary>
    /// <param name="filter">Filter parameters</param>
    /// <param name="pagination">Pagination parameters</param>
    /// <returns>Paginated list of units</returns>
    [HttpGet]
    [ProducesResponseType(typeof(ApiResponse<List<UnitSummaryDto>>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status400BadRequest)]
    [SwaggerOperation(
        Summary = "Get all units",
        Description = "Retrieves a paginated list of units with optional filtering",
        OperationId = "GetUnits"
    )]
    public async Task<ActionResult<ApiResponse<List<UnitSummaryDto>>>> GetUnits(
        [FromQuery] UnitFilterDto filter,
        [FromQuery] PaginationRequestDto pagination)
    {
        try
        {
            var tenantId = GetTenantId();
            _logger.LogInformation("Getting units for tenant: {TenantId}", tenantId);

            // TODO: Implement get units query with MediatR
            // var query = new GetUnitsQuery { TenantId = tenantId, Filter = filter, Pagination = pagination };
            // var result = await _mediator.Send(query);

            // For now, return mock data
            var mockUnits = new List<UnitSummaryDto>
            {
                new UnitSummaryDto
                {
                    Id = Guid.NewGuid(),
                    Name = "Main Office",
                    Code = "MAIN",
                    Status = "Active",
                    City = "São Paulo",
                    IsOpen = true,
                    ActiveQueueCount = 3
                },
                new UnitSummaryDto
                {
                    Id = Guid.NewGuid(),
                    Name = "Branch Office",
                    Code = "BRANCH",
                    Status = "Active",
                    City = "Rio de Janeiro",
                    IsOpen = true,
                    ActiveQueueCount = 2
                }
            };

            var meta = new ApiMeta
            {
                Pagination = new PaginationMeta
                {
                    Page = pagination.Page,
                    PageSize = pagination.PageSize,
                    TotalItems = mockUnits.Count,
                    TotalPages = 1,
                    HasNext = false,
                    HasPrevious = false
                }
            };

            return Success(mockUnits, meta);
        }
        catch (Exception ex)
        {
            return LogAndReturnError<List<UnitSummaryDto>>(ex, "Failed to get units");
        }
    }

    /// <summary>
    /// Get a specific unit by ID
    /// </summary>
    /// <param name="id">Unit ID</param>
    /// <returns>Unit details</returns>
    [HttpGet("{id}")]
    [ProducesResponseType(typeof(ApiResponse<UnitDto>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status404NotFound)]
    [SwaggerOperation(
        Summary = "Get unit by ID",
        Description = "Retrieves detailed information about a specific unit",
        OperationId = "GetUnit"
    )]
    public async Task<ActionResult<ApiResponse<UnitDto>>> GetUnit(Guid id)
    {
        try
        {
            var tenantId = GetTenantId();
            _logger.LogInformation("Getting unit {UnitId} for tenant: {TenantId}", id, tenantId);

            // TODO: Implement get unit query with MediatR
            // var query = new GetUnitByIdQuery { Id = id, TenantId = tenantId };
            // var result = await _mediator.Send(query);

            // For now, return mock data
            var mockUnit = new UnitDto
            {
                Id = id,
                Name = "Main Office",
                Code = "MAIN",
                Status = "Active",
                Address = new AddressDto
                {
                    Street = "Main Street",
                    Number = "123",
                    Neighborhood = "Downtown",
                    City = "São Paulo",
                    State = "SP",
                    ZipCode = "01234-567",
                    Country = "Brazil"
                },
                QueueCount = 3,
                UserCount = 5,
                IsOpen = true,
                CreatedAt = DateTime.UtcNow.AddDays(-30),
                UpdatedAt = DateTime.UtcNow
            };

            return Success(mockUnit);
        }
        catch (Exception ex)
        {
            return LogAndReturnError<UnitDto>(ex, "Failed to get unit");
        }
    }

    /// <summary>
    /// Create a new unit
    /// </summary>
    /// <param name="dto">Unit creation data</param>
    /// <returns>Created unit</returns>
    [HttpPost]
    [ProducesResponseType(typeof(ApiResponse<UnitDto>), StatusCodes.Status201Created)]
    [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status400BadRequest)]
    [SwaggerOperation(
        Summary = "Create unit",
        Description = "Creates a new unit for the tenant",
        OperationId = "CreateUnit"
    )]
    public async Task<ActionResult<ApiResponse<UnitDto>>> CreateUnit([FromBody] CreateUnitDto dto)
    {
        try
        {
            var tenantId = GetTenantId();
            var userId = GetUserId();
            _logger.LogInformation("Creating unit for tenant: {TenantId} by user: {UserId}", tenantId, userId);

            // TODO: Implement create unit command with MediatR
            // var command = new CreateUnitCommand 
            // { 
            //     TenantId = tenantId,
            //     Name = dto.Name,
            //     Code = dto.Code,
            //     Address = dto.Address,
            //     CreatedBy = userId
            // };
            // var result = await _mediator.Send(command);

            // For now, return mock data
            var mockUnit = new UnitDto
            {
                Id = Guid.NewGuid(),
                Name = dto.Name,
                Code = dto.Code,
                Status = "Active",
                Address = dto.Address,
                QueueCount = 0,
                UserCount = 0,
                IsOpen = false,
                CreatedAt = DateTime.UtcNow,
                UpdatedAt = DateTime.UtcNow
            };

            _logger.LogInformation("Unit created successfully: {UnitId}", mockUnit.Id);
            return Created(mockUnit, nameof(GetUnit), new { id = mockUnit.Id });
        }
        catch (Exception ex)
        {
            return LogAndReturnError<UnitDto>(ex, "Failed to create unit");
        }
    }

    /// <summary>
    /// Update an existing unit
    /// </summary>
    /// <param name="id">Unit ID</param>
    /// <param name="dto">Unit update data</param>
    /// <returns>Updated unit</returns>
    [HttpPut("{id}")]
    [ProducesResponseType(typeof(ApiResponse<UnitDto>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status404NotFound)]
    [SwaggerOperation(
        Summary = "Update unit",
        Description = "Updates an existing unit",
        OperationId = "UpdateUnit"
    )]
    public async Task<ActionResult<ApiResponse<UnitDto>>> UpdateUnit(Guid id, [FromBody] UpdateUnitDto dto)
    {
        try
        {
            var tenantId = GetTenantId();
            var userId = GetUserId();
            _logger.LogInformation("Updating unit {UnitId} for tenant: {TenantId} by user: {UserId}", id, tenantId, userId);

            // TODO: Implement update unit command with MediatR
            // var command = new UpdateUnitCommand 
            // { 
            //     Id = id,
            //     TenantId = tenantId,
            //     Name = dto.Name,
            //     Code = dto.Code,
            //     Address = dto.Address,
            //     Status = dto.Status,
            //     UpdatedBy = userId
            // };
            // var result = await _mediator.Send(command);

            // For now, return mock data
            var mockUnit = new UnitDto
            {
                Id = id,
                Name = dto.Name ?? "Main Office",
                Code = dto.Code ?? "MAIN",
                Status = dto.Status ?? "Active",
                Address = dto.Address ?? new AddressDto
                {
                    Street = "Main Street",
                    Number = "123",
                    Neighborhood = "Downtown",
                    City = "São Paulo",
                    State = "SP",
                    ZipCode = "01234-567",
                    Country = "Brazil"
                },
                QueueCount = 3,
                UserCount = 5,
                IsOpen = true,
                CreatedAt = DateTime.UtcNow.AddDays(-30),
                UpdatedAt = DateTime.UtcNow
            };

            _logger.LogInformation("Unit updated successfully: {UnitId}", id);
            return Success(mockUnit);
        }
        catch (Exception ex)
        {
            return LogAndReturnError<UnitDto>(ex, "Failed to update unit");
        }
    }

    /// <summary>
    /// Delete a unit
    /// </summary>
    /// <param name="id">Unit ID</param>
    /// <returns>No content</returns>
    [HttpDelete("{id}")]
    [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status204NoContent)]
    [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status404NotFound)]
    [SwaggerOperation(
        Summary = "Delete unit",
        Description = "Soft deletes a unit",
        OperationId = "DeleteUnit"
    )]
    public async Task<ActionResult<ApiResponse<object>>> DeleteUnit(Guid id)
    {
        try
        {
            var tenantId = GetTenantId();
            var userId = GetUserId();
            _logger.LogInformation("Deleting unit {UnitId} for tenant: {TenantId} by user: {UserId}", id, tenantId, userId);

            // TODO: Implement delete unit command with MediatR
            // var command = new DeleteUnitCommand { Id = id, TenantId = tenantId, DeletedBy = userId };
            // await _mediator.Send(command);

            _logger.LogInformation("Unit deleted successfully: {UnitId}", id);
            return NoContent();
        }
        catch (Exception ex)
        {
            return LogAndReturnError<object>(ex, "Failed to delete unit");
        }
    }

    /// <summary>
    /// Get queues for a specific unit
    /// </summary>
    /// <param name="id">Unit ID</param>
    /// <returns>List of queues in the unit</returns>
    [HttpGet("{id}/queues")]
    [ProducesResponseType(typeof(ApiResponse<List<object>>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status404NotFound)]
    [SwaggerOperation(
        Summary = "Get unit queues",
        Description = "Retrieves all queues associated with a specific unit",
        OperationId = "GetUnitQueues"
    )]
    public async Task<ActionResult<ApiResponse<List<object>>>> GetUnitQueues(Guid id)
    {
        try
        {
            var tenantId = GetTenantId();
            _logger.LogInformation("Getting queues for unit {UnitId} in tenant: {TenantId}", id, tenantId);

            // TODO: Implement get unit queues query with MediatR
            // var query = new GetUnitQueuesQuery { UnitId = id, TenantId = tenantId };
            // var result = await _mediator.Send(query);

            // For now, return mock data
            var mockQueues = new List<object>
            {
                new { Id = Guid.NewGuid(), Name = "Customer Service", Status = "Open" },
                new { Id = Guid.NewGuid(), Name = "Technical Support", Status = "Open" }
            };

            return Success(mockQueues);
        }
        catch (Exception ex)
        {
            return LogAndReturnError<List<object>>(ex, "Failed to get unit queues");
        }
    }

    /// <summary>
    /// Get users assigned to a specific unit
    /// </summary>
    /// <param name="id">Unit ID</param>
    /// <returns>List of users in the unit</returns>
    [HttpGet("{id}/users")]
    [ProducesResponseType(typeof(ApiResponse<List<object>>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status404NotFound)]
    [SwaggerOperation(
        Summary = "Get unit users",
        Description = "Retrieves all users assigned to a specific unit",
        OperationId = "GetUnitUsers"
    )]
    public async Task<ActionResult<ApiResponse<List<object>>>> GetUnitUsers(Guid id)
    {
        try
        {
            var tenantId = GetTenantId();
            _logger.LogInformation("Getting users for unit {UnitId} in tenant: {TenantId}", id, tenantId);

            // TODO: Implement get unit users query with MediatR
            // var query = new GetUnitUsersQuery { UnitId = id, TenantId = tenantId };
            // var result = await _mediator.Send(query);

            // For now, return mock data
            var mockUsers = new List<object>
            {
                new { Id = Guid.NewGuid(), Name = "John Doe", Role = "Attendant" },
                new { Id = Guid.NewGuid(), Name = "Jane Smith", Role = "Manager" }
            };

            return Success(mockUsers);
        }
        catch (Exception ex)
        {
            return LogAndReturnError<List<object>>(ex, "Failed to get unit users");
        }
    }
}