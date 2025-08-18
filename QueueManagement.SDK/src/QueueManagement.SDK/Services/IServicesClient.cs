using QueueManagement.SDK.Models.Common;
using QueueManagement.SDK.Models.Services;

namespace QueueManagement.SDK.Services;

/// <summary>
/// Client for managing services.
/// </summary>
public interface IServicesClient
{
    /// <summary>
    /// Creates a new service.
    /// </summary>
    Task<ServiceResponse> CreateAsync(CreateServiceRequest request, CancellationToken cancellationToken = default);

    /// <summary>
    /// Gets a service by ID.
    /// </summary>
    Task<ServiceResponse> GetAsync(Guid serviceId, CancellationToken cancellationToken = default);

    /// <summary>
    /// Updates an existing service.
    /// </summary>
    Task<ServiceResponse> UpdateAsync(Guid serviceId, UpdateServiceRequest request, CancellationToken cancellationToken = default);

    /// <summary>
    /// Deletes a service.
    /// </summary>
    Task DeleteAsync(Guid serviceId, CancellationToken cancellationToken = default);

    /// <summary>
    /// Gets all services.
    /// </summary>
    Task<List<ServiceResponse>> GetAllAsync(CancellationToken cancellationToken = default);

    /// <summary>
    /// Gets active services.
    /// </summary>
    Task<List<ServiceResponse>> GetActiveAsync(CancellationToken cancellationToken = default);

    /// <summary>
    /// Gets services by type.
    /// </summary>
    Task<List<ServiceResponse>> GetByTypeAsync(ServiceType type, CancellationToken cancellationToken = default);

    /// <summary>
    /// Gets paginated services.
    /// </summary>
    Task<PagedResult<ServiceResponse>> GetPagedAsync(PagedRequest request, CancellationToken cancellationToken = default);

    /// <summary>
    /// Activates a service.
    /// </summary>
    Task<ServiceResponse> ActivateAsync(Guid serviceId, CancellationToken cancellationToken = default);

    /// <summary>
    /// Deactivates a service.
    /// </summary>
    Task<ServiceResponse> DeactivateAsync(Guid serviceId, CancellationToken cancellationToken = default);

    /// <summary>
    /// Gets services by tag.
    /// </summary>
    Task<List<ServiceResponse>> GetByTagAsync(string tag, CancellationToken cancellationToken = default);

    /// <summary>
    /// Searches services by name or description.
    /// </summary>
    Task<List<ServiceResponse>> SearchAsync(string query, CancellationToken cancellationToken = default);
}