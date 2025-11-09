using AssessmentService.Application.Abstractions.Messaging;
using AssessmentService.Application.IServices;
using AssessmentService.Domain.Commons;
using AssessmentService.Domain.Interfaces;
using AutoMapper;

namespace AssessmentService.Application.Features.AssessmentQuestion.GetAllAssessmentQuestionByAssessmentId
{
    public class GetAllAssessmentQuestionByAssessmentIdQueryHandler : IQueryHandler<GetAllAssessmentQuestionByAssessmentIdQuery, List<GetAllAssessmentQuestionByAssessmentIdResponse>>
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;
        private readonly IRedisService _redisService;
        private static readonly TimeSpan CacheExpiry = TimeSpan.FromMinutes(10);

        public GetAllAssessmentQuestionByAssessmentIdQueryHandler(IUnitOfWork unitOfWork, IMapper mapper, IRedisService redisService)
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
            _redisService = redisService;
        }

        public async Task<ObjectResponse<List<GetAllAssessmentQuestionByAssessmentIdResponse>>> Handle(GetAllAssessmentQuestionByAssessmentIdQuery query, CancellationToken cancellationToken)
        {
            try
            {
                string cacheKey = $"assessmentQuestions:assessmentId:{query.Id}";
                // 1. Kiểm tra cache trước
                var cachedQuestions = await _redisService.GetAsync<List<GetAllAssessmentQuestionByAssessmentIdResponse>>(cacheKey);
                if (cachedQuestions != null)
                {
                    return ObjectResponse<List<GetAllAssessmentQuestionByAssessmentIdResponse>>.SuccessResponse(cachedQuestions);
                }

                // 2. Nếu không có cache, query từ database
                var assessmentQuestions = await _unitOfWork.AssessmentQuestionRepository.GetAllByAsync(x => x.AssessmentId == query.Id);
                var questions = _mapper.Map<List<GetAllAssessmentQuestionByAssessmentIdResponse>>(assessmentQuestions);
                
                // 3. Lưu vào cache
                await _redisService.SetAsync(cacheKey, questions, CacheExpiry);
                return ObjectResponse<List<GetAllAssessmentQuestionByAssessmentIdResponse>>.SuccessResponse(questions);
            }
            catch (Exception e)
            {
                return ObjectResponse<List<GetAllAssessmentQuestionByAssessmentIdResponse>>.FailureResponse(e);
            }
        }
    }
}
