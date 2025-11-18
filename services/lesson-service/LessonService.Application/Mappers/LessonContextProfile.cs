using AutoMapper;
using LessonService.Application.Features.LessonContexts.CreateLessonContext;
using LessonService.Domain.Entities;

namespace LessonService.Application.Mappers;

public class LessonContextProfile : Profile
{
    public LessonContextProfile()
    {
        CreateMap<CreateLessonContextCommand, LessonContext>();
    }
}

