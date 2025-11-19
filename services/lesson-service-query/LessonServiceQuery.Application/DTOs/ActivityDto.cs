﻿namespace LessonServiceQuery.Application.DTOs;

public class ActivityDto
{
    public Guid ActivityId { get; set; }
    public string Title { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public string ActivityType { get; set; } = string.Empty;
    public string Instructions { get; set; } = string.Empty;
    public int EstimatedTimeMinutes { get; set; }
    public int Position { get; set; }
    public List<string> Materials { get; set; } = new();
    public List<string> Objectives { get; set; } = new();
    public DateTime CreatedAt { get; set; }
    public DateTime UpdatedAt { get; set; }
}