﻿namespace LessonServiceQuery.Application.DTOs;

public class LessonDto
{
    public Guid LessonId { get; set; }
    public Guid SessionId { get; set; }
    public string Title { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public int DurationMinutes { get; set; }
    public int Position { get; set; }
    
    // LessonContexts - flat array (hierarchical with ParentId and Level)
    public List<LessonContextDto> LessonContexts { get; set; } = new();
    
    // Activities - flat array (same level as LessonContexts, sorted after contexts by Position)
    public List<ActivityDto> Activities { get; set; } = new();
    
    public DateTime CreatedAt { get; set; }
    public DateTime UpdatedAt { get; set; }
}