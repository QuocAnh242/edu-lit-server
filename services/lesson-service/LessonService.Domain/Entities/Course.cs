using System;
using System.Collections.Generic;

namespace LessonService.Domain.Entities;

public partial class Course
{
    public Guid Id { get; set; }

    public Guid SyllabusId { get; set; }

    public string CourseCode { get; set; } = null!;

    public string Title { get; set; } = null!;

    public string? Description { get; set; }

    public DateTime? CreatedAt { get; set; }

    public DateTime? UpdatedAt { get; set; }

    public virtual ICollection<Session> Sessions { get; set; } = new List<Session>();

    public virtual Syllabus Syllabus { get; set; } = null!;
}
