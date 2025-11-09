using LessonService.Application.Abstractions.Messaging;

namespace LessonService.Application.Features.Sessions.GetSessionById;

public class GetSessionByIdQuery : IQuery<GetSessionByIdResponse>
{
    public Guid Id { get; }
    
    public GetSessionByIdQuery(Guid id)
    {
        Id = id;
    }
}