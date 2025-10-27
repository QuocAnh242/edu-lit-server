using AutoMapper;
using LessonService.Application.Abstractions.Messaging;
using LessonService.Application.IServices;
using LessonService.Domain.Commons;
using LessonService.Domain.Interfaces;

namespace LessonService.Application.Features.Syllabus.GetSyllabusById;

public class GetSyllabusByIdQueryHandler : IQueryHandler<GetSyllabusByIdQuery, GetSyllabusByIdResponse>
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly IMapper _mapper;
    private readonly IRedisService _redisService;
    
    public GetSyllabusByIdQueryHandler(IUnitOfWork unitOfWork, IMapper mapper, IRedisService redisService)
    {
        _unitOfWork = unitOfWork;
        _mapper = mapper;
        _redisService = redisService;
    }
    
    public async Task<ApiResponse<GetSyllabusByIdResponse>> Handle(GetSyllabusByIdQuery query, CancellationToken cancellationToken)
    {
        var cacheKey = $"syllabus:{query.Id}";
        var cachedSyllabus = await _redisService.GetAsync<GetSyllabusByIdResponse>(cacheKey);
        if (cachedSyllabus is not null)
        {
            return ApiResponse<GetSyllabusByIdResponse>.SuccessResponse(cachedSyllabus, "Get Syllabus Successfully", 200);
        }

        var syllabysEntity = await _unitOfWork.SyllabusRepository.GetByIdAsync(query.Id);
        if (syllabysEntity is null)
        {
            return ApiResponse<GetSyllabusByIdResponse>.FailureResponse("Syllabus Not Found", 404);
        }
        var syllabus = _mapper.Map<GetSyllabusByIdResponse>(syllabysEntity);
        await _redisService.SetAsync(cacheKey, syllabus, TimeSpan.FromMinutes(10));
        return ApiResponse<GetSyllabusByIdResponse>.SuccessResponse(syllabus, "Get Syllabus Successfully", 200);
    }
}