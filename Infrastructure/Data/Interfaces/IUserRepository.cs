using QueueManagement.Domain.Entities;
using QueueManagement.Domain.Enums;

namespace QueueManagement.Infrastructure.Data.Interfaces;

/// <summary>
/// Repository interface for User entity operations
/// </summary>
public interface IUserRepository : IGenericRepository<User>
{
    /// <summary>
    /// Get user by email
    /// </summary>
    Task<User?> GetByEmailAsync(Guid tenantId, string email);

    /// <summary>
    /// Get user by employee code
    /// </summary>
    Task<User?> GetByEmployeeCodeAsync(Guid tenantId, string employeeCode);

    /// <summary>
    /// Get users by unit
    /// </summary>
    Task<List<User>> GetByUnitAsync(Guid tenantId, Guid unitId);

    /// <summary>
    /// Get active users for a tenant
    /// </summary>
    Task<List<User>> GetActiveUsersAsync(Guid tenantId);

    /// <summary>
    /// Get users by role
    /// </summary>
    Task<List<User>> GetByRoleAsync(Guid tenantId, UserRole role);

    /// <summary>
    /// Check if email exists
    /// </summary>
    Task<bool> EmailExistsAsync(Guid tenantId, string email, Guid? excludeId = null);

    /// <summary>
    /// Check if employee code exists
    /// </summary>
    Task<bool> EmployeeCodeExistsAsync(Guid tenantId, string employeeCode, Guid? excludeId = null);
}