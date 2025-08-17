using AutoMapper;
using QueueManagement.Domain.Entities;
using QueueManagement.Api.DTOs.Tickets;

namespace QueueManagement.Api.Mappings;

/// <summary>
/// Mapping profile for Ticket entity
/// </summary>
public class TicketMappingProfile : Profile
{
    public TicketMappingProfile()
    {
        // Ticket to TicketDto
        CreateMap<Ticket, TicketDto>()
            .ForMember(dest => dest.Id, opt => opt.MapFrom(src => src.Id))
            .ForMember(dest => dest.Number, opt => opt.MapFrom(src => src.Number))
            .ForMember(dest => dest.Status, opt => opt.MapFrom(src => src.Status.ToString()))
            .ForMember(dest => dest.Priority, opt => opt.MapFrom(src => src.Priority.ToString()))
            .ForMember(dest => dest.IssuedAt, opt => opt.MapFrom(src => src.IssuedAt))
            .ForMember(dest => dest.CalledAt, opt => opt.MapFrom(src => src.CalledAt))
            .ForMember(dest => dest.StartedAt, opt => opt.MapFrom(src => src.StartedAt))
            .ForMember(dest => dest.CompletedAt, opt => opt.MapFrom(src => src.CompletedAt))
            .ForMember(dest => dest.CustomerName, opt => opt.MapFrom(src => src.CustomerName))
            .ForMember(dest => dest.CustomerDocument, opt => opt.MapFrom(src => src.CustomerDocument))
            .ForMember(dest => dest.CustomerPhone, opt => opt.MapFrom(src => src.CustomerPhone))
            .ForMember(dest => dest.Notes, opt => opt.MapFrom(src => src.Notes))
            .ForMember(dest => dest.CompletionNotes, opt => opt.MapFrom(src => src.CompletionNotes))
            .ForMember(dest => dest.QueueId, opt => opt.MapFrom(src => src.QueueId))
            .ForMember(dest => dest.QueueName, opt => opt.MapFrom(src => src.Queue != null ? src.Queue.Name : string.Empty))
            .ForMember(dest => dest.ServiceId, opt => opt.MapFrom(src => src.ServiceId))
            .ForMember(dest => dest.ServiceName, opt => opt.MapFrom(src => src.Service != null ? src.Service.Name : string.Empty))
            .ForMember(dest => dest.UnitId, opt => opt.MapFrom(src => src.Queue != null ? src.Queue.UnitId : Guid.Empty))
            .ForMember(dest => dest.UnitName, opt => opt.MapFrom(src => src.Queue != null && src.Queue.Unit != null ? src.Queue.Unit.Name : string.Empty))
            .ForMember(dest => dest.WaitingTime, opt => opt.MapFrom(src => 
                src.CalledAt.HasValue ? src.CalledAt.Value - src.IssuedAt : null))
            .ForMember(dest => dest.ServiceTime, opt => opt.MapFrom(src => 
                src.CompletedAt.HasValue && src.StartedAt.HasValue ? src.CompletedAt.Value - src.StartedAt.Value : null))
            .ForMember(dest => dest.CreatedAt, opt => opt.MapFrom(src => src.CreatedAt))
            .ForMember(dest => dest.UpdatedAt, opt => opt.MapFrom(src => src.UpdatedAt));

        // Ticket to TicketStatusDto
        CreateMap<Ticket, TicketStatusDto>()
            .ForMember(dest => dest.Number, opt => opt.MapFrom(src => src.Number))
            .ForMember(dest => dest.Status, opt => opt.MapFrom(src => src.Status.ToString()))
            .ForMember(dest => dest.Priority, opt => opt.MapFrom(src => src.Priority.ToString()))
            .ForMember(dest => dest.QueueName, opt => opt.MapFrom(src => src.Queue != null ? src.Queue.Name : string.Empty))
            .ForMember(dest => dest.ServiceName, opt => opt.MapFrom(src => src.Service != null ? src.Service.Name : string.Empty))
            .ForMember(dest => dest.IssuedAt, opt => opt.MapFrom(src => src.IssuedAt))
            .ForMember(dest => dest.CalledAt, opt => opt.MapFrom(src => src.CalledAt))
            .ForMember(dest => dest.EstimatedWaitingMinutes, opt => opt.MapFrom(src => 
                src.Queue != null ? CalculateEstimatedWaitingTime(src.Queue) : null));

        // CreateTicketDto to Ticket
        CreateMap<CreateTicketDto, Ticket>()
            .ForMember(dest => dest.Id, opt => opt.Ignore())
            .ForMember(dest => dest.Number, opt => opt.MapFrom(src => GenerateTicketNumber()))
            .ForMember(dest => dest.Status, opt => opt.MapFrom(src => Domain.Enums.TicketStatus.Waiting))
            .ForMember(dest => dest.Priority, opt => opt.MapFrom(src => 
                string.IsNullOrEmpty(src.Priority) ? Domain.Enums.TicketPriority.Normal : 
                Enum.Parse<Domain.Enums.TicketPriority>(src.Priority, true)))
            .ForMember(dest => dest.IssuedAt, opt => opt.MapFrom(src => DateTime.UtcNow))
            .ForMember(dest => dest.CustomerName, opt => opt.MapFrom(src => src.CustomerName))
            .ForMember(dest => dest.CustomerDocument, opt => opt.MapFrom(src => src.CustomerDocument))
            .ForMember(dest => dest.CustomerPhone, opt => opt.MapFrom(src => src.CustomerPhone))
            .ForMember(dest => dest.Notes, opt => opt.MapFrom(src => src.Notes))
            .ForMember(dest => dest.QueueId, opt => opt.MapFrom(src => src.QueueId))
            .ForMember(dest => dest.ServiceId, opt => opt.MapFrom(src => src.ServiceId))
            .ForMember(dest => dest.CreatedAt, opt => opt.MapFrom(src => DateTime.UtcNow))
            .ForMember(dest => dest.UpdatedAt, opt => opt.MapFrom(src => DateTime.UtcNow));

        // UpdateTicketDto to Ticket
        CreateMap<UpdateTicketDto, Ticket>()
            .ForMember(dest => dest.Id, opt => opt.Ignore())
            .ForMember(dest => dest.CustomerName, opt => opt.Condition(src => !string.IsNullOrEmpty(src.CustomerName)))
            .ForMember(dest => dest.CustomerDocument, opt => opt.Condition(src => !string.IsNullOrEmpty(src.CustomerDocument)))
            .ForMember(dest => dest.CustomerPhone, opt => opt.Condition(src => !string.IsNullOrEmpty(src.CustomerPhone)))
            .ForMember(dest => dest.Priority, opt => opt.Condition(src => !string.IsNullOrEmpty(src.Priority)))
            .ForMember(dest => dest.Notes, opt => opt.Condition(src => !string.IsNullOrEmpty(src.Notes)))
            .ForMember(dest => dest.UpdatedAt, opt => opt.MapFrom(src => DateTime.UtcNow));

        // CallTicketDto to Ticket
        CreateMap<CallTicketDto, Ticket>()
            .ForMember(dest => dest.Id, opt => opt.Ignore())
            .ForMember(dest => dest.CalledAt, opt => opt.MapFrom(src => DateTime.UtcNow))
            .ForMember(dest => dest.Status, opt => opt.MapFrom(src => Domain.Enums.TicketStatus.Called))
            .ForMember(dest => dest.UpdatedAt, opt => opt.MapFrom(src => DateTime.UtcNow));

        // TransferTicketDto to Ticket
        CreateMap<TransferTicketDto, Ticket>()
            .ForMember(dest => dest.Id, opt => opt.Ignore())
            .ForMember(dest => dest.QueueId, opt => opt.MapFrom(src => src.NewQueueId))
            .ForMember(dest => dest.ServiceId, opt => opt.MapFrom(src => src.NewServiceId))
            .ForMember(dest => dest.UpdatedAt, opt => opt.MapFrom(src => DateTime.UtcNow));
    }

    /// <summary>
    /// Generate a unique ticket number
    /// </summary>
    private static string GenerateTicketNumber()
    {
        // TODO: Implement proper ticket number generation logic
        // This should typically involve:
        // 1. Getting the current date/time
        // 2. Getting the current sequence number for the day
        // 3. Formatting as YYYYMMDD-XXXX
        return DateTime.UtcNow.ToString("yyyyMMdd") + "-" + Guid.NewGuid().ToString("N").Substring(0, 4).ToUpper();
    }

    /// <summary>
    /// Calculate estimated waiting time for a ticket
    /// </summary>
    private static int? CalculateEstimatedWaitingTime(Queue queue)
    {
        // TODO: Implement proper waiting time calculation logic
        // This should typically involve:
        // 1. Getting the average service time for the queue
        // 2. Getting the number of tickets ahead
        // 3. Getting the number of active users
        // 4. Calculating: (tickets ahead * avg service time) / active users
        
        if (queue.Tickets == null || !queue.Tickets.Any())
            return 0;

        var waitingTickets = queue.Tickets.Count(t => t.Status == Domain.Enums.TicketStatus.Waiting);
        var avgServiceTimeMinutes = 15; // Default value, should come from actual data
        var activeUsers = 2; // Default value, should come from actual data

        return waitingTickets * avgServiceTimeMinutes / Math.Max(activeUsers, 1);
    }
}