using AutoMapper;
using LessonService.Domain.Commons;
using LessonServiceQuery.Application.Abstractions.Messaging;
using LessonServiceQuery.Application.DTOs;
using LessonServiceQuery.Domain.Repositories;

namespace LessonServiceQuery.Application.Features.Courses.GetCourseBySyllabusId;
public class GetCoursesBySyllabusIdQueryHandler : IQueryHandler<GetCoursesBySyllabusIdQuery, List<CourseDto>>
{
    private readonly ICourseRepository _repository;
    private readonly IMapper _mapper;
    public GetCoursesBySyllabusIdQueryHandler(ICourseRepository repository, IMapper mapper)
    {
        _repository = repository;
        _mapper = mapper;
    }
    public async Task<ApiResponse<List<CourseDto>>> Handle(GetCoursesBySyllabusIdQuery query, CancellationToken cancellationToken = default)
    {
        var courses = await _repository.GetBySyllabusIdAsync(query.SyllabusId);
        var dtos = _mapper.Map<List<CourseDto>>(courses);
        return ApiResponse<List<CourseDto>>.SuccessResponse(dtos);
    }
}