namespace LessonService.Api.Requests.Courses;


public class UpdateCourseRequest
{
    public Guid SyllabusId { get; set; }
    public string CourseCode { get; set; } = null!;
    public string Title { get; set; } = null!;
    public string? Description { get; set; }
}