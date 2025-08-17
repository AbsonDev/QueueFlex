using AutoMapper;
using QueueManagement.Domain.Entities;
using QueueManagement.Api.DTOs.Sessions;

namespace QueueManagement.Api.Mappings;

/// <summary>
/// Mapping profile for Session entity
/// </summary>
public class SessionMappingProfile : Profile
{
    public SessionMappingProfile()
    {
        // Session to SessionDto
        CreateMap<Session, SessionDto>()
            .ForMember(dest => dest.Id, opt => opt.MapFrom(src => src.Id))
            .ForMember(dest => dest.Status, opt => opt.MapFrom(src => src.Status.ToString()))
            .ForMember(dest => dest.StartedAt, opt => opt.MapFrom(src => src.StartedAt))
            .ForMember(dest => dest.PausedAt, opt => opt.MapFrom(src => src.PausedAt))
            .ForMember(dest => dest.PausedDuration, opt => opt.MapFrom(src => src.PausedDuration))
            .ForMember(dest => dest.CompletedAt, opt => opt.MapFrom(src => src.CompletedAt))
            .ForMember(dest => dest.CustomerRating, opt => opt.MapFrom(src => src.CustomerRating))
            .ForMember(dest => dest.CustomerFeedback, opt => opt.MapFrom(src => src.CustomerFeedback))
            .ForMember(dest => dest.InternalNotes, opt => opt.MapFrom(src => src.InternalNotes))
            .ForMember(dest => dest.TicketId, opt => opt.MapFrom(src => src.TicketId))
            .ForMember(dest => dest.TicketNumber, opt => opt.MapFrom(src => src.Ticket != null ? src.Ticket.Number : string.Empty))
            .ForMember(dest => dest.UserId, opt => opt.MapFrom(src => src.UserId))
            .ForMember(dest => dest.UserName, opt => opt.MapFrom(src => src.User != null ? src.User.Name : string.Empty))
            .ForMember(dest => dest.ResourceId, opt => opt.MapFrom(src => src.ResourceId))
            .ForMember(dest => dest.ResourceName, opt => opt.MapFrom(src => src.Resource != null ? src.Resource.Name : string.Empty))
            .ForMember(dest => dest.TotalDuration, opt => opt.MapFrom(src => CalculateTotalDuration(src)))
            .ForMember(dest => dest.ActiveDuration, opt => opt.MapFrom(src => CalculateActiveDuration(src)))
            .ForMember(dest => dest.CreatedAt, opt => opt.MapFrom(src => src.CreatedAt))
            .ForMember(dest => dest.UpdatedAt, opt => opt.MapFrom(src => src.UpdatedAt));

        // CreateSessionDto to Session
        CreateMap<CreateSessionDto, Session>()
            .ForMember(dest => dest.Id, opt => opt.Ignore())
            .ForMember(dest => dest.Status, opt => opt.MapFrom(src => Domain.Enums.SessionStatus.Active))
            .ForMember(dest => dest.StartedAt, opt => opt.MapFrom(src => DateTime.UtcNow))
            .ForMember(dest => dest.TicketId, opt => opt.MapFrom(src => src.TicketId))
            .ForMember(dest => dest.UserId, opt => opt.MapFrom(src => src.UserId))
            .ForMember(dest => dest.ResourceId, opt => opt.MapFrom(src => src.ResourceId))
            .ForMember(dest => dest.InternalNotes, opt => opt.MapFrom(src => src.InitialNotes))
            .ForMember(dest => dest.CreatedAt, opt => opt.MapFrom(src => DateTime.UtcNow))
            .ForMember(dest => dest.UpdatedAt, opt => opt.MapFrom(src => DateTime.UtcNow));

        // CompleteSessionDto to Session
        CreateMap<CompleteSessionDto, Session>()
            .ForMember(dest => dest.Id, opt => opt.Ignore())
            .ForMember(dest => dest.Status, opt => opt.MapFrom(src => Domain.Enums.SessionStatus.Completed))
            .ForMember(dest => dest.CompletedAt, opt => opt.MapFrom(src => DateTime.UtcNow))
            .ForMember(dest => dest.CustomerRating, opt => opt.Condition(src => src.CustomerRating.HasValue))
            .ForMember(dest => dest.CustomerFeedback, opt => opt.Condition(src => !string.IsNullOrEmpty(src.CustomerFeedback)))
            .ForMember(dest => dest.InternalNotes, opt => opt.Condition(src => !string.IsNullOrEmpty(src.InternalNotes)))
            .ForMember(dest => dest.UpdatedAt, opt => opt.MapFrom(src => DateTime.UtcNow));

        // PauseSessionDto to Session
        CreateMap<PauseSessionDto, Session>()
            .ForMember(dest => dest.Id, opt => opt.Ignore())
            .ForMember(dest => dest.Status, opt => opt.MapFrom(src => Domain.Enums.SessionStatus.Paused))
            .ForMember(dest => dest.PausedAt, opt => opt.MapFrom(src => DateTime.UtcNow))
            .ForMember(dest => dest.InternalNotes, opt => opt.Condition(src => !string.IsNullOrEmpty(src.Notes)))
            .ForMember(dest => dest.UpdatedAt, opt => opt.MapFrom(src => DateTime.UtcNow));

        // ResumeSessionDto to Session
        CreateMap<ResumeSessionDto, Session>()
            .ForMember(dest => dest.Id, opt => opt.Ignore())
            .ForMember(dest => dest.Status, opt => opt.MapFrom(src => Domain.Enums.SessionStatus.Active))
            .ForMember(dest => dest.InternalNotes, opt => opt.Condition(src => !string.IsNullOrEmpty(src.Notes)))
            .ForMember(dest => dest.UpdatedAt, opt => opt.MapFrom(src => DateTime.UtcNow));
    }

    /// <summary>
    /// Calculate total duration of a session
    /// </summary>
    private static TimeSpan CalculateTotalDuration(Session session)
    {
        var endTime = session.CompletedAt ?? DateTime.UtcNow;
        return endTime - session.StartedAt;
    }

    /// <summary>
    /// Calculate active duration of a session (excluding pauses)
    /// </summary>
    private static TimeSpan CalculateActiveDuration(Session session)
    {
        var totalDuration = CalculateTotalDuration(session);
        var pausedDuration = session.PausedDuration ?? TimeSpan.Zero;
        return totalDuration - pausedDuration;
    }
}