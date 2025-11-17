using AutoMapper;
using LessonServiceQuery.Application.DTOs;
using LessonServiceQuery.Domain.Entities;
namespace LessonServiceQuery.Application.Mappings;
public class MappingProfile : Profile
{
    public MappingProfile()
    {
        // Activity mapping
        CreateMap<Activity, ActivityDto>();
        // LessonContext mapping - sort activities by OrderIndex
        CreateMap<LessonContext, LessonContextDto>()
            .ForMember(dest => dest.Activities, 
                opt => opt.MapFrom(src => src.Activities.OrderBy(a => a.OrderIndex)));
        // Lesson mapping - sort contexts by OrderIndex
        CreateMap<Lesson, LessonDto>()
            .ForMember(dest => dest.LessonContexts, 
                opt => opt.MapFrom(src => src.LessonContexts.OrderBy(lc => lc.OrderIndex)));
        // Session mapping
        CreateMap<Session, SessionDto>();
        // Course mapping - sort sessions by OrderIndex
        CreateMap<Course, CourseDto>()
            .ForMember(dest => dest.Sessions, 
                opt => opt.MapFrom(src => src.Sessions.OrderBy(s => s.OrderIndex)));
        // Syllabus mapping - sort courses by OrderIndex
        CreateMap<Syllabus, SyllabusDto>()
            .ForMember(dest => dest.Courses, 
                opt => opt.MapFrom(src => src.Courses.OrderBy(c => c.OrderIndex)));
    }
}