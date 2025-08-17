using System.ComponentModel.DataAnnotations;

namespace QueueManagement.Api.DTOs.Services;

/// <summary>
/// Service response DTO
/// </summary>
public class ServiceDto
{
    /// <summary>
    /// Service ID
    /// </summary>
    public Guid Id { get; set; }

    /// <summary>
    /// Service name
    /// </summary>
    public string Name { get; set; } = string.Empty;

    /// <summary>
    /// Service code
    /// </summary>
    public string Code { get; set; } = string.Empty;

    /// <summary>
    /// Service description
    /// </summary>
    public string? Description { get; set; }

    /// <summary>
    /// Estimated duration in minutes
    /// </summary>
    public int EstimatedDurationMinutes { get; set; }

    /// <summary>
    /// Color for UI representation
    /// </summary>
    public string Color { get; set; } = string.Empty;

    /// <summary>
    /// Whether this service requires a physical resource
    /// </summary>
    public bool RequiresResource { get; set; }

    /// <summary>
    /// Whether this service is active
    /// </summary>
    public bool IsActive { get; set; }

    /// <summary>
    /// Number of queues offering this service
    /// </summary>
    public int QueueCount { get; set; }

    /// <summary>
    /// Number of active tickets using this service
    /// </summary>
    public int ActiveTicketCount { get; set; }

    /// <summary>
    /// Creation date
    /// </summary>
    public DateTime CreatedAt { get; set; }

    /// <summary>
    /// Last update date
    /// </summary>
    public DateTime UpdatedAt { get; set; }
}

/// <summary>
/// Create service request DTO
/// </summary>
public class CreateServiceDto
{
    /// <summary>
    /// Service name
    /// </summary>
    [Required]
    [MaxLength(200)]
    public string Name { get; set; } = string.Empty;

    /// <summary>
    /// Service code
    /// </summary>
    [Required]
    [MaxLength(50)]
    [RegularExpression(@"^[A-Z0-9]+$", ErrorMessage = "Code can only contain uppercase letters and numbers")]
    public string Code { get; set; } = string.Empty;

    /// <summary>
    /// Service description
    /// </summary>
    [MaxLength(1000)]
    public string? Description { get; set; }

    /// <summary>
    /// Estimated duration in minutes
    /// </summary>
    [Required]
    [Range(1, 1440)]
    public int EstimatedDurationMinutes { get; set; }

    /// <summary>
    /// Color for UI representation
    /// </summary>
    [Required]
    [MaxLength(7)]
    [RegularExpression(@"^#[0-9A-Fa-f]{6}$", ErrorMessage = "Color must be in #RRGGBB format")]
    public string Color { get; set; } = "#007bff";

    /// <summary>
    /// Whether this service requires a physical resource
    /// </summary>
    public bool RequiresResource { get; set; }
}

/// <summary>
/// Update service request DTO
/// </summary>
public class UpdateServiceDto
{
    /// <summary>
    /// Service name
    /// </summary>
    [MaxLength(200)]
    public string? Name { get; set; }

    /// <summary>
    /// Service code
    /// </summary>
    [MaxLength(50)]
    [RegularExpression(@"^[A-Z0-9]+$", ErrorMessage = "Code can only contain uppercase letters and numbers")]
    public string? Code { get; set; }

    /// <summary>
    /// Service description
    /// </summary>
    [MaxLength(1000)]
    public string? Description { get; set; }

    /// <summary>
    /// Estimated duration in minutes
    /// </summary>
    [Range(1, 1440)]
    public int? EstimatedDurationMinutes { get; set; }

    /// <summary>
    /// Color for UI representation
    /// </summary>
    [MaxLength(7)]
    [RegularExpression(@"^#[0-9A-Fa-f]{6}$", ErrorMessage = "Color must be in #RRGGBB format")]
    public string? Color { get; set; }

    /// <summary>
    /// Whether this service requires a physical resource
    /// </summary>
    public bool? RequiresResource { get; set; }

    /// <summary>
    /// Whether this service is active
    /// </summary>
    public bool? IsActive { get; set; }
}