namespace LessonService.Api.Requests.Courses;

public class CreateCourseRequest
{
    public Guid SyllabusId { get; set; }
    public string CourseCode { get; set; } = null!;
    public string Title { get; set; } = null!;
    public string? Description { get; set; }
}