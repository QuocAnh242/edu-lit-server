using LessonServiceQuery.Application.Abstractions.Messaging;
using LessonServiceQuery.Application.DTOs;
namespace LessonServiceQuery.Application.Features.Syllabuses.GetSyllabusById;
public record GetSyllabusByIdQuery(Guid SyllabusId) : IQuery<SyllabusDto>;