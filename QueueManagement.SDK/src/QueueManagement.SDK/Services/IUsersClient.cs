using QueueManagement.SDK.Models.Common;
using QueueManagement.SDK.Models.Users;

namespace QueueManagement.SDK.Services;

/// <summary>
/// Client for managing users.
/// </summary>
public interface IUsersClient
{
    /// <summary>
    /// Creates a new user.
    /// </summary>
    Task<UserResponse> CreateAsync(CreateUserRequest request, CancellationToken cancellationToken = default);

    /// <summary>
    /// Gets a user by ID.
    /// </summary>
    Task<UserResponse> GetAsync(Guid userId, CancellationToken cancellationToken = default);

    /// <summary>
    /// Updates an existing user.
    /// </summary>
    Task<UserResponse> UpdateAsync(Guid userId, UpdateUserRequest request, CancellationToken cancellationToken = default);

    /// <summary>
    /// Deletes a user.
    /// </summary>
    Task DeleteAsync(Guid userId, CancellationToken cancellationToken = default);

    /// <summary>
    /// Gets all users.
    /// </summary>
    Task<List<UserResponse>> GetAllAsync(CancellationToken cancellationToken = default);

    /// <summary>
    /// Gets users by role.
    /// </summary>
    Task<List<UserResponse>> GetByRoleAsync(UserRole role, CancellationToken cancellationToken = default);

    /// <summary>
    /// Gets users by unit.
    /// </summary>
    Task<List<UserResponse>> GetByUnitAsync(Guid unitId, CancellationToken cancellationToken = default);

    /// <summary>
    /// Gets paginated users.
    /// </summary>
    Task<PagedResult<UserResponse>> GetPagedAsync(PagedRequest request, CancellationToken cancellationToken = default);

    /// <summary>
    /// Gets the current user.
    /// </summary>
    Task<UserResponse> GetCurrentAsync(CancellationToken cancellationToken = default);

    /// <summary>
    /// Changes a user's password.
    /// </summary>
    Task ChangePasswordAsync(Guid userId, ChangePasswordRequest request, CancellationToken cancellationToken = default);

    /// <summary>
    /// Activates a user.
    /// </summary>
    Task<UserResponse> ActivateAsync(Guid userId, CancellationToken cancellationToken = default);

    /// <summary>
    /// Deactivates a user.
    /// </summary>
    Task<UserResponse> DeactivateAsync(Guid userId, CancellationToken cancellationToken = default);

    /// <summary>
    /// Gets online users.
    /// </summary>
    Task<List<UserResponse>> GetOnlineAsync(CancellationToken cancellationToken = default);

    /// <summary>
    /// Searches users by name or email.
    /// </summary>
    Task<List<UserResponse>> SearchAsync(string query, CancellationToken cancellationToken = default);

    /// <summary>
    /// Assigns a user to a unit.
    /// </summary>
    Task<UserResponse> AssignToUnitAsync(Guid userId, Guid unitId, CancellationToken cancellationToken = default);

    /// <summary>
    /// Updates user queues.
    /// </summary>
    Task<UserResponse> UpdateQueuesAsync(Guid userId, List<Guid> queueIds, CancellationToken cancellationToken = default);

    /// <summary>
    /// Updates user services.
    /// </summary>
    Task<UserResponse> UpdateServicesAsync(Guid userId, List<Guid> serviceIds, CancellationToken cancellationToken = default);
}