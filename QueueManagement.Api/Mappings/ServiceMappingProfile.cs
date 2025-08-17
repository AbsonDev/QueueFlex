using AutoMapper;
using QueueManagement.Domain.Entities;
using QueueManagement.Api.DTOs.Services;

namespace QueueManagement.Api.Mappings;

/// <summary>
/// Mapping profile for Service entity
/// </summary>
public class ServiceMappingProfile : Profile
{
    public ServiceMappingProfile()
    {
        // Service to ServiceDto
        CreateMap<Service, ServiceDto>()
            .ForMember(dest => dest.Id, opt => opt.MapFrom(src => src.Id))
            .ForMember(dest => dest.Name, opt => opt.MapFrom(src => src.Name))
            .ForMember(dest => dest.Code, opt => opt.MapFrom(src => src.Code))
            .ForMember(dest => dest.Description, opt => opt.MapFrom(src => src.Description))
            .ForMember(dest => dest.EstimatedDurationMinutes, opt => opt.MapFrom(src => src.EstimatedDurationMinutes))
            .ForMember(dest => dest.Color, opt => opt.MapFrom(src => src.Color))
            .ForMember(dest => dest.RequiresResource, opt => opt.MapFrom(src => src.RequiresResource))
            .ForMember(dest => dest.IsActive, opt => opt.MapFrom(src => src.IsActive))
            .ForMember(dest => dest.QueueCount, opt => opt.MapFrom(src => 
                src.Queues != null ? src.Queues.Count : 0))
            .ForMember(dest => dest.ActiveTicketCount, opt => opt.MapFrom(src => 
                src.Queues != null ? src.Queues.SelectMany(q => q.Tickets ?? Enumerable.Empty<Ticket>())
                    .Count(t => t.Status == Domain.Enums.TicketStatus.Waiting || t.Status == Domain.Enums.TicketStatus.Called) : 0))
            .ForMember(dest => dest.CreatedAt, opt => opt.MapFrom(src => src.CreatedAt))
            .ForMember(dest => dest.UpdatedAt, opt => opt.MapFrom(src => src.UpdatedAt));

        // CreateServiceDto to Service
        CreateMap<CreateServiceDto, Service>()
            .ForMember(dest => dest.Id, opt => opt.Ignore())
            .ForMember(dest => dest.Name, opt => opt.MapFrom(src => src.Name))
            .ForMember(dest => dest.Code, opt => opt.MapFrom(src => src.Code))
            .ForMember(dest => dest.Description, opt => opt.MapFrom(src => src.Description))
            .ForMember(dest => dest.EstimatedDurationMinutes, opt => opt.MapFrom(src => src.EstimatedDurationMinutes))
            .ForMember(dest => dest.Color, opt => opt.MapFrom(src => src.Color))
            .ForMember(dest => dest.RequiresResource, opt => opt.MapFrom(src => src.RequiresResource))
            .ForMember(dest => dest.IsActive, opt => opt.MapFrom(src => true))
            .ForMember(dest => dest.CreatedAt, opt => opt.MapFrom(src => DateTime.UtcNow))
            .ForMember(dest => dest.UpdatedAt, opt => opt.MapFrom(src => DateTime.UtcNow));

        // UpdateServiceDto to Service
        CreateMap<UpdateServiceDto, Service>()
            .ForMember(dest => dest.Id, opt => opt.Ignore())
            .ForMember(dest => dest.Name, opt => opt.Condition(src => !string.IsNullOrEmpty(src.Name)))
            .ForMember(dest => dest.Code, opt => opt.Condition(src => !string.IsNullOrEmpty(src.Code)))
            .ForMember(dest => dest.Description, opt => opt.Condition(src => !string.IsNullOrEmpty(src.Description)))
            .ForMember(dest => dest.EstimatedDurationMinutes, opt => opt.Condition(src => src.EstimatedDurationMinutes.HasValue))
            .ForMember(dest => dest.Color, opt => opt.Condition(src => !string.IsNullOrEmpty(src.Color)))
            .ForMember(dest => dest.RequiresResource, opt => opt.Condition(src => src.RequiresResource.HasValue))
            .ForMember(dest => dest.IsActive, opt => opt.Condition(src => src.IsActive.HasValue))
            .ForMember(dest => dest.UpdatedAt, opt => opt.MapFrom(src => DateTime.UtcNow));
    }
}