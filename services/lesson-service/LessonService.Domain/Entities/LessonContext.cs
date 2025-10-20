using System;
using System.Collections.Generic;

namespace LessonService.Domain.Entities;

public partial class LessonContext
{
    public Guid Id { get; set; }

    public Guid SessionId { get; set; }

    public Guid? ParentLessonId { get; set; }

    public string LessonTitle { get; set; } = null!;

    public string? LessonContent { get; set; }

    public int Position { get; set; }

    public int? Level { get; set; }

    public DateTime? CreatedAt { get; set; }

    public DateTime? UpdatedAt { get; set; }

    public virtual ICollection<LessonContext> InverseParentLesson { get; set; } = new List<LessonContext>();

    public virtual LessonContext? ParentLesson { get; set; }

    public virtual Session Session { get; set; } = null!;
}
