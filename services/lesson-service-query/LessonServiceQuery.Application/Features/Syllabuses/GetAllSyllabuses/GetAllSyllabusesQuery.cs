using LessonServiceQuery.Application.Abstractions.Messaging;
using LessonServiceQuery.Application.DTOs;
namespace LessonServiceQuery.Application.Features.Syllabuses.GetAllSyllabuses;
public record GetAllSyllabusesQuery : IQuery<List<SyllabusDto>>;