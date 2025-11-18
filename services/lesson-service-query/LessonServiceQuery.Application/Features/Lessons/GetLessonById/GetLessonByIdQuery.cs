using LessonServiceQuery.Application.Abstractions.Messaging;
using LessonServiceQuery.Application.DTOs;
namespace LessonServiceQuery.Application.Features.Lessons.GetLessonById;
public record GetLessonByIdQuery(Guid LessonId) : IQuery<LessonDto>;