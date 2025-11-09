using AutoMapper;
using LessonService.Application.Abstractions.Messaging;
using LessonService.Application.IServices;
using LessonService.Domain.Commons;
using LessonService.Domain.Interfaces;

namespace LessonService.Application.Features.Sessions.GetSessionById;

public class GetSessionByIdQueryHandler : IQueryHandler<GetSessionByIdQuery, GetSessionByIdResponse>
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly IMapper _mapper;
    private readonly IRedisService _redisService;

    public GetSessionByIdQueryHandler(IUnitOfWork unitOfWork, IMapper mapper, IRedisService redisService)
    {
        _unitOfWork = unitOfWork;
        _mapper = mapper;
        _redisService = redisService;
    }

    public async Task<ApiResponse<GetSessionByIdResponse>> Handle(GetSessionByIdQuery query, CancellationToken cancellationToken)
    {
        var cacheKey = $"session:{query.Id}";
        var cachedSession = await _redisService.GetAsync<GetSessionByIdResponse>(cacheKey);
        if (cachedSession is not null)
        {
            return ApiResponse<GetSessionByIdResponse>.SuccessResponse(cachedSession, "Get Session Successfully");
        }

        var sessionEntity = await _unitOfWork.SessionRepository.GetByIdAsync(query.Id);
        if (sessionEntity is null)
        {
            return ApiResponse<GetSessionByIdResponse>.FailureResponse("Session Not Found", 404);
        }

        var session = _mapper.Map<GetSessionByIdResponse>(sessionEntity);
        await _redisService.SetAsync(cacheKey, session, TimeSpan.FromMinutes(10));
        
        return ApiResponse<GetSessionByIdResponse>.SuccessResponse(session, "Get Session Successfully");
    }
}


