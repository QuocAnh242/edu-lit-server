namespace LessonService.Api.Requests.Activities;

public class CreateActivityRequest
{
    public Guid SessionId { get; set; }
    public string Title { get; set; } = null!;
    public string? Description { get; set; }
    public string ActivityType { get; set; } = null!;
    public string? Content { get; set; }
    public int? Points { get; set; }
    public int Position { get; set; }
    public bool? IsRequired { get; set; }
}