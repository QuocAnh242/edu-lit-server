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
        
        // LessonContext mapping (now flat, without nested activities)
        CreateMap<LessonContext, LessonContextDto>();
        
        // Lesson mapping - sort contexts and activities by Position
        CreateMap<Lesson, LessonDto>()
            .ForMember(dest => dest.LessonContexts, 
                opt => opt.MapFrom(src => src.LessonContexts.OrderBy(lc => lc.Position)))
            .ForMember(dest => dest.Activities,
                opt => opt.MapFrom(src => src.Activities.OrderBy(a => a.Position)));
        
        // Session mapping
        CreateMap<Session, SessionDto>();
        
        // Course mapping - sort sessions by Position
        CreateMap<Course, CourseDto>()
            .ForMember(dest => dest.Sessions, 
                opt => opt.MapFrom(src => src.Sessions.OrderBy(s => s.Position)));
        
        // Syllabus mapping - sort courses by OrderIndex
        CreateMap<Syllabus, SyllabusDto>()
            .ForMember(dest => dest.Courses, 
                opt => opt.MapFrom(src => src.Courses.OrderBy(c => c.OrderIndex)));
    }
}