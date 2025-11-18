using LessonService.Application.Abstractions.Messaging;

namespace LessonService.Application.Features.Activities.DeleteActivity;

public class DeleteActivityCommand : ICommand
{
    public Guid Id { get; }
    
    public DeleteActivityCommand(Guid id)
    {
        Id = id;
    }
}