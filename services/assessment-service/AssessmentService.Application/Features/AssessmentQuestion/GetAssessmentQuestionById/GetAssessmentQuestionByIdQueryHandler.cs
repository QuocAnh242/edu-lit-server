using AssessmentService.Application.Abstractions.Messaging;
using AssessmentService.Application.IServices;
using AssessmentService.Domain.Commons;
using AssessmentService.Domain.Interfaces;
using AutoMapper;

namespace AssessmentService.Application.Features.AssessmentQuestion.GetAssessmentQuestionById
{
    public class GetAssessmentQuestionByIdQueryHandler : IQueryHandler<GetAssessmentQuestionByIdQuery, GetAssessmentQuestionByIdResponse>
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;
        private readonly IRedisService _redisService;

        public GetAssessmentQuestionByIdQueryHandler(IUnitOfWork unitOfWork, IMapper mapper, IRedisService redisService)
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
            _redisService = redisService;
        }

        public async Task<ObjectResponse<GetAssessmentQuestionByIdResponse>> Handle(GetAssessmentQuestionByIdQuery query, CancellationToken cancellationToken)
        {
            // Using redis to save cache for now
            var cacheKey = $"assessmentQuestion:{query.Id}";
            var cachedAssessmentQuestion = await _redisService.GetAsync<GetAssessmentQuestionByIdResponse>(cacheKey);
            if (cachedAssessmentQuestion is not null)
            {
                return ObjectResponse<GetAssessmentQuestionByIdResponse>.SuccessResponse(cachedAssessmentQuestion);
            }

            var assQuestEntity = await _unitOfWork.AssessmentQuestionRepository.GetByIdAsync(query.Id);
            if (assQuestEntity is null)
            {
                return ObjectResponse<GetAssessmentQuestionByIdResponse>.Response("404", "Assessment Not Found", null);
            }
            var assessmentQuest = _mapper.Map<GetAssessmentQuestionByIdResponse>(assQuestEntity);

            // set redis cache for now
            await _redisService.SetAsync(cacheKey, assessmentQuest, TimeSpan.FromMinutes(10));

            return ObjectResponse<GetAssessmentQuestionByIdResponse>.SuccessResponse(assessmentQuest);

        }
    }
}
