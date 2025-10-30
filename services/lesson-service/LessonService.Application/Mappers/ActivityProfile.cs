using AutoMapper;
using LessonService.Application.Features.Activities.CreateActivity;
using LessonService.Application.Features.Activities.GetActivityById;
using LessonService.Application.Features.Activities.GetPaginationActivities;
using LessonService.Domain.Commons;
using LessonService.Domain.Entities;

namespace LessonService.Application.Mappers;

public class ActivityProfile : Profile
{
    public ActivityProfile()
    {
        CreateMap<CreateActivityCommand, Activity>();
        CreateMap<Activity, GetActivityByIdResponse>();
        CreateMap<Activity, GetActivitiesResponse>();
        CreateMap<PagedResult<Activity>, PagedResult<GetActivitiesResponse>>()
            .ForMember(dest => dest.Items, opt => opt.MapFrom(src => src.Items));
    }
}

