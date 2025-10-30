using LessonService.Application.Abstractions.Messaging;

namespace LessonService.Application.Features.LessonContexts.GetLessonContextById;

public class GetLessonContextByIdQuery : IQuery<GetLessonContextByIdResponse>
{
    public Guid Id { get; }
    
    public GetLessonContextByIdQuery(Guid id)
    {
        Id = id;
    }
}