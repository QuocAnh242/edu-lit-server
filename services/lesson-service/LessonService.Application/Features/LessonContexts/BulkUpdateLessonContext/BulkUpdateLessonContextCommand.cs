using LessonService.Application.Abstractions.Messaging;

namespace LessonService.Application.Features.LessonContexts.BulkUpdateLessonContext;

public record BulkUpdateLessonContextCommand(
    List<LessonContextUpdateItemDto> Items
) : ICommand<BulkUpdateLessonContextResponse>;

public class LessonContextUpdateItemDto
{
    public Guid Id { get; set; }
    public string? LessonTitle { get; set; }
    public string? LessonContent { get; set; }
    public int? Position { get; set; }
    public int? Level { get; set; }
}

public class BulkUpdateLessonContextResponse
{
    public int TotalUpdated { get; set; }
    public List<Guid> UpdatedIds { get; set; } = new();
}

