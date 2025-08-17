using AutoMapper;
using QueueManagement.Domain.Entities;
using QueueManagement.Api.DTOs.Dashboard;

namespace QueueManagement.Api.Mappings;

/// <summary>
/// Mapping profile for Dashboard entity mappings
/// </summary>
public class DashboardMappingProfile : Profile
{
    public DashboardMappingProfile()
    {
        // Unit to UnitDashboardDto
        CreateMap<Unit, UnitDashboardDto>()
            .ForMember(dest => dest.UnitId, opt => opt.MapFrom(src => src.Id))
            .ForMember(dest => dest.UnitName, opt => opt.MapFrom(src => src.Name))
            .ForMember(dest => dest.IsOpen, opt => opt.MapFrom(src => src.Status == Domain.Enums.UnitStatus.Active))
            .ForMember(dest => dest.ActiveQueues, opt => opt.MapFrom(src => 
                src.Queues != null ? src.Queues.Count(q => q.IsActive) : 0))
            .ForMember(dest => dest.WaitingTickets, opt => opt.MapFrom(src => 
                src.Queues != null ? src.Queues.SelectMany(q => q.Tickets ?? Enumerable.Empty<Ticket>())
                    .Count(t => t.Status == Domain.Enums.TicketStatus.Waiting) : 0))
            .ForMember(dest => dest.ActiveSessions, opt => opt.MapFrom(src => 
                src.Users != null ? src.Users.SelectMany(u => u.Sessions ?? Enumerable.Empty<Session>())
                    .Count(s => s.Status == Domain.Enums.SessionStatus.Active) : 0))
            .ForMember(dest => dest.AvailableUsers, opt => opt.MapFrom(src => 
                src.Users != null ? src.Users.Count(u => u.Status == Domain.Enums.UserStatus.Available) : 0))
            .ForMember(dest => dest.AverageWaitingTimeMinutes, opt => opt.MapFrom(src => 
                CalculateAverageWaitingTime(src)))
            .ForMember(dest => dest.QueueStatuses, opt => opt.MapFrom(src => src.Queues))
            .ForMember(dest => dest.UserStatuses, opt => opt.MapFrom(src => src.Users));

        // Queue to QueueStatusSummaryDto
        CreateMap<Queue, QueueStatusSummaryDto>()
            .ForMember(dest => dest.QueueId, opt => opt.MapFrom(src => src.Id))
            .ForMember(dest => dest.QueueName, opt => opt.MapFrom(src => src.Name))
            .ForMember(dest => dest.Status, opt => opt.MapFrom(src => src.Status.ToString()))
            .ForMember(dest => dest.WaitingTickets, opt => opt.MapFrom(src => 
                src.Tickets != null ? src.Tickets.Count(t => t.Status == Domain.Enums.TicketStatus.Waiting) : 0))
            .ForMember(dest => dest.EstimatedWaitingMinutes, opt => opt.MapFrom(src => 
                CalculateEstimatedWaitingTime(src)))
            .ForMember(dest => dest.IsAcceptingTickets, opt => opt.MapFrom(src => 
                src.IsActive && src.Status == Domain.Enums.QueueStatus.Open));

        // User to UserStatusSummaryDto
        CreateMap<User, UserStatusSummaryDto>()
            .ForMember(dest => dest.UserId, opt => opt.MapFrom(src => src.Id))
            .ForMember(dest => dest.UserName, opt => opt.MapFrom(src => src.Name))
            .ForMember(dest => dest.Status, opt => opt.MapFrom(src => src.Status.ToString()))
            .ForMember(dest => dest.IsAvailable, opt => opt.MapFrom(src => 
                src.Status == Domain.Enums.UserStatus.Available))
            .ForMember(dest => dest.ActiveSessions, opt => opt.MapFrom(src => 
                src.Sessions != null ? src.Sessions.Count(s => s.Status == Domain.Enums.SessionStatus.Active) : 0))
            .ForMember(dest => dest.CurrentSessionDuration, opt => opt.MapFrom(src => 
                CalculateCurrentSessionDuration(src)));
    }

    /// <summary>
    /// Calculate average waiting time for a unit
    /// </summary>
    private static double CalculateAverageWaitingTime(Unit unit)
    {
        if (unit.Queues == null || !unit.Queues.Any())
            return 0;

        var allTickets = unit.Queues.SelectMany(q => q.Tickets ?? Enumerable.Empty<Ticket>());
        var completedTickets = allTickets.Where(t => t.CalledAt.HasValue);

        if (!completedTickets.Any())
            return 0;

        var totalWaitingTime = completedTickets.Sum(t => (t.CalledAt!.Value - t.IssuedAt).TotalMinutes);
        return totalWaitingTime / completedTickets.Count();
    }

    /// <summary>
    /// Calculate estimated waiting time for a queue
    /// </summary>
    private static int? CalculateEstimatedWaitingTime(Queue queue)
    {
        if (queue.Tickets == null || !queue.Tickets.Any())
            return 0;

        var waitingTickets = queue.Tickets.Count(t => t.Status == Domain.Enums.TicketStatus.Waiting);
        var avgServiceTimeMinutes = 15; // Default value, should come from actual data
        var activeUsers = 2; // Default value, should come from actual data

        return waitingTickets * avgServiceTimeMinutes / Math.Max(activeUsers, 1);
    }

    /// <summary>
    /// Calculate current session duration for a user
    /// </summary>
    private static TimeSpan? CalculateCurrentSessionDuration(User user)
    {
        if (user.Sessions == null || !user.Sessions.Any())
            return null;

        var activeSession = user.Sessions.FirstOrDefault(s => s.Status == Domain.Enums.SessionStatus.Active);
        if (activeSession == null)
            return null;

        return DateTime.UtcNow - activeSession.StartedAt;
    }
}