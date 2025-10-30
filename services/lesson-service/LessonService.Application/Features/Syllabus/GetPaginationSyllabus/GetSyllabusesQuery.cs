using LessonService.Application.Abstractions.Messaging;
using LessonService.Application.Features.Syllabus.GetPagination;
using LessonService.Application.Features.Syllabus.GetPagination;
using LessonService.Domain.Commons;
using LessonService.Domain.Enums;

namespace LessonService.Application.Features.Syllabus.GetPaginationSyllabus;

public class GetSyllabusesQuery : IQuery<PagedResult<GetSyllabusesResponse>>
{
    public int PageNumber { get; set; } = 1;
    public int PageSize { get; set; } = 10;
    public string? SearchTerm { get; set; }
    public Semester? Semester { get; set; }
    public bool? IsActive { get; set; }
}