using AutoMapper;
using LessonService.Application.Abstractions.Messaging;
using LessonService.Application.IServices;
using LessonService.Domain.Commons;
using LessonService.Domain.Interfaces;

namespace LessonService.Application.Features.LessonContexts.GetLessonContextById;

public class GetLessonContextByIdQueryHandler : IQueryHandler<GetLessonContextByIdQuery, GetLessonContextByIdResponse>
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly IMapper _mapper;
    private readonly IRedisService _redisService;

    public GetLessonContextByIdQueryHandler(IUnitOfWork unitOfWork, IMapper mapper, IRedisService redisService)
    {
        _unitOfWork = unitOfWork;
        _mapper = mapper;
        _redisService = redisService;
    }

    public async Task<ApiResponse<GetLessonContextByIdResponse>> Handle(GetLessonContextByIdQuery query, CancellationToken cancellationToken)
    {
        var cacheKey = $"lessoncontext:{query.Id}";
        var cachedLessonContext = await _redisService.GetAsync<GetLessonContextByIdResponse>(cacheKey);
        if (cachedLessonContext is not null)
        {
            return ApiResponse<GetLessonContextByIdResponse>.SuccessResponse(cachedLessonContext, "Get LessonContext Successfully");
        }

        var lessonContextEntity = await _unitOfWork.LessonContextRepository.GetByIdAsync(query.Id);
        if (lessonContextEntity is null)
        {
            return ApiResponse<GetLessonContextByIdResponse>.FailureResponse("LessonContext Not Found", 404);
        }

        var lessonContext = _mapper.Map<GetLessonContextByIdResponse>(lessonContextEntity);
        await _redisService.SetAsync(cacheKey, lessonContext, TimeSpan.FromMinutes(10));
        
        return ApiResponse<GetLessonContextByIdResponse>.SuccessResponse(lessonContext, "Get LessonContext Successfully");
    }
}


