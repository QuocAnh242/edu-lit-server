namespace LessonService.Api.Requests.Activities;

public class UpdateActivityRequest
{
    public string? Title { get; set; }
    public string? Description { get; set; }
    public string? ActivityType { get; set; }
    public string? Content { get; set; }
    public int? Points { get; set; }
    public int? Position { get; set; }
    public bool? IsRequired { get; set; }
}

