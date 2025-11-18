using LessonService.Application.Abstractions.Messaging;

namespace LessonService.Application.Features.Sessions.DeleteSession;

public class DeleteSessionCommand : ICommand
{
    public Guid Id { get; }
    
    public DeleteSessionCommand(Guid id)
    {
        Id = id;
    }
}