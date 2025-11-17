using LessonServiceQuery.Application.Abstractions.Messaging;
using LessonServiceQuery.Application.DTOs;
namespace LessonServiceQuery.Application.Features.Sessions.GetSessionsByCourseId;
public record GetSessionsByCourseIdQuery(Guid CourseId) : IQuery<List<SessionDto>>;