using LessonServiceQuery.Application.Abstractions.Messaging;
using LessonServiceQuery.Application.DTOs;

namespace LessonServiceQuery.Application.Features.Courses.GetCourseBySyllabusId;
public record GetCoursesBySyllabusIdQuery(Guid SyllabusId) : IQuery<List<CourseDto>>;