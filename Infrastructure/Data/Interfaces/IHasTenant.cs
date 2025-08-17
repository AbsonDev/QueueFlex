namespace QueueManagement.Infrastructure.Data.Interfaces;

/// <summary>
/// Interface for entities that have tenant information
/// </summary>
public interface IHasTenant
{
    Guid TenantId { get; set; }
}