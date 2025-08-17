using AutoMapper;
using QueueManagement.Domain.Entities;
using QueueManagement.Api.DTOs.Units;

namespace QueueManagement.Api.Mappings;

/// <summary>
/// Mapping profile for Unit entity
/// </summary>
public class UnitMappingProfile : Profile
{
    public UnitMappingProfile()
    {
        // Unit to UnitDto
        CreateMap<Unit, UnitDto>()
            .ForMember(dest => dest.Id, opt => opt.MapFrom(src => src.Id))
            .ForMember(dest => dest.Name, opt => opt.MapFrom(src => src.Name))
            .ForMember(dest => dest.Code, opt => opt.MapFrom(src => src.Code))
            .ForMember(dest => dest.Status, opt => opt.MapFrom(src => src.Status.ToString()))
            .ForMember(dest => dest.Address, opt => opt.MapFrom(src => src.Address))
            .ForMember(dest => dest.QueueCount, opt => opt.MapFrom(src => src.Queues != null ? src.Queues.Count : 0))
            .ForMember(dest => dest.UserCount, opt => opt.MapFrom(src => src.Users != null ? src.Users.Count : 0))
            .ForMember(dest => dest.IsOpen, opt => opt.MapFrom(src => src.Status == Domain.Enums.UnitStatus.Active))
            .ForMember(dest => dest.CreatedAt, opt => opt.MapFrom(src => src.CreatedAt))
            .ForMember(dest => dest.UpdatedAt, opt => opt.MapFrom(src => src.UpdatedAt));

        // Unit to UnitSummaryDto
        CreateMap<Unit, UnitSummaryDto>()
            .ForMember(dest => dest.Id, opt => opt.MapFrom(src => src.Id))
            .ForMember(dest => dest.Name, opt => opt.MapFrom(src => src.Name))
            .ForMember(dest => dest.Code, opt => opt.MapFrom(src => src.Code))
            .ForMember(dest => dest.Status, opt => opt.MapFrom(src => src.Status.ToString()))
            .ForMember(dest => dest.City, opt => opt.MapFrom(src => src.Address.City))
            .ForMember(dest => dest.IsOpen, opt => opt.MapFrom(src => src.Status == Domain.Enums.UnitStatus.Active))
            .ForMember(dest => dest.ActiveQueueCount, opt => opt.MapFrom(src => 
                src.Queues != null ? src.Queues.Count(q => q.IsActive) : 0));

        // CreateUnitDto to Unit
        CreateMap<CreateUnitDto, Unit>()
            .ForMember(dest => dest.Id, opt => opt.Ignore())
            .ForMember(dest => dest.Name, opt => opt.MapFrom(src => src.Name))
            .ForMember(dest => dest.Code, opt => opt.MapFrom(src => src.Code))
            .ForMember(dest => dest.Status, opt => opt.MapFrom(src => Domain.Enums.UnitStatus.Active))
            .ForMember(dest => dest.Address, opt => opt.MapFrom(src => src.Address))
            .ForMember(dest => dest.CreatedAt, opt => opt.MapFrom(src => DateTime.UtcNow))
            .ForMember(dest => dest.UpdatedAt, opt => opt.MapFrom(src => DateTime.UtcNow));

        // UpdateUnitDto to Unit
        CreateMap<UpdateUnitDto, Unit>()
            .ForMember(dest => dest.Id, opt => opt.Ignore())
            .ForMember(dest => dest.Name, opt => opt.Condition(src => !string.IsNullOrEmpty(src.Name)))
            .ForMember(dest => dest.Code, opt => opt.Condition(src => !string.IsNullOrEmpty(src.Code)))
            .ForMember(dest => dest.Status, opt => opt.Condition(src => !string.IsNullOrEmpty(src.Status)))
            .ForMember(dest => dest.Address, opt => opt.Condition(src => src.Address != null))
            .ForMember(dest => dest.UpdatedAt, opt => opt.MapFrom(src => DateTime.UtcNow));

        // AddressDto to Address
        CreateMap<AddressDto, Domain.ValueObjects.Address>()
            .ForMember(dest => dest.Street, opt => opt.MapFrom(src => src.Street))
            .ForMember(dest => dest.Number, opt => opt.MapFrom(src => src.Number))
            .ForMember(dest => dest.Complement, opt => opt.MapFrom(src => src.Complement))
            .ForMember(dest => dest.Neighborhood, opt => opt.MapFrom(src => src.Neighborhood))
            .ForMember(dest => dest.City, opt => opt.MapFrom(src => src.City))
            .ForMember(dest => dest.State, opt => opt.MapFrom(src => src.State))
            .ForMember(dest => dest.ZipCode, opt => opt.MapFrom(src => src.ZipCode))
            .ForMember(dest => dest.Country, opt => opt.MapFrom(src => src.Country));

        // Address to AddressDto
        CreateMap<Domain.ValueObjects.Address, AddressDto>()
            .ForMember(dest => dest.Street, opt => opt.MapFrom(src => src.Street))
            .ForMember(dest => dest.Number, opt => opt.MapFrom(src => src.Number))
            .ForMember(dest => dest.Complement, opt => opt.MapFrom(src => src.Complement))
            .ForMember(dest => dest.Neighborhood, opt => opt.MapFrom(src => src.Neighborhood))
            .ForMember(dest => dest.City, opt => opt.MapFrom(src => src.City))
            .ForMember(dest => dest.State, opt => opt.MapFrom(src => src.State))
            .ForMember(dest => dest.ZipCode, opt => opt.MapFrom(src => src.ZipCode))
            .ForMember(dest => dest.Country, opt => opt.MapFrom(src => src.Country));
    }
}