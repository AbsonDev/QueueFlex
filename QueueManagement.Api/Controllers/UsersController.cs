using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using QueueManagement.Api.DTOs.Users;
using QueueManagement.Api.DTOs.Common;
using MediatR;
using Swashbuckle.AspNetCore.Annotations;

namespace QueueManagement.Api.Controllers;

/// <summary>
/// Controller for managing users
/// </summary>
[SwaggerTag("User management endpoints for creating, reading, updating, and managing users")]
public class UsersController : BaseController
{
    public UsersController(IMediator mediator, ILogger<UsersController> logger) : base(mediator, logger) { }

    /// <summary>
    /// Get all users with pagination and filtering
    /// </summary>
    /// <param name="filter">Filter parameters</param>
    /// <param name="pagination">Pagination parameters</param>
    /// <returns>Paginated list of users</returns>
    [HttpGet]
    [ProducesResponseType(typeof(ApiResponse<List<UserSummaryDto>>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status400BadRequest)]
    [SwaggerOperation(
        Summary = "Get all users",
        Description = "Retrieves a paginated list of users with optional filtering",
        OperationId = "GetUsers"
    )]
    public async Task<ActionResult<ApiResponse<List<UserSummaryDto>>>> GetUsers(
        [FromQuery] UserFilterDto filter,
        [FromQuery] PaginationRequestDto pagination)
    {
        try
        {
            var tenantId = GetTenantId();
            _logger.LogInformation("Getting users for tenant: {TenantId}", tenantId);

            // TODO: Implement get users query with MediatR
            // var query = new GetUsersQuery { TenantId = tenantId, Filter = filter, Pagination = pagination };
            // var result = await _mediator.Send(query);

            // For now, return mock data
            var mockUsers = new List<UserSummaryDto>
            {
                new UserSummaryDto
                {
                    Id = Guid.NewGuid(),
                    Name = "John Doe",
                    EmployeeCode = "EMP001",
                    Role = "Manager",
                    Status = "Active",
                    IsAvailable = true,
                    UnitCount = 2
                },
                new UserSummaryDto
                {
                    Id = Guid.NewGuid(),
                    Name = "Jane Smith",
                    EmployeeCode = "EMP002",
                    Role = "Agent",
                    Status = "Active",
                    IsAvailable = true,
                    UnitCount = 1
                },
                new UserSummaryDto
                {
                    Id = Guid.NewGuid(),
                    Name = "Bob Johnson",
                    EmployeeCode = "EMP003",
                    Role = "Agent",
                    Status = "Inactive",
                    IsAvailable = false,
                    UnitCount = 1
                }
            };

            var meta = new ApiMeta
            {
                Pagination = new PaginationMeta
                {
                    Page = pagination.Page,
                    PageSize = pagination.PageSize,
                    TotalItems = mockUsers.Count,
                    TotalPages = 1,
                    HasNext = false,
                    HasPrevious = false
                }
            };

            return Success(mockUsers, meta);
        }
        catch (Exception ex)
        {
            return LogAndReturnError<List<UserSummaryDto>>(ex, "Failed to get users");
        }
    }

    /// <summary>
    /// Get a user by ID
    /// </summary>
    /// <param name="id">User ID</param>
    /// <returns>User details</returns>
    [HttpGet("{id:guid}")]
    [ProducesResponseType(typeof(ApiResponse<UserDto>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status404NotFound)]
    [SwaggerOperation(
        Summary = "Get user by ID",
        Description = "Retrieves a specific user by their ID",
        OperationId = "GetUser"
    )]
    public async Task<ActionResult<ApiResponse<UserDto>>> GetUser(Guid id)
    {
        try
        {
            var tenantId = GetTenantId();
            _logger.LogInformation("Getting user {UserId} for tenant: {TenantId}", id, tenantId);

            // TODO: Implement get user query with MediatR
            // var query = new GetUserByIdQuery { UserId = id, TenantId = tenantId };
            // var result = await _mediator.Send(query);

            // For now, return mock data
            var mockUser = new UserDto
            {
                Id = id,
                Name = "John Doe",
                Email = "john.doe@company.com",
                EmployeeCode = "EMP001",
                Role = "Manager",
                Status = "Active",
                UnitCount = 2,
                ActiveSessionCount = 1,
                IsAvailable = true,
                CreatedAt = DateTime.UtcNow.AddDays(-90),
                UpdatedAt = DateTime.UtcNow
            };

            return Success(mockUser);
        }
        catch (Exception ex)
        {
            return LogAndReturnError<UserDto>(ex, "Failed to get user");
        }
    }

    /// <summary>
    /// Create a new user
    /// </summary>
    /// <param name="dto">User creation data</param>
    /// <returns>Created user</returns>
    [HttpPost]
    [ProducesResponseType(typeof(ApiResponse<UserDto>), StatusCodes.Status201Created)]
    [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status400BadRequest)]
    [SwaggerOperation(
        Summary = "Create user",
        Description = "Creates a new user",
        OperationId = "CreateUser"
    )]
    public async Task<ActionResult<ApiResponse<UserDto>>> CreateUser([FromBody] CreateUserDto dto)
    {
        try
        {
            var tenantId = GetTenantId();
            var userId = GetUserId();
            _logger.LogInformation("Creating user for tenant: {TenantId} by user: {UserId}", tenantId, userId);

            // TODO: Implement create user command with MediatR
            // var command = new CreateUserCommand 
            // { 
            //     TenantId = tenantId,
            //     Name = dto.Name,
            //     Email = dto.Email,
            //     EmployeeCode = dto.EmployeeCode,
            //     Password = dto.Password,
            //     Role = dto.Role,
            //     UnitIds = dto.UnitIds,
            //     CreatedBy = userId
            // };
            // var result = await _mediator.Send(command);

            // For now, return mock data
            var mockUser = new UserDto
            {
                Id = Guid.NewGuid(),
                Name = dto.Name,
                Email = dto.Email,
                EmployeeCode = dto.EmployeeCode,
                Role = dto.Role,
                Status = "Active",
                UnitCount = dto.UnitIds.Count,
                ActiveSessionCount = 0,
                IsAvailable = true,
                CreatedAt = DateTime.UtcNow,
                UpdatedAt = DateTime.UtcNow
            };

            _logger.LogInformation("User created successfully: {UserId}", mockUser.Id);
            return Created(mockUser, nameof(GetUser), new { id = mockUser.Id });
        }
        catch (Exception ex)
        {
            return LogAndReturnError<UserDto>(ex, "Failed to create user");
        }
    }

    /// <summary>
    /// Update a user
    /// </summary>
    /// <param name="id">User ID</param>
    /// <param name="dto">User update data</param>
    /// <returns>Updated user</returns>
    [HttpPut("{id:guid}")]
    [ProducesResponseType(typeof(ApiResponse<UserDto>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status404NotFound)]
    [SwaggerOperation(
        Summary = "Update user",
        Description = "Updates an existing user",
        OperationId = "UpdateUser"
    )]
    public async Task<ActionResult<ApiResponse<UserDto>>> UpdateUser(Guid id, [FromBody] UpdateUserDto dto)
    {
        try
        {
            var tenantId = GetTenantId();
            var userId = GetUserId();
            _logger.LogInformation("Updating user {UserId} for tenant: {TenantId} by user: {UserId}", id, tenantId, userId);

            // TODO: Implement update user command with MediatR
            // var command = new UpdateUserCommand 
            // { 
            //     UserId = id,
            //     TenantId = tenantId,
            //     Name = dto.Name,
            //     Email = dto.Email,
            //     EmployeeCode = dto.EmployeeCode,
            //     Role = dto.Role,
            //     UnitIds = dto.UnitIds,
            //     UpdatedBy = userId
            // };
            // var result = await _mediator.Send(command);

            // For now, return mock data
            var mockUser = new UserDto
            {
                Id = id,
                Name = dto.Name ?? "John Doe",
                Email = dto.Email ?? "john.doe@company.com",
                EmployeeCode = dto.EmployeeCode ?? "EMP001",
                Role = dto.Role ?? "Manager",
                Status = "Active",
                UnitCount = dto.UnitIds?.Count ?? 2,
                ActiveSessionCount = 1,
                IsAvailable = true,
                CreatedAt = DateTime.UtcNow.AddDays(-90),
                UpdatedAt = DateTime.UtcNow
            };

            _logger.LogInformation("User updated successfully: {UserId}", id);
            return Success(mockUser);
        }
        catch (Exception ex)
        {
            return LogAndReturnError<UserDto>(ex, "Failed to update user");
        }
    }

    /// <summary>
    /// Update user status
    /// </summary>
    /// <param name="id">User ID</param>
    /// <param name="dto">Status update data</param>
    /// <returns>Updated user</returns>
    [HttpPatch("{id:guid}/status")]
    [ProducesResponseType(typeof(ApiResponse<UserDto>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status404NotFound)]
    [SwaggerOperation(
        Summary = "Update user status",
        Description = "Updates the status of a user",
        OperationId = "UpdateUserStatus"
    )]
    public async Task<ActionResult<ApiResponse<UserDto>>> UpdateUserStatus(Guid id, [FromBody] UpdateUserStatusDto dto)
    {
        try
        {
            var tenantId = GetTenantId();
            var userId = GetUserId();
            _logger.LogInformation("Updating status for user {UserId} in tenant: {TenantId} by user: {UserId}", id, tenantId, userId);

            // TODO: Implement update user status command with MediatR
            // var command = new UpdateUserStatusCommand 
            // { 
            //     UserId = id,
            //     TenantId = tenantId,
            //     Status = dto.Status,
            //     Reason = dto.Reason,
            //     UpdatedBy = userId
            // };
            // var result = await _mediator.Send(command);

            // For now, return mock data
            var mockUser = new UserDto
            {
                Id = id,
                Name = "John Doe",
                Email = "john.doe@company.com",
                EmployeeCode = "EMP001",
                Role = "Manager",
                Status = dto.Status,
                UnitCount = 2,
                ActiveSessionCount = 1,
                IsAvailable = dto.Status == "Active",
                CreatedAt = DateTime.UtcNow.AddDays(-90),
                UpdatedAt = DateTime.UtcNow
            };

            _logger.LogInformation("User status updated successfully: {UserId} to {Status}", id, dto.Status);
            return Success(mockUser);
        }
        catch (Exception ex)
        {
            return LogAndReturnError<UserDto>(ex, "Failed to update user status");
        }
    }

    /// <summary>
    /// Change user password
    /// </summary>
    /// <param name="id">User ID</param>
    /// <param name="dto">Password change data</param>
    /// <returns>Success response</returns>
    [HttpPost("{id:guid}/change-password")]
    [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status404NotFound)]
    [SwaggerOperation(
        Summary = "Change user password",
        Description = "Changes the password for a user",
        OperationId = "ChangeUserPassword"
    )]
    public async Task<ActionResult<ApiResponse<object>>> ChangeUserPassword(Guid id, [FromBody] ChangePasswordDto dto)
    {
        try
        {
            var tenantId = GetTenantId();
            var userId = GetUserId();
            _logger.LogInformation("Changing password for user {UserId} in tenant: {TenantId} by user: {UserId}", id, tenantId, userId);

            // TODO: Implement change password command with MediatR
            // var command = new ChangeUserPasswordCommand 
            // { 
            //     UserId = id,
            //     TenantId = tenantId,
            //     CurrentPassword = dto.CurrentPassword,
            //     NewPassword = dto.NewPassword,
            //     ChangedBy = userId
            // };
            // await _mediator.Send(command);

            _logger.LogInformation("User password changed successfully: {UserId}", id);
            return Success<object>(null, "Password changed successfully");
        }
        catch (Exception ex)
        {
            return LogAndReturnError<object>(ex, "Failed to change user password");
        }
    }

    /// <summary>
    /// Delete a user (soft delete)
    /// </summary>
    /// <param name="id">User ID</param>
    /// <returns>Success response</returns>
    [HttpDelete("{id:guid}")]
    [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status404NotFound)]
    [SwaggerOperation(
        Summary = "Delete user",
        Description = "Soft deletes a user",
        OperationId = "DeleteUser"
    )]
    public async Task<ActionResult<ApiResponse<object>>> DeleteUser(Guid id)
    {
        try
        {
            var tenantId = GetTenantId();
            var userId = GetUserId();
            _logger.LogInformation("Deleting user {UserId} for tenant: {TenantId} by user: {UserId}", id, tenantId, userId);

            // TODO: Implement delete user command with MediatR
            // var command = new DeleteUserCommand { UserId = id, TenantId = tenantId, DeletedBy = userId };
            // await _mediator.Send(command);

            _logger.LogInformation("User deleted successfully: {UserId}", id);
            return Success<object>(null, "User deleted successfully");
        }
        catch (Exception ex)
        {
            return LogAndReturnError<object>(ex, "Failed to delete user");
        }
    }

    /// <summary>
    /// Get users by unit ID
    /// </summary>
    /// <param name="unitId">Unit ID</param>
    /// <returns>List of users assigned to the unit</returns>
    [HttpGet("unit/{unitId:guid}")]
    [ProducesResponseType(typeof(ApiResponse<List<UserSummaryDto>>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status400BadRequest)]
    [SwaggerOperation(
        Summary = "Get users by unit",
        Description = "Retrieves all users assigned to a specific unit",
        OperationId = "GetUsersByUnit"
    )]
    public async Task<ActionResult<ApiResponse<List<UserSummaryDto>>>> GetUsersByUnit(Guid unitId)
    {
        try
        {
            var tenantId = GetTenantId();
            _logger.LogInformation("Getting users for unit {UnitId} in tenant: {TenantId}", unitId, tenantId);

            // TODO: Implement get users by unit query with MediatR
            // var query = new GetUsersByUnitQuery { UnitId = unitId, TenantId = tenantId };
            // var result = await _mediator.Send(query);

            // For now, return mock data
            var mockUsers = new List<UserSummaryDto>
            {
                new UserSummaryDto
                {
                    Id = Guid.NewGuid(),
                    Name = "John Doe",
                    EmployeeCode = "EMP001",
                    Role = "Manager",
                    Status = "Active",
                    IsAvailable = true,
                    UnitCount = 1
                },
                new UserSummaryDto
                {
                    Id = Guid.NewGuid(),
                    Name = "Jane Smith",
                    EmployeeCode = "EMP002",
                    Role = "Agent",
                    Status = "Active",
                    IsAvailable = true,
                    UnitCount = 1
                }
            };

            return Success(mockUsers);
        }
        catch (Exception ex)
        {
            return LogAndReturnError<List<UserSummaryDto>>(ex, "Failed to get users by unit");
        }
    }

    /// <summary>
    /// Get available users
    /// </summary>
    /// <returns>List of available users</returns>
    [HttpGet("available")]
    [ProducesResponseType(typeof(ApiResponse<List<UserSummaryDto>>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status400BadRequest)]
    [SwaggerOperation(
        Summary = "Get available users",
        Description = "Retrieves all users who are currently available for work",
        OperationId = "GetAvailableUsers"
    )]
    public async Task<ActionResult<ApiResponse<List<UserSummaryDto>>>> GetAvailableUsers()
    {
        try
        {
            var tenantId = GetTenantId();
            _logger.LogInformation("Getting available users for tenant: {TenantId}", tenantId);

            // TODO: Implement get available users query with MediatR
            // var query = new GetAvailableUsersQuery { TenantId = tenantId };
            // var result = await _mediator.Send(query);

            // For now, return mock data
            var mockUsers = new List<UserSummaryDto>
            {
                new UserSummaryDto
                {
                    Id = Guid.NewGuid(),
                    Name = "John Doe",
                    EmployeeCode = "EMP001",
                    Role = "Manager",
                    Status = "Active",
                    IsAvailable = true,
                    UnitCount = 2
                },
                new UserSummaryDto
                {
                    Id = Guid.NewGuid(),
                    Name = "Jane Smith",
                    EmployeeCode = "EMP002",
                    Role = "Agent",
                    Status = "Active",
                    IsAvailable = true,
                    UnitCount = 1
                }
            };

            return Success(mockUsers);
        }
        catch (Exception ex)
        {
            return LogAndReturnError<List<UserSummaryDto>>(ex, "Failed to get available users");
        }
    }
}