using LessonService.Application.Abstractions.Messaging;

namespace LessonService.Application.Features.LessonContexts.CreateBulkLessonContext;

public record CreateBulkLessonContextCommand(
    Guid SessionId,
    List<LessonContextItemDto> LessonContexts
) : ICommand<CreateBulkLessonContextResponse>;

public class LessonContextItemDto
{
    public string LessonTitle { get; set; } = null!;
    public string? LessonContent { get; set; }
    public int Position { get; set; }
    public int Level { get; set; }
}

public class CreateBulkLessonContextResponse
{
    public int TotalCreated { get; set; }
    public List<LessonContextCreatedDto> CreatedItems { get; set; } = new();
}

public class LessonContextCreatedDto
{
    public Guid Id { get; set; }
    public Guid? ParentLessonId { get; set; }
    public string LessonTitle { get; set; } = null!;
    public int Position { get; set; }
    public int Level { get; set; }
}

