using System.Text.Json.Serialization;
using LessonService.Application.Abstractions.Messaging;
using LessonService.Domain.Enums;

namespace LessonService.Application.Features.Syllabus.UpdateSyllabus;

public class UpdateSyllabusCommand : ICommand
{
    public Guid Id { get; set; }
    public string? Title { get; set; }
    public string? AcademicYear { get; set; }
    public Semester? Semester { get; set; }
    public string? Description { get; set; }
    public bool? IsActive { get; set; }
}