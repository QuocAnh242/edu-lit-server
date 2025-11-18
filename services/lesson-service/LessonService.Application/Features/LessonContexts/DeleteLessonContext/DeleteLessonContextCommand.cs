using LessonService.Application.Abstractions.Messaging;

namespace LessonService.Application.Features.LessonContexts.DeleteLessonContext;

public class DeleteLessonContextCommand : ICommand
{
    public Guid Id { get; }
    
    public DeleteLessonContextCommand(Guid id)
    {
        Id = id;
    }
}