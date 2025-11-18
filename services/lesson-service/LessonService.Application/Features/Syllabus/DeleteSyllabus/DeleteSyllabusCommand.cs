using LessonService.Application.Abstractions.Messaging;

namespace LessonService.Application.Features.Syllabus.DeleteSyllabus;

public class DeleteSyllabusCommand : ICommand
{
    public Guid Id { get; }
    public DeleteSyllabusCommand(Guid id)
    {
        Id = id;
    }
}