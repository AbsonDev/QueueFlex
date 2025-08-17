using AutoMapper;
using QueueManagement.Domain.Entities;
using QueueManagement.Api.DTOs.Webhooks;

namespace QueueManagement.Api.Mappings;

/// <summary>
/// Mapping profile for Webhook entity
/// </summary>
public class WebhookMappingProfile : Profile
{
    public WebhookMappingProfile()
    {
        // Webhook to WebhookDto
        CreateMap<Webhook, WebhookDto>()
            .ForMember(dest => dest.Id, opt => opt.MapFrom(src => src.Id))
            .ForMember(dest => dest.Name, opt => opt.MapFrom(src => src.Name))
            .ForMember(dest => dest.Url, opt => opt.MapFrom(src => src.Url))
            .ForMember(dest => dest.Events, opt => opt.MapFrom(src => src.Events))
            .ForMember(dest => dest.IsActive, opt => opt.MapFrom(src => src.IsActive))
            .ForMember(dest => dest.RetryCount, opt => opt.MapFrom(src => src.RetryCount))
            .ForMember(dest => dest.LastTriggeredAt, opt => opt.MapFrom(src => src.LastTriggeredAt))
            .ForMember(dest => dest.CreatedAt, opt => opt.MapFrom(src => src.CreatedAt))
            .ForMember(dest => dest.UpdatedAt, opt => opt.MapFrom(src => src.UpdatedAt));

        // CreateWebhookDto to Webhook
        CreateMap<CreateWebhookDto, Webhook>()
            .ForMember(dest => dest.Id, opt => opt.Ignore())
            .ForMember(dest => dest.Name, opt => opt.MapFrom(src => src.Name))
            .ForMember(dest => dest.Url, opt => opt.MapFrom(src => src.Url))
            .ForMember(dest => dest.Events, opt => opt.MapFrom(src => src.Events))
            .ForMember(dest => dest.IsActive, opt => opt.MapFrom(src => true))
            .ForMember(dest => dest.RetryCount, opt => opt.MapFrom(src => src.RetryCount))
            .ForMember(dest => dest.LastTriggeredAt, opt => opt.MapFrom(src => (DateTime?)null))
            .ForMember(dest => dest.CreatedAt, opt => opt.MapFrom(src => DateTime.UtcNow))
            .ForMember(dest => dest.UpdatedAt, opt => opt.MapFrom(src => DateTime.UtcNow));

        // UpdateWebhookDto to Webhook
        CreateMap<UpdateWebhookDto, Webhook>()
            .ForMember(dest => dest.Id, opt => opt.Ignore())
            .ForMember(dest => dest.Name, opt => opt.Condition(src => !string.IsNullOrEmpty(src.Name)))
            .ForMember(dest => dest.Url, opt => opt.Condition(src => !string.IsNullOrEmpty(src.Url)))
            .ForMember(dest => dest.Events, opt => opt.Condition(src => src.Events != null))
            .ForMember(dest => dest.IsActive, opt => opt.Condition(src => src.IsActive.HasValue))
            .ForMember(dest => dest.RetryCount, opt => opt.Condition(src => src.RetryCount.HasValue))
            .ForMember(dest => dest.UpdatedAt, opt => opt.MapFrom(src => DateTime.UtcNow));
    }
}