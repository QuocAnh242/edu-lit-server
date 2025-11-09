using LessonService.Application.Abstractions.Messaging;

namespace LessonService.Application.Features.Activities.UpdateActivity;

public class UpdateActivityCommand : ICommand
{
    public Guid Id { get; set; }
    public string? Title { get; set; }
    public string? Description { get; set; }
    public string? ActivityType { get; set; }
    public string? Content { get; set; }
    public int? Points { get; set; }
    public int? Position { get; set; }
    public bool? IsRequired { get; set; }
}