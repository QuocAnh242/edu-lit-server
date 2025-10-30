using AutoMapper;
using LessonService.Application.Features.Courses.CreateCourse;
using LessonService.Application.Features.Courses.GetCourseById;
using LessonService.Application.Features.Courses.GetPaginationCourses;
using LessonService.Domain.Commons;
using LessonService.Domain.Entities;

namespace LessonService.Application.Mappers;

public class CourseProfile : Profile
{
    public CourseProfile()
    {
        CreateMap<CreateCourseCommand, Course>();
        CreateMap<Course, GetCourseByIdResponse>();
        CreateMap<Course, GetCoursesResponse>();
        CreateMap<PagedResult<Course>, PagedResult<GetCoursesResponse>>()
            .ForMember(dest => dest.Items, opt => opt.MapFrom(src => src.Items));
    }
}

