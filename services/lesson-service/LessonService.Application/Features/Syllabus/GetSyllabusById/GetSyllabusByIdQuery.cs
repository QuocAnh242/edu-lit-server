using LessonService.Application.Abstractions.Messaging;
using LessonService.Domain.Enums;

namespace LessonService.Application.Features.Syllabus.GetSyllabusById;

public class GetSyllabusByIdQuery : IQuery<GetSyllabusByIdResponse>
{
   public Guid Id { get; }
   public GetSyllabusByIdQuery(Guid id)
   {
       Id = id;
   }
}