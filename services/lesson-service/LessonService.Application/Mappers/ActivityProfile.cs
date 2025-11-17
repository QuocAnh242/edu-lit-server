using AutoMapper;
using LessonService.Application.Features.Activities.CreateActivity;
using LessonService.Domain.Entities;

namespace LessonService.Application.Mappers;

public class ActivityProfile : Profile
{
    public ActivityProfile()
    {
        CreateMap<CreateActivityCommand, Activity>();
    }
}

