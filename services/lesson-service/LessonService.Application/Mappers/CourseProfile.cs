using AutoMapper;
using LessonService.Application.Features.Courses.CreateCourse;
using LessonService.Domain.Entities;

namespace LessonService.Application.Mappers;

public class CourseProfile : Profile
{
    public CourseProfile()
    {
        CreateMap<CreateCourseCommand, Course>();
    }
}

