using System;
using System.Collections.Generic;

namespace LessonService.Domain.Entities;

public partial class Activity
{
    public Guid Id { get; set; }

    public Guid SessionId { get; set; }

    public string Title { get; set; } = null!;

    public string? Description { get; set; }

    public string ActivityType { get; set; } = null!;

    public string? Content { get; set; }

    public int? Points { get; set; }

    public int Position { get; set; }

    public bool? IsRequired { get; set; }

    public DateTime? CreatedAt { get; set; }

    public DateTime? UpdatedAt { get; set; }

    public virtual Session Session { get; set; } = null!;
}
