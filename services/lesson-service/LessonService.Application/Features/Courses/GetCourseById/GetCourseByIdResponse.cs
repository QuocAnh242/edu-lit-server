namespace LessonService.Application.Features.Courses.GetCourseById;

public class GetCourseByIdResponse
{
    public Guid Id { get; set; }
    public Guid SyllabusId { get; set; }
    public string CourseCode { get; set; } = null!;
    public string Title { get; set; } = null!;
    public string? Description { get; set; }
    public DateTime? CreatedAt { get; set; }
    public DateTime? UpdatedAt { get; set; }
}

