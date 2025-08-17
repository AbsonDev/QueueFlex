using AutoMapper;
using QueueManagement.Domain.Entities;
using QueueManagement.Api.DTOs.Queues;

namespace QueueManagement.Api.Mappings;

/// <summary>
/// Mapping profile for Queue entity
/// </summary>
public class QueueMappingProfile : Profile
{
    public QueueMappingProfile()
    {
        // Queue to QueueDto
        CreateMap<Queue, QueueDto>()
            .ForMember(dest => dest.Id, opt => opt.MapFrom(src => src.Id))
            .ForMember(dest => dest.Name, opt => opt.MapFrom(src => src.Name))
            .ForMember(dest => dest.Code, opt => opt.MapFrom(src => src.Code))
            .ForMember(dest => dest.DisplayName, opt => opt.MapFrom(src => src.DisplayName))
            .ForMember(dest => dest.MaxCapacity, opt => opt.MapFrom(src => src.MaxCapacity))
            .ForMember(dest => dest.Status, opt => opt.MapFrom(src => src.Status.ToString()))
            .ForMember(dest => dest.IsActive, opt => opt.MapFrom(src => src.IsActive))
            .ForMember(dest => dest.UnitId, opt => opt.MapFrom(src => src.UnitId))
            .ForMember(dest => dest.UnitName, opt => opt.MapFrom(src => src.Unit != null ? src.Unit.Name : string.Empty))
            .ForMember(dest => dest.CurrentTicketCount, opt => opt.MapFrom(src => 
                src.Tickets != null ? src.Tickets.Count(t => t.Status == Domain.Enums.TicketStatus.Waiting) : 0))
            .ForMember(dest => dest.IsAtCapacity, opt => opt.MapFrom(src => 
                src.Tickets != null && src.Tickets.Count(t => t.Status == Domain.Enums.TicketStatus.Waiting) >= src.MaxCapacity))
            .ForMember(dest => dest.IsAcceptingTickets, opt => opt.MapFrom(src => 
                src.IsActive && src.Status == Domain.Enums.QueueStatus.Open))
            .ForMember(dest => dest.CreatedAt, opt => opt.MapFrom(src => src.CreatedAt))
            .ForMember(dest => dest.UpdatedAt, opt => opt.MapFrom(src => src.UpdatedAt));

        // Queue to QueueStatusDto
        CreateMap<Queue, QueueStatusDto>()
            .ForMember(dest => dest.Id, opt => opt.MapFrom(src => src.Id))
            .ForMember(dest => dest.Name, opt => opt.MapFrom(src => src.Name))
            .ForMember(dest => dest.Status, opt => opt.MapFrom(src => src.Status.ToString()))
            .ForMember(dest => dest.IsActive, opt => opt.MapFrom(src => src.IsActive))
            .ForMember(dest => dest.CurrentTicketCount, opt => opt.MapFrom(src => 
                src.Tickets != null ? src.Tickets.Count(t => t.Status == Domain.Enums.TicketStatus.Waiting) : 0))
            .ForMember(dest => dest.MaxCapacity, opt => opt.MapFrom(src => src.MaxCapacity))
            .ForMember(dest => dest.IsAtCapacity, opt => opt.MapFrom(src => 
                src.Tickets != null && src.Tickets.Count(t => t.Status == Domain.Enums.TicketStatus.Waiting) >= src.MaxCapacity))
            .ForMember(dest => dest.IsAcceptingTickets, opt => opt.MapFrom(src => 
                src.IsActive && src.Status == Domain.Enums.QueueStatus.Open))
            .ForMember(dest => dest.LastUpdated, opt => opt.MapFrom(src => src.UpdatedAt));

        // CreateQueueDto to Queue
        CreateMap<CreateQueueDto, Queue>()
            .ForMember(dest => dest.Id, opt => opt.Ignore())
            .ForMember(dest => dest.Name, opt => opt.MapFrom(src => src.Name))
            .ForMember(dest => dest.Code, opt => opt.MapFrom(src => src.Code))
            .ForMember(dest => dest.DisplayName, opt => opt.MapFrom(src => src.DisplayName))
            .ForMember(dest => dest.MaxCapacity, opt => opt.MapFrom(src => src.MaxCapacity))
            .ForMember(dest => dest.Status, opt => opt.MapFrom(src => Domain.Enums.QueueStatus.Open))
            .ForMember(dest => dest.IsActive, opt => opt.MapFrom(src => true))
            .ForMember(dest => dest.UnitId, opt => opt.MapFrom(src => src.UnitId))
            .ForMember(dest => dest.CreatedAt, opt => opt.MapFrom(src => DateTime.UtcNow))
            .ForMember(dest => dest.UpdatedAt, opt => opt.MapFrom(src => DateTime.UtcNow));

        // UpdateQueueDto to Queue
        CreateMap<UpdateQueueDto, Queue>()
            .ForMember(dest => dest.Id, opt => opt.Ignore())
            .ForMember(dest => dest.Name, opt => opt.Condition(src => !string.IsNullOrEmpty(src.Name)))
            .ForMember(dest => dest.Code, opt => opt.Condition(src => !string.IsNullOrEmpty(src.Code)))
            .ForMember(dest => dest.DisplayName, opt => opt.Condition(src => !string.IsNullOrEmpty(src.DisplayName)))
            .ForMember(dest => dest.MaxCapacity, opt => opt.Condition(src => src.MaxCapacity.HasValue))
            .ForMember(dest => dest.IsActive, opt => opt.Condition(src => src.IsActive.HasValue))
            .ForMember(dest => dest.UpdatedAt, opt => opt.MapFrom(src => DateTime.UtcNow));

        // UpdateQueueStatusDto to Queue
        CreateMap<UpdateQueueStatusDto, Queue>()
            .ForMember(dest => dest.Id, opt => opt.Ignore())
            .ForMember(dest => dest.Status, opt => opt.Condition(src => !string.IsNullOrEmpty(src.Status)))
            .ForMember(dest => dest.IsActive, opt => opt.Condition(src => src.IsActive.HasValue))
            .ForMember(dest => dest.UpdatedAt, opt => opt.MapFrom(src => DateTime.UtcNow));
    }
}