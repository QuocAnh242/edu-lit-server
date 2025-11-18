using AssessmentService.Application.Abstractions.Messaging;
using AssessmentService.Application.IServices;
using AssessmentService.Domain.Commons;
using AssessmentService.Domain.Interfaces;
using AutoMapper;

namespace AssessmentService.Application.Features.GradingFeedback.GetGradingFeedback
{
    public class GetGradingFeedbackQueryHandler : IQueryHandler<GetGradingFeedbackQuery, GetGradingFeedbackResponse>
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;
        private readonly IRedisService _redisService;
        private static readonly TimeSpan CacheExpiry = TimeSpan.FromMinutes(10);

        public GetGradingFeedbackQueryHandler(IUnitOfWork unitOfWork, IMapper mapper, IRedisService redisService)
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
            _redisService = redisService;
        }

        public async Task<ObjectResponse<GetGradingFeedbackResponse>> Handle(GetGradingFeedbackQuery query, CancellationToken cancellationToken)
        {
            try
            {
                var cacheKey = $"grading:attempt:{query.Id}";

                // 1. Check cache
                var cachedFeedback = await _redisService.GetAsync<GetGradingFeedbackResponse>(cacheKey);
                if (cachedFeedback != null)
                {
                    return ObjectResponse<GetGradingFeedbackResponse>.SuccessResponse(cachedFeedback);
                }

                // 2. Query từ database
                var feedbacks = await _unitOfWork.GradingFeedbackRepository
                    .GetAllByAsync(f => f.AttemptsId == query.Id);

                var feedback = feedbacks.FirstOrDefault();

                if (feedback == null)
                {
                    return ObjectResponse<GetGradingFeedbackResponse>.Response("404", "Grading feedback not found", null);
                }

                var response = _mapper.Map<GetGradingFeedbackResponse>(feedback);
                response.Grade = CalculateGrade(feedback.TotalScore);
                response.Performance = GetPerformanceText(feedback.TotalScore);

                // 3. Cache result
                await _redisService.SetAsync(cacheKey, response, CacheExpiry);

                return ObjectResponse<GetGradingFeedbackResponse>.SuccessResponse(response);
            }
            catch (Exception e)
            {
                return ObjectResponse<GetGradingFeedbackResponse>.FailureResponse(e);
            }
        }

        private static string CalculateGrade(decimal score)
        {
            return score switch
            {
                >= 9.0m => "A",
                >= 8.0m => "B",
                >= 7.0m => "C",
                >= 5.0m => "D",
                _ => "F"
            };
        }

        private static string GetPerformanceText(decimal score)
        {
            return score switch
            {
                >= 9.0m => "Excellent",
                >= 8.0m => "Very Good",
                >= 7.0m => "Good",
                >= 5.0m => "Pass",
                _ => "Fail"
            };
        }
    }
}
