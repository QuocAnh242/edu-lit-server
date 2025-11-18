using LessonServiceQuery.Application.Abstractions.Messaging;
using LessonServiceQuery.Application.DTOs;
namespace LessonServiceQuery.Application.Features.Courses.GetCourseById;
public record GetCourseByIdQuery(Guid CourseId) : IQuery<CourseDto>;