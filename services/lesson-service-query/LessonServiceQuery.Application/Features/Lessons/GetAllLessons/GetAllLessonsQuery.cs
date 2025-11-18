using LessonServiceQuery.Application.Abstractions.Messaging;
using LessonServiceQuery.Application.DTOs;
namespace LessonServiceQuery.Application.Features.Lessons.GetAllLessons;
public record GetAllLessonsQuery : IQuery<List<LessonDto>>;