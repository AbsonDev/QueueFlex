using AutoMapper;
using QueueManagement.Domain.Entities;
using QueueManagement.Api.DTOs.Users;

namespace QueueManagement.Api.Mappings;

/// <summary>
/// Mapping profile for User entity
/// </summary>
public class UserMappingProfile : Profile
{
    public UserMappingProfile()
    {
        // User to UserDto
        CreateMap<User, UserDto>()
            .ForMember(dest => dest.Id, opt => opt.MapFrom(src => src.Id))
            .ForMember(dest => dest.Name, opt => opt.MapFrom(src => src.Name))
            .ForMember(dest => dest.Email, opt => opt.MapFrom(src => src.Email))
            .ForMember(dest => dest.EmployeeCode, opt => opt.MapFrom(src => src.EmployeeCode))
            .ForMember(dest => dest.Role, opt => opt.MapFrom(src => src.Role.ToString()))
            .ForMember(dest => dest.Status, opt => opt.MapFrom(src => src.Status.ToString()))
            .ForMember(dest => dest.UnitCount, opt => opt.MapFrom(src => 
                src.Units != null ? src.Units.Count : 0))
            .ForMember(dest => dest.ActiveSessionCount, opt => opt.MapFrom(src => 
                src.Sessions != null ? src.Sessions.Count(s => s.Status == Domain.Enums.SessionStatus.Active) : 0))
            .ForMember(dest => dest.IsAvailable, opt => opt.MapFrom(src => 
                src.Status == Domain.Enums.UserStatus.Available))
            .ForMember(dest => dest.CreatedAt, opt => opt.MapFrom(src => src.CreatedAt))
            .ForMember(dest => dest.UpdatedAt, opt => opt.MapFrom(src => src.UpdatedAt));

        // User to UserSummaryDto
        CreateMap<User, UserSummaryDto>()
            .ForMember(dest => dest.Id, opt => opt.MapFrom(src => src.Id))
            .ForMember(dest => dest.Name, opt => opt.MapFrom(src => src.Name))
            .ForMember(dest => dest.EmployeeCode, opt => opt.MapFrom(src => src.EmployeeCode))
            .ForMember(dest => dest.Role, opt => opt.MapFrom(src => src.Role.ToString()))
            .ForMember(dest => dest.Status, opt => opt.MapFrom(src => src.Status.ToString()))
            .ForMember(dest => dest.IsAvailable, opt => opt.MapFrom(src => 
                src.Status == Domain.Enums.UserStatus.Available))
            .ForMember(dest => dest.UnitCount, opt => opt.MapFrom(src => 
                src.Units != null ? src.Units.Count : 0));

        // CreateUserDto to User
        CreateMap<CreateUserDto, User>()
            .ForMember(dest => dest.Id, opt => opt.Ignore())
            .ForMember(dest => dest.Name, opt => opt.MapFrom(src => src.Name))
            .ForMember(dest => dest.Email, opt => opt.MapFrom(src => src.Email))
            .ForMember(dest => dest.EmployeeCode, opt => opt.MapFrom(src => src.EmployeeCode))
            .ForMember(dest => dest.PasswordHash, opt => opt.MapFrom(src => HashPassword(src.Password)))
            .ForMember(dest => dest.Role, opt => opt.MapFrom(src => 
                string.IsNullOrEmpty(src.Role) ? Domain.Enums.UserRole.Agent : 
                Enum.Parse<Domain.Enums.UserRole>(src.Role, true)))
            .ForMember(dest => dest.Status, opt => opt.MapFrom(src => Domain.Enums.UserStatus.Available))
            .ForMember(dest => dest.CreatedAt, opt => opt.MapFrom(src => DateTime.UtcNow))
            .ForMember(dest => dest.UpdatedAt, opt => opt.MapFrom(src => DateTime.UtcNow));

        // UpdateUserDto to User
        CreateMap<UpdateUserDto, User>()
            .ForMember(dest => dest.Id, opt => opt.Ignore())
            .ForMember(dest => dest.Name, opt => opt.Condition(src => !string.IsNullOrEmpty(src.Name)))
            .ForMember(dest => dest.Email, opt => opt.Condition(src => !string.IsNullOrEmpty(src.Email)))
            .ForMember(dest => dest.EmployeeCode, opt => opt.Condition(src => !string.IsNullOrEmpty(src.EmployeeCode)))
            .ForMember(dest => dest.Role, opt => opt.Condition(src => !string.IsNullOrEmpty(src.Role)))
            .ForMember(dest => dest.UpdatedAt, opt => opt.MapFrom(src => DateTime.UtcNow));

        // UpdateUserStatusDto to User
        CreateMap<UpdateUserStatusDto, User>()
            .ForMember(dest => dest.Id, opt => opt.Ignore())
            .ForMember(dest => dest.Status, opt => opt.Condition(src => !string.IsNullOrEmpty(src.Status)))
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
}