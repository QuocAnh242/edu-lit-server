using System;
using System.Collections.Generic;

namespace LessonService.Domain.Entities;

public partial class Session
{
    public Guid Id { get; set; }

    public Guid CourseId { get; set; }

    public string Title { get; set; } = null!;

    public string? Description { get; set; }

    public int Position { get; set; }

    public int? DurationMinutes { get; set; }

    public DateTime? CreatedAt { get; set; }

    public DateTime? UpdatedAt { get; set; }

    public virtual ICollection<Activity> Activities { get; set; } = new List<Activity>();

    public virtual Course Course { get; set; } = null!;

    public virtual ICollection<LessonContext> LessonContexts { get; set; } = new List<LessonContext>();
}
