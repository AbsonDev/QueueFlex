using QueueManagement.SDK.Models.Common;
using QueueManagement.SDK.Models.Units;

namespace QueueManagement.SDK.Services;

/// <summary>
/// Client for managing units/branches.
/// </summary>
public interface IUnitsClient
{
    /// <summary>
    /// Creates a new unit.
    /// </summary>
    Task<UnitResponse> CreateAsync(CreateUnitRequest request, CancellationToken cancellationToken = default);

    /// <summary>
    /// Gets a unit by ID.
    /// </summary>
    Task<UnitResponse> GetAsync(Guid unitId, CancellationToken cancellationToken = default);

    /// <summary>
    /// Updates an existing unit.
    /// </summary>
    Task<UnitResponse> UpdateAsync(Guid unitId, UpdateUnitRequest request, CancellationToken cancellationToken = default);

    /// <summary>
    /// Deletes a unit.
    /// </summary>
    Task DeleteAsync(Guid unitId, CancellationToken cancellationToken = default);

    /// <summary>
    /// Gets all units.
    /// </summary>
    Task<List<UnitResponse>> GetAllAsync(CancellationToken cancellationToken = default);

    /// <summary>
    /// Gets paginated units.
    /// </summary>
    Task<PagedResult<UnitResponse>> GetPagedAsync(PagedRequest request, CancellationToken cancellationToken = default);

    /// <summary>
    /// Opens a unit.
    /// </summary>
    Task<UnitResponse> OpenAsync(Guid unitId, CancellationToken cancellationToken = default);

    /// <summary>
    /// Closes a unit.
    /// </summary>
    Task<UnitResponse> CloseAsync(Guid unitId, CancellationToken cancellationToken = default);

    /// <summary>
    /// Gets units by city.
    /// </summary>
    Task<List<UnitResponse>> GetByCityAsync(string city, CancellationToken cancellationToken = default);

    /// <summary>
    /// Gets units by state.
    /// </summary>
    Task<List<UnitResponse>> GetByStateAsync(string state, CancellationToken cancellationToken = default);

    /// <summary>
    /// Checks if a unit is open.
    /// </summary>
    Task<bool> IsOpenAsync(Guid unitId, CancellationToken cancellationToken = default);

    /// <summary>
    /// Gets the working hours for a unit.
    /// </summary>
    Task<Dictionary<string, WorkingHours>> GetWorkingHoursAsync(Guid unitId, CancellationToken cancellationToken = default);

    /// <summary>
    /// Updates the working hours for a unit.
    /// </summary>
    Task<UnitResponse> UpdateWorkingHoursAsync(Guid unitId, Dictionary<string, WorkingHours> workingHours, CancellationToken cancellationToken = default);
}