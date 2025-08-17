using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using QueueManagement.Domain.Entities;
using QueueManagement.Domain.Enums;
using QueueManagement.Infrastructure.Data.Interfaces;

namespace QueueManagement.Infrastructure.Data.Repositories;

/// <summary>
/// Repository implementation for User entity with optimized queries
/// </summary>
public class UserRepository : GenericRepository<User>, IUserRepository
{
    public UserRepository(QueueManagementDbContext context, ILogger<UserRepository> logger) 
        : base(context, logger) { }

    public async Task<User?> GetByEmailAsync(Guid tenantId, string email)
    {
        try
        {
            _logger.LogDebug("Getting user by email {Email} for tenant {TenantId}", email, tenantId);

            return await _dbSet
                .Include(u => u.Unit)
                .Include(u => u.Sessions.Where(s => !s.IsDeleted))
                .AsNoTracking()
                .Where(u => u.Email == email && u.Unit.TenantId == tenantId && !u.IsDeleted)
                .FirstOrDefaultAsync();
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error getting user by email {Email} for tenant {TenantId}", email, tenantId);
            throw;
        }
    }

    public async Task<User?> GetByEmployeeCodeAsync(Guid tenantId, string employeeCode)
    {
        try
        {
            _logger.LogDebug("Getting user by employee code {EmployeeCode} for tenant {TenantId}", employeeCode, tenantId);

            return await _dbSet
                .Include(u => u.Unit)
                .Include(u => u.Sessions.Where(s => !s.IsDeleted))
                .AsNoTracking()
                .Where(u => u.EmployeeCode == employeeCode && u.Unit.TenantId == tenantId && !u.IsDeleted)
                .FirstOrDefaultAsync();
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error getting user by employee code {EmployeeCode} for tenant {TenantId}", 
                employeeCode, tenantId);
            throw;
        }
    }

    public async Task<List<User>> GetByUnitAsync(Guid tenantId, Guid unitId)
    {
        try
        {
            _logger.LogDebug("Getting users by unit {UnitId} for tenant {TenantId}", unitId, tenantId);

            return await _dbSet
                .Include(u => u.Unit)
                .Include(u => u.Sessions.Where(s => !s.IsDeleted))
                .AsNoTracking()
                .Where(u => u.UnitId == unitId && u.Unit.TenantId == tenantId && !u.IsDeleted)
                .OrderBy(u => u.Name)
                .ToListAsync();
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error getting users by unit {UnitId} for tenant {TenantId}", unitId, tenantId);
            throw;
        }
    }

    public async Task<List<User>> GetActiveUsersAsync(Guid tenantId)
    {
        try
        {
            _logger.LogDebug("Getting active users for tenant {TenantId}", tenantId);

            return await _dbSet
                .Include(u => u.Unit)
                .AsNoTracking()
                .Where(u => u.Unit.TenantId == tenantId && u.IsActive && !u.IsDeleted)
                .OrderBy(u => u.Unit.Name)
                .ThenBy(u => u.Name)
                .ToListAsync();
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error getting active users for tenant {TenantId}", tenantId);
            throw;
        }
    }

    public async Task<List<User>> GetByRoleAsync(Guid tenantId, UserRole role)
    {
        try
        {
            _logger.LogDebug("Getting users by role {Role} for tenant {TenantId}", role, tenantId);

            return await _dbSet
                .Include(u => u.Unit)
                .AsNoTracking()
                .Where(u => u.Unit.TenantId == tenantId && u.Role == role && u.IsActive && !u.IsDeleted)
                .OrderBy(u => u.Unit.Name)
                .ThenBy(u => u.Name)
                .ToListAsync();
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error getting users by role {Role} for tenant {TenantId}", role, tenantId);
            throw;
        }
    }

    public async Task<bool> EmailExistsAsync(Guid tenantId, string email, Guid? excludeId = null)
    {
        try
        {
            _logger.LogDebug("Checking if email {Email} exists for tenant {TenantId}", email, tenantId);

            var query = _dbSet
                .AsNoTracking()
                .Where(u => u.Email == email && u.Unit.TenantId == tenantId && !u.IsDeleted);

            if (excludeId.HasValue)
            {
                query = query.Where(u => u.Id != excludeId.Value);
            }

            return await query.AnyAsync();
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error checking if email {Email} exists for tenant {TenantId}", email, tenantId);
            throw;
        }
    }

    public async Task<bool> EmployeeCodeExistsAsync(Guid tenantId, string employeeCode, Guid? excludeId = null)
    {
        try
        {
            _logger.LogDebug("Checking if employee code {EmployeeCode} exists for tenant {TenantId}", 
                employeeCode, tenantId);

            var query = _dbSet
                .AsNoTracking()
                .Where(u => u.EmployeeCode == employeeCode && u.Unit.TenantId == tenantId && !u.IsDeleted);

            if (excludeId.HasValue)
            {
                query = query.Where(u => u.Id != excludeId.Value);
            }

            return await query.AnyAsync();
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error checking if employee code {EmployeeCode} exists for tenant {TenantId}", 
                employeeCode, tenantId);
            throw;
        }
    }
}