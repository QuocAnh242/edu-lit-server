using AssessmentService.Application.Abstractions.Messaging;
using AssessmentService.Application.IServices;
using AssessmentService.Domain.Commons;
using AssessmentService.Domain.Interfaces;
using AutoMapper;

namespace AssessmentService.Application.Features.AssessmentAnswer.GetAssessmentAnswerById
{
    public class GetAssessmentAnswerByIdQueryHandler : IQueryHandler<GetAssessmentAnswerByIdQuery, GetAssessmentAnswerByIdResponse>
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;
        private readonly IRedisService _redisService;
        private static readonly TimeSpan CacheExpiry = TimeSpan.FromMinutes(10);

        public GetAssessmentAnswerByIdQueryHandler(IUnitOfWork unitOfWork, IMapper mapper, IRedisService redisService)
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
            _redisService = redisService;
        }

        public async Task<ObjectResponse<GetAssessmentAnswerByIdResponse>> Handle(GetAssessmentAnswerByIdQuery query, CancellationToken cancellationToken)
        {
            // using redis to save cache for now
            var cacheKey = $"assessmentAnswer:{query.Id}";
            var cachedAssessmentAnswer = await _redisService.GetAsync<GetAssessmentAnswerByIdResponse>(cacheKey);
            if (cachedAssessmentAnswer is not null)
            {
                return ObjectResponse<GetAssessmentAnswerByIdResponse>.SuccessResponse(cachedAssessmentAnswer);
            }
            var assessmentAnswerEntity = await _unitOfWork.AssessmentAnswerRepository.GetByIdAsync(query.Id);
            if (assessmentAnswerEntity is null)
            {
                return ObjectResponse<GetAssessmentAnswerByIdResponse>.Response("404", "Assessment Answer Not Found", null);
            }
            var assessmentAnswer = _mapper.Map<GetAssessmentAnswerByIdResponse>(assessmentAnswerEntity);
            // set redis cache for now
            await _redisService.SetAsync(cacheKey, assessmentAnswer, CacheExpiry);
            return ObjectResponse<GetAssessmentAnswerByIdResponse>.SuccessResponse(assessmentAnswer);
        }
    }
}
