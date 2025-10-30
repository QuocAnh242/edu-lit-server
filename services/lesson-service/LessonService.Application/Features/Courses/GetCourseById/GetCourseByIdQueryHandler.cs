using AutoMapper;
using LessonService.Application.Abstractions.Messaging;
using LessonService.Application.IServices;
using LessonService.Domain.Commons;
using LessonService.Domain.Interfaces;

namespace LessonService.Application.Features.Courses.GetCourseById;

public class GetCourseByIdQueryHandler : IQueryHandler<GetCourseByIdQuery, GetCourseByIdResponse>
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly IMapper _mapper;
    private readonly IRedisService _redisService;

    public GetCourseByIdQueryHandler(IUnitOfWork unitOfWork, IMapper mapper, IRedisService redisService)
    {
        _unitOfWork = unitOfWork;
        _mapper = mapper;
        _redisService = redisService;
    }

    public async Task<ApiResponse<GetCourseByIdResponse>> Handle(GetCourseByIdQuery query, CancellationToken cancellationToken)
    {
        var cacheKey = $"course:{query.Id}";
        var cachedCourse = await _redisService.GetAsync<GetCourseByIdResponse>(cacheKey);
        if (cachedCourse is not null)
        {
            return ApiResponse<GetCourseByIdResponse>.SuccessResponse(cachedCourse, "Get Course Successfully");
        }

        var courseEntity = await _unitOfWork.CourseRepository.GetByIdAsync(query.Id);
        if (courseEntity is null)
        {
            return ApiResponse<GetCourseByIdResponse>.FailureResponse("Course Not Found", 404);
        }

        var course = _mapper.Map<GetCourseByIdResponse>(courseEntity);
        await _redisService.SetAsync(cacheKey, course, TimeSpan.FromMinutes(10));
        
        return ApiResponse<GetCourseByIdResponse>.SuccessResponse(course, "Get Course Successfully");
    }
}


