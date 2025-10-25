using LessonService.Domain.Enums;

namespace LessonService.Application.Features.Syllabus.GetSyllabusById;

public class GetSyllabusByIdResponse
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