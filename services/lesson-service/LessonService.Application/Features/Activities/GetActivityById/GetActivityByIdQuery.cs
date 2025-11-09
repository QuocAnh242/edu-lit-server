using LessonService.Application.Abstractions.Messaging;

namespace LessonService.Application.Features.Activities.GetActivityById;

public class GetActivityByIdQuery : IQuery<GetActivityByIdResponse>
{
    public Guid Id { get; }
    
    public GetActivityByIdQuery(Guid id)
    {
        Id = id;
    }
}