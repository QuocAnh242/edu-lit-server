using AutoMapper;
using LessonService.Domain.Commons;
using LessonServiceQuery.Application.Abstractions.Messaging;
using LessonServiceQuery.Application.DTOs;
using LessonServiceQuery.Domain.Repositories;
namespace LessonServiceQuery.Application.Features.Courses.GetAllCourses;
public class GetAllCoursesQueryHandler : IQueryHandler<GetAllCoursesQuery, List<CourseDto>>
{
    private readonly ICourseRepository _repository;
    private readonly IMapper _mapper;
    public GetAllCoursesQueryHandler(ICourseRepository repository, IMapper mapper)
    {
        _repository = repository;
        _mapper = mapper;
    }
    public async Task<ApiResponse<List<CourseDto>>> Handle(GetAllCoursesQuery query, CancellationToken cancellationToken = default)
    {
        var courses = await _repository.GetAllAsync();
        var dtos = _mapper.Map<List<CourseDto>>(courses);
        return ApiResponse<List<CourseDto>>.SuccessResponse(dtos);
    }
}

