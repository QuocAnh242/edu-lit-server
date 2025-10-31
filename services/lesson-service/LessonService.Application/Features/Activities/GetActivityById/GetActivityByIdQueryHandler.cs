using AutoMapper;
using LessonService.Application.Abstractions.Messaging;
using LessonService.Application.IServices;
using LessonService.Domain.Commons;
using LessonService.Domain.Interfaces;

namespace LessonService.Application.Features.Activities.GetActivityById;

public class GetActivityByIdQueryHandler : IQueryHandler<GetActivityByIdQuery, GetActivityByIdResponse>
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly IMapper _mapper;
    private readonly IRedisService _redisService;

    public GetActivityByIdQueryHandler(IUnitOfWork unitOfWork, IMapper mapper, IRedisService redisService)
    {
        _unitOfWork = unitOfWork;
        _mapper = mapper;
        _redisService = redisService;
    }

    public async Task<ApiResponse<GetActivityByIdResponse>> Handle(GetActivityByIdQuery query, CancellationToken cancellationToken)
    {
        var cacheKey = $"activity:{query.Id}";
        var cachedActivity = await _redisService.GetAsync<GetActivityByIdResponse>(cacheKey);
        if (cachedActivity is not null)
        {
            return ApiResponse<GetActivityByIdResponse>.SuccessResponse(cachedActivity, "Get Activity Successfully");
        }

        var activityEntity = await _unitOfWork.ActivityRepository.GetByIdAsync(query.Id);
        if (activityEntity is null)
        {
            return ApiResponse<GetActivityByIdResponse>.FailureResponse("Activity Not Found", 404);
        }

        var activity = _mapper.Map<GetActivityByIdResponse>(activityEntity);
        await _redisService.SetAsync(cacheKey, activity, TimeSpan.FromMinutes(10));
        
        return ApiResponse<GetActivityByIdResponse>.SuccessResponse(activity, "Get Activity Successfully");
    }
}


