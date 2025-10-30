using LessonService.Domain.Enums;

namespace LessonService.Api.Requests;

public class CreateSyllabusRequest
{
    public string Title { get; set; }
    public string AcademicYear { get; set; }
    public Semester Semester { get; set; }
    public string Description { get; set; }
}