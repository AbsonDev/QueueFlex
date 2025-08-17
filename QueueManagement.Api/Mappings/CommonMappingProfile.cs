using AutoMapper;
using QueueManagement.Domain.Entities;
using QueueManagement.Api.DTOs.Common;

namespace QueueManagement.Api.Mappings;

/// <summary>
/// Common mapping profile for basic entity mappings
/// </summary>
public class CommonMappingProfile : Profile
{
    public CommonMappingProfile()
    {
        // Base entity mappings
        CreateMap<BaseEntity, object>()
            .ForMember(dest => dest.Id, opt => opt.MapFrom(src => src.Id))
            .ForMember(dest => dest.CreatedAt, opt => opt.MapFrom(src => src.CreatedAt))
            .ForMember(dest => dest.UpdatedAt, opt => opt.MapFrom(src => src.UpdatedAt));

        // Pagination mappings
        CreateMap<PaginationRequestDto, object>()
            .ForMember(dest => dest.Page, opt => opt.MapFrom(src => src.Page))
            .ForMember(dest => dest.PageSize, opt => opt.MapFrom(src => src.PageSize))
            .ForMember(dest => dest.SortBy, opt => opt.MapFrom(src => src.SortBy))
            .ForMember(dest => dest.SortDirection, opt => opt.MapFrom(src => src.SortDirection))
            .ForMember(dest => dest.SearchTerm, opt => opt.MapFrom(src => src.SearchTerm))
            .ForMember(dest => dest.Filters, opt => opt.MapFrom(src => src.Filters));

        // Filter mappings
        CreateMap<BaseFilterDto, object>()
            .ForMember(dest => dest.SearchTerm, opt => opt.MapFrom(src => src.SearchTerm))
            .ForMember(dest => dest.IncludeDeleted, opt => opt.MapFrom(src => src.IncludeDeleted))
            .ForMember(dest => dest.StartDate, opt => opt.MapFrom(src => src.StartDate))
            .ForMember(dest => dest.EndDate, opt => opt.MapFrom(src => src.EndDate));
    }
}