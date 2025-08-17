using AutoMapper;
using QueueManagement.Domain.Entities;
using QueueManagement.Api.DTOs.Auth;

namespace QueueManagement.Api.Mappings;

/// <summary>
/// Mapping profile for Auth entity mappings
/// </summary>
public class AuthMappingProfile : Profile
{
    public AuthMappingProfile()
    {
        // User to UserInfoDto
        CreateMap<User, UserInfoDto>()
            .ForMember(dest => dest.Id, opt => opt.MapFrom(src => src.Id))
            .ForMember(dest => dest.Name, opt => opt.MapFrom(src => src.Name))
            .ForMember(dest => dest.Email, opt => opt.MapFrom(src => src.Email))
            .ForMember(dest => dest.Role, opt => opt.MapFrom(src => src.Role.ToString()))
            .ForMember(dest => dest.EmployeeCode, opt => opt.MapFrom(src => src.EmployeeCode));

        // Tenant to TenantInfoDto
        CreateMap<Tenant, TenantInfoDto>()
            .ForMember(dest => dest.Id, opt => opt.MapFrom(src => src.Id))
            .ForMember(dest => dest.Name, opt => opt.MapFrom(src => src.Name))
            .ForMember(dest => dest.Subdomain, opt => opt.MapFrom(src => src.Subdomain))
            .ForMember(dest => dest.TimeZone, opt => opt.MapFrom(src => src.TimeZone));

        // RegisterTenantDto to Tenant
        CreateMap<RegisterTenantDto, Tenant>()
            .ForMember(dest => dest.Id, opt => opt.Ignore())
            .ForMember(dest => dest.Name, opt => opt.MapFrom(src => src.CompanyName))
            .ForMember(dest => dest.Subdomain, opt => opt.MapFrom(src => src.Subdomain))
            .ForMember(dest => dest.TimeZone, opt => opt.MapFrom(src => src.TimeZone))
            .ForMember(dest => dest.Status, opt => opt.MapFrom(src => Domain.Enums.TenantStatus.Active))
            .ForMember(dest => dest.CreatedAt, opt => opt.MapFrom(src => DateTime.UtcNow))
            .ForMember(dest => dest.UpdatedAt, opt => opt.MapFrom(src => DateTime.UtcNow));

        // RegisterTenantDto to User (for admin user)
        CreateMap<RegisterTenantDto, User>()
            .ForMember(dest => dest.Id, opt => opt.Ignore())
            .ForMember(dest => dest.Name, opt => opt.MapFrom(src => src.AdminName))
            .ForMember(dest => dest.Email, opt => opt.MapFrom(src => src.AdminEmail))
            .ForMember(dest => dest.PasswordHash, opt => opt.MapFrom(src => HashPassword(src.AdminPassword)))
            .ForMember(dest => dest.EmployeeCode, opt => opt.MapFrom(src => GenerateEmployeeCode()))
            .ForMember(dest => dest.Role, opt => opt.MapFrom(src => Domain.Enums.UserRole.Admin))
            .ForMember(dest => dest.Status, opt => opt.MapFrom(src => Domain.Enums.UserStatus.Available))
            .ForMember(dest => dest.CreatedAt, opt => opt.MapFrom(src => DateTime.UtcNow))
            .ForMember(dest => dest.UpdatedAt, opt => opt.MapFrom(src => DateTime.UtcNow));
    }

    /// <summary>
    /// Hash password for storage
    /// </summary>
    private static string HashPassword(string password)
    {
        // TODO: Implement proper password hashing
        // This should use a secure hashing algorithm like BCrypt or Argon2
        // For now, return a placeholder hash
        return BCrypt.Net.BCrypt.HashPassword(password);
    }

    /// <summary>
    /// Generate employee code for admin user
    /// </summary>
    private static string GenerateEmployeeCode()
    {
        // TODO: Implement proper employee code generation logic
        // This should typically involve:
        // 1. Getting the current date/time
        // 2. Getting the current sequence number for the day
        // 3. Formatting as ADMIN-YYYYMMDD-XXXX
        return "ADMIN-" + DateTime.UtcNow.ToString("yyyyMMdd") + "-" + Guid.NewGuid().ToString("N").Substring(0, 4).ToUpper();
    }
}