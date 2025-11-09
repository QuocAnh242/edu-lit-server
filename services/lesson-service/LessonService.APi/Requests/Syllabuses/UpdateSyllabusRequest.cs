using LessonService.Domain.Enums;

namespace LessonService.Api.Requests;

public class UpdateSyllabusRequest
{
    public string? Title { get; set; }
    public string? AcademicYear { get; set; }
    public Semester? Semester { get; set; }
    public string? Description { get; set; }
    public bool? IsActive { get; set; }
}