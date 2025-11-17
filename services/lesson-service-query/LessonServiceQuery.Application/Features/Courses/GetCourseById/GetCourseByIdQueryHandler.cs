using AutoMapper;
using LessonService.Domain.Commons;
using LessonServiceQuery.Application.Abstractions.Messaging;
using LessonServiceQuery.Application.DTOs;
using LessonServiceQuery.Domain.Repositories;
namespace LessonServiceQuery.Application.Features.Courses.GetCourseById;
public class GetCourseByIdQueryHandler : IQueryHandler<GetCourseByIdQuery, CourseDto>
{
    private readonly ICourseRepository _repository;
    private readonly IMapper _mapper;
    public GetCourseByIdQueryHandler(ICourseRepository repository, IMapper mapper)
    {
        _repository = repository;
        _mapper = mapper;
    }
    public async Task<ApiResponse<CourseDto>> Handle(GetCourseByIdQuery query, CancellationToken cancellationToken = default)
    {
        var course = await _repository.GetByIdAsync(query.CourseId);
        if (course == null)
            return ApiResponse<CourseDto>.FailureResponse("Course not found", 404);
        var dto = _mapper.Map<CourseDto>(course);
        return ApiResponse<CourseDto>.SuccessResponse(dto);
    }
}