using LessonService.Domain.Enums;

namespace LessonService.Application.Features.Syllabus.GetPaginationSyllabus;

public class GetSyllabusesResponse
{
    public Guid Id { get; set; }
    public string Title { get; set; } = null!;
    public string AcademicYear { get; set; } = null!;
    public Semester Semester { get; set; }
    public string OwnerId { get; set; } = null!;
    public string? Description { get; set; }
    public bool? IsActive { get; set; }
    public DateTime? CreatedAt { get; set; }
    public DateTime? UpdatedAt { get; set; }
}


