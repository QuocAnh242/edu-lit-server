using System;
using System.Collections.Generic;

namespace LessonService.Domain.Entities;

public partial class Syllabus
{
    public Guid Id { get; set; }

    public string Title { get; set; } = null!;

    public string AcademicYear { get; set; } = null!;

    public string Semester { get; set; } = null!;

    public string OwnerId { get; set; } = null!;

    public string? Description { get; set; }

    public bool? IsActive { get; set; }

    public DateTime? CreatedAt { get; set; }

    public DateTime? UpdatedAt { get; set; }

    public virtual ICollection<Course> Courses { get; set; } = new List<Course>();
}
