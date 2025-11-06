using AssessmentService.Application.Abstractions.Messaging;
using AssessmentService.Application.IServices;
using AssessmentService.Domain.Commons;
using AssessmentService.Domain.Interfaces;
using AutoMapper;

namespace AssessmentService.Application.Features.AssessmentQuestion.GetAllAssessmentQuestion
{
    public class GetAllAssessmentQuestionQueryHandler : IQueryHandler<GetAllAssessmentQuestionQuery, List<GetAllAssessmentQuestionResponse>>
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;
        private readonly IRedisService _redisService;
        private const string CacheKey = "assessmentQuestions:all";
        private static readonly TimeSpan CacheExpiry = TimeSpan.FromMinutes(10);
        public GetAllAssessmentQuestionQueryHandler(IUnitOfWork unitOfWork, IMapper mapper, IRedisService redisService)
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
            _redisService = redisService;
        }

        public async Task<ObjectResponse<List<GetAllAssessmentQuestionResponse>>> Handle(GetAllAssessmentQuestionQuery query, CancellationToken cancellationToken)
        {
            try
            {
                // 1. Kiểm tra cache trước
                var cachedAssessmentQuestions = await _redisService.GetAsync<List<GetAllAssessmentQuestionResponse>>(CacheKey);
                if (cachedAssessmentQuestions != null)
                {
                    return ObjectResponse<List<GetAllAssessmentQuestionResponse>>.SuccessResponse(cachedAssessmentQuestions);
                }
                
                //2. Nếu không có cache, query từ database
                var assessmentQuestionEntities = await _unitOfWork.AssessmentQuestionRepository.GetAllAsync();
                var assessmentQuestions = _mapper.Map<List<GetAllAssessmentQuestionResponse>>(assessmentQuestionEntities);
                
                // 3. Lưu vào cache
                await _redisService.SetAsync(CacheKey, assessmentQuestions, CacheExpiry);
                return ObjectResponse<List<GetAllAssessmentQuestionResponse>>.SuccessResponse(assessmentQuestions);
            }
            catch (Exception e)
            {
                return ObjectResponse<List<GetAllAssessmentQuestionResponse>>.FailureResponse(e);
            }
        }
    }
}
