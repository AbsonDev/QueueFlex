using QueueManagement.SDK.Models.Common;

namespace QueueManagement.SDK.Models.Services;

/// <summary>
/// Represents a service offered by the organization.
/// </summary>
public class ServiceResponse
{
    /// <summary>
    /// Gets or sets the unique identifier of the service.
    /// </summary>
    public Guid Id { get; set; }

    /// <summary>
    /// Gets or sets the service name.
    /// </summary>
    public string Name { get; set; } = string.Empty;

    /// <summary>
    /// Gets or sets the service code.
    /// </summary>
    public string Code { get; set; } = string.Empty;

    /// <summary>
    /// Gets or sets the service description.
    /// </summary>
    public string? Description { get; set; }

    /// <summary>
    /// Gets or sets the service type.
    /// </summary>
    public ServiceType Type { get; set; }

    /// <summary>
    /// Gets or sets the estimated service duration in minutes.
    /// </summary>
    public int EstimatedDurationMinutes { get; set; }

    /// <summary>
    /// Gets or sets the maximum duration in minutes.
    /// </summary>
    public int MaxDurationMinutes { get; set; }

    /// <summary>
    /// Gets or sets whether the service requires an appointment.
    /// </summary>
    public bool RequiresAppointment { get; set; }

    /// <summary>
    /// Gets or sets whether the service is currently active.
    /// </summary>
    public bool IsActive { get; set; }

    /// <summary>
    /// Gets or sets the display order for UI purposes.
    /// </summary>
    public int DisplayOrder { get; set; }

    /// <summary>
    /// Gets or sets the icon name/URL for the service.
    /// </summary>
    public string? Icon { get; set; }

    /// <summary>
    /// Gets or sets the color code for the service.
    /// </summary>
    public string? Color { get; set; }

    /// <summary>
    /// Gets or sets the list of required documents.
    /// </summary>
    public List<string>? RequiredDocuments { get; set; }

    /// <summary>
    /// Gets or sets the list of tags/categories.
    /// </summary>
    public List<string>? Tags { get; set; }

    /// <summary>
    /// Gets or sets custom metadata.
    /// </summary>
    public Dictionary<string, object>? Metadata { get; set; }

    /// <summary>
    /// Gets or sets when the service was created.
    /// </summary>
    public DateTime CreatedAt { get; set; }

    /// <summary>
    /// Gets or sets when the service was last updated.
    /// </summary>
    public DateTime UpdatedAt { get; set; }
}

/// <summary>
/// Request to create a new service.
/// </summary>
public class CreateServiceRequest
{
    /// <summary>
    /// Gets or sets the service name.
    /// </summary>
    public string Name { get; set; } = string.Empty;

    /// <summary>
    /// Gets or sets the service code.
    /// </summary>
    public string Code { get; set; } = string.Empty;

    /// <summary>
    /// Gets or sets the service description (optional).
    /// </summary>
    public string? Description { get; set; }

    /// <summary>
    /// Gets or sets the service type (default: Standard).
    /// </summary>
    public ServiceType Type { get; set; } = ServiceType.Standard;

    /// <summary>
    /// Gets or sets the estimated duration in minutes.
    /// </summary>
    public int EstimatedDurationMinutes { get; set; }

    /// <summary>
    /// Gets or sets the maximum duration in minutes (optional).
    /// </summary>
    public int? MaxDurationMinutes { get; set; }

    /// <summary>
    /// Gets or sets whether the service requires an appointment (default: false).
    /// </summary>
    public bool RequiresAppointment { get; set; } = false;

    /// <summary>
    /// Gets or sets the display order (optional).
    /// </summary>
    public int DisplayOrder { get; set; }

    /// <summary>
    /// Gets or sets the icon name/URL (optional).
    /// </summary>
    public string? Icon { get; set; }

    /// <summary>
    /// Gets or sets the color code (optional).
    /// </summary>
    public string? Color { get; set; }

    /// <summary>
    /// Gets or sets the list of required documents (optional).
    /// </summary>
    public List<string>? RequiredDocuments { get; set; }

    /// <summary>
    /// Gets or sets the list of tags/categories (optional).
    /// </summary>
    public List<string>? Tags { get; set; }

    /// <summary>
    /// Gets or sets custom metadata (optional).
    /// </summary>
    public Dictionary<string, object>? Metadata { get; set; }
}

/// <summary>
/// Request to update an existing service.
/// </summary>
public class UpdateServiceRequest
{
    /// <summary>
    /// Gets or sets the updated name (optional).
    /// </summary>
    public string? Name { get; set; }

    /// <summary>
    /// Gets or sets the updated description (optional).
    /// </summary>
    public string? Description { get; set; }

    /// <summary>
    /// Gets or sets the updated service type (optional).
    /// </summary>
    public ServiceType? Type { get; set; }

    /// <summary>
    /// Gets or sets the updated estimated duration (optional).
    /// </summary>
    public int? EstimatedDurationMinutes { get; set; }

    /// <summary>
    /// Gets or sets the updated maximum duration (optional).
    /// </summary>
    public int? MaxDurationMinutes { get; set; }

    /// <summary>
    /// Gets or sets whether the service requires an appointment (optional).
    /// </summary>
    public bool? RequiresAppointment { get; set; }

    /// <summary>
    /// Gets or sets whether the service is active (optional).
    /// </summary>
    public bool? IsActive { get; set; }

    /// <summary>
    /// Gets or sets the updated display order (optional).
    /// </summary>
    public int? DisplayOrder { get; set; }

    /// <summary>
    /// Gets or sets the updated icon (optional).
    /// </summary>
    public string? Icon { get; set; }

    /// <summary>
    /// Gets or sets the updated color (optional).
    /// </summary>
    public string? Color { get; set; }

    /// <summary>
    /// Gets or sets the updated required documents (optional).
    /// </summary>
    public List<string>? RequiredDocuments { get; set; }

    /// <summary>
    /// Gets or sets the updated tags (optional).
    /// </summary>
    public List<string>? Tags { get; set; }

    /// <summary>
    /// Gets or sets updated metadata (optional).
    /// </summary>
    public Dictionary<string, object>? Metadata { get; set; }
}