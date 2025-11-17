using AssessmentService.Application.Abstractions.Messaging;
using AssessmentService.Application.IServices;
using AssessmentService.Domain.Commons;
using AssessmentService.Domain.Interfaces;
using AutoMapper;

namespace AssessmentService.Application.Features.AssessmentAnswer.GetAllAssessmentAnswer
{
    public class GetAllAssessmentAnswerQueryHandler : IQueryHandler<GetAllAssessmentAnswerQuery, List<GetAllAssessmentAnswerResponse>>
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;
        private readonly IRedisService _redisService;
        private const string CacheKey = "assessmentAnswer:all";
        private static readonly TimeSpan CacheExpiry = TimeSpan.FromMinutes(10);

        public GetAllAssessmentAnswerQueryHandler(IUnitOfWork unitOfWork, IMapper mapper, IRedisService redisService)
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
            _redisService = redisService;
        }

        public async Task<ObjectResponse<List<GetAllAssessmentAnswerResponse>>> Handle(GetAllAssessmentAnswerQuery query, CancellationToken cancellationToken)
        {
            try
            {
                // 1. Kiểm tra cache trước
                var cachedAssessmentAnswers = await _redisService.GetAsync<List<GetAllAssessmentAnswerResponse>>(CacheKey);
                if (cachedAssessmentAnswers != null)
                {
                    return ObjectResponse<List<GetAllAssessmentAnswerResponse>>.SuccessResponse(cachedAssessmentAnswers);
                }
                // 2. Nếu không có cache, query từ database
                var assessmentAnswerEntities = await _unitOfWork.AssessmentAnswerRepository.GetAllAsync();
                var assessmentAnswers = _mapper.Map<List<GetAllAssessmentAnswerResponse>>(assessmentAnswerEntities);
                // 3. Lưu vào cache
                await _redisService.SetAsync(CacheKey, assessmentAnswers, CacheExpiry);
                return ObjectResponse<List<GetAllAssessmentAnswerResponse>>.SuccessResponse(assessmentAnswers);
            }
            catch (Exception e)
            {
                return ObjectResponse<List<GetAllAssessmentAnswerResponse>>.FailureResponse(e);
            }
        }
    }
}
