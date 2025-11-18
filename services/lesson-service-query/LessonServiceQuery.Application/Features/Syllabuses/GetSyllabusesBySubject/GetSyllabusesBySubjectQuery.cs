using LessonServiceQuery.Application.Abstractions.Messaging;
using LessonServiceQuery.Application.DTOs;
namespace LessonServiceQuery.Application.Features.Syllabuses.GetSyllabusesBySubject;
public record GetSyllabusesBySubjectQuery(string Subject) : IQuery<List<SyllabusDto>>;