using AutoMapper;
using LessonService.Application.Features.LessonContexts.CreateLessonContext;
using LessonService.Application.Features.LessonContexts.GetLessonContextById;
using LessonService.Application.Features.LessonContexts.GetPaginationLessonContexts;
using LessonService.Domain.Commons;
using LessonService.Domain.Entities;

namespace LessonService.Application.Mappers;

public class LessonContextProfile : Profile
{
    public LessonContextProfile()
    {
        CreateMap<CreateLessonContextCommand, LessonContext>();
        CreateMap<LessonContext, GetLessonContextByIdResponse>();
        CreateMap<LessonContext, GetLessonContextsResponse>();
        CreateMap<PagedResult<LessonContext>, PagedResult<GetLessonContextsResponse>>()
            .ForMember(dest => dest.Items, opt => opt.MapFrom(src => src.Items));
    }
}

