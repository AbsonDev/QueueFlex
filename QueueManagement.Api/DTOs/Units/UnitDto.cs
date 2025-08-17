using System.ComponentModel.DataAnnotations;
using QueueManagement.Domain.Enums;

namespace QueueManagement.Api.DTOs.Units;

/// <summary>
/// Unit response DTO
/// </summary>
public class UnitDto
{
    /// <summary>
    /// Unit ID
    /// </summary>
    public Guid Id { get; set; }

    /// <summary>
    /// Unit name
    /// </summary>
    public string Name { get; set; } = string.Empty;

    /// <summary>
    /// Unit code
    /// </summary>
    public string Code { get; set; } = string.Empty;

    /// <summary>
    /// Unit status
    /// </summary>
    public string Status { get; set; } = string.Empty;

    /// <summary>
    /// Unit address
    /// </summary>
    public AddressDto Address { get; set; } = null!;

    /// <summary>
    /// Number of queues in this unit
    /// </summary>
    public int QueueCount { get; set; }

    /// <summary>
    /// Number of users assigned to this unit
    /// </summary>
    public int UserCount { get; set; }

    /// <summary>
    /// Whether the unit is currently open
    /// </summary>
    public bool IsOpen { get; set; }

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
/// Unit summary DTO for lists
/// </summary>
public class UnitSummaryDto
{
    /// <summary>
    /// Unit ID
    /// </summary>
    public Guid Id { get; set; }

    /// <summary>
    /// Unit name
    /// </summary>
    public string Name { get; set; } = string.Empty;

    /// <summary>
    /// Unit code
    /// </summary>
    public string Code { get; set; } = string.Empty;

    /// <summary>
    /// Unit status
    /// </summary>
    public string Status { get; set; } = string.Empty;

    /// <summary>
    /// City where the unit is located
    /// </summary>
    public string City { get; set; } = string.Empty;

    /// <summary>
    /// Whether the unit is currently open
    /// </summary>
    public bool IsOpen { get; set; }

    /// <summary>
    /// Number of active queues
    /// </summary>
    public int ActiveQueueCount { get; set; }
}

/// <summary>
/// Create unit request DTO
/// </summary>
public class CreateUnitDto
{
    /// <summary>
    /// Unit name
    /// </summary>
    [Required]
    [MaxLength(200)]
    public string Name { get; set; } = string.Empty;

    /// <summary>
    /// Unit code
    /// </summary>
    [Required]
    [MaxLength(50)]
    [RegularExpression(@"^[A-Z0-9]+$", ErrorMessage = "Code can only contain uppercase letters and numbers")]
    public string Code { get; set; } = string.Empty;

    /// <summary>
    /// Unit address
    /// </summary>
    [Required]
    public AddressDto Address { get; set; } = null!;
}

/// <summary>
/// Update unit request DTO
/// </summary>
public class UpdateUnitDto
{
    /// <summary>
    /// Unit name
    /// </summary>
    [MaxLength(200)]
    public string? Name { get; set; }

    /// <summary>
    /// Unit code
    /// </summary>
    [MaxLength(50)]
    [RegularExpression(@"^[A-Z0-9]+$", ErrorMessage = "Code can only contain uppercase letters and numbers")]
    public string? Code { get; set; }

    /// <summary>
    /// Unit address
    /// </summary>
    public AddressDto? Address { get; set; }

    /// <summary>
    /// Unit status
    /// </summary>
    public string? Status { get; set; }
}

/// <summary>
/// Address DTO
/// </summary>
public class AddressDto
{
    /// <summary>
    /// Street name
    /// </summary>
    [Required]
    [MaxLength(200)]
    public string Street { get; set; } = string.Empty;

    /// <summary>
    /// Street number
    /// </summary>
    [Required]
    [MaxLength(20)]
    public string Number { get; set; } = string.Empty;

    /// <summary>
    /// Address complement
    /// </summary>
    [MaxLength(100)]
    public string? Complement { get; set; }

    /// <summary>
    /// Neighborhood
    /// </summary>
    [Required]
    [MaxLength(100)]
    public string Neighborhood { get; set; } = string.Empty;

    /// <summary>
    /// City name
    /// </summary>
    [Required]
    [MaxLength(100)]
    public string City { get; set; } = string.Empty;

    /// <summary>
    /// State or province
    /// </summary>
    [Required]
    [MaxLength(100)]
    public string State { get; set; } = string.Empty;

    /// <summary>
    /// ZIP code
    /// </summary>
    [Required]
    [MaxLength(20)]
    public string ZipCode { get; set; } = string.Empty;

    /// <summary>
    /// Country name
    /// </summary>
    [Required]
    [MaxLength(100)]
    public string Country { get; set; } = string.Empty;
}