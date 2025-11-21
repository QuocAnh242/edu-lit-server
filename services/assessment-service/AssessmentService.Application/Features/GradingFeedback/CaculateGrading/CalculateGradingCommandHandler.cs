using AssessmentService.Application.Abstractions.Messaging;
using AssessmentService.Application.IServices;
using AssessmentService.Domain.Commons;
using AssessmentService.Domain.Interfaces;
using AutoMapper;

namespace AssessmentService.Application.Features.GradingFeedback.CalculateGrading
{
    public class CalculateGradingCommandHandler : ICommandHandler<CalculateGradingCommand, CalculateGradingResponse>
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;
        private readonly IRedisService _redisService;

        public CalculateGradingCommandHandler(IUnitOfWork unitOfWork, IMapper mapper, IRedisService redisService)
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
            _redisService = redisService;
        }

        public async Task<ObjectResponse<CalculateGradingResponse>> Handle(CalculateGradingCommand command, CancellationToken cancellationToken)
        {
            try
            {
                // 1. Lấy attempt 
                var attempts = await _unitOfWork.AssignmentAttemptRepository
                    .GetAllByAsync(a => a.AttemptsId == command.AttemptId);

                var attempt = attempts.FirstOrDefault();

                if (attempt == null)
                {
                    return ObjectResponse<CalculateGradingResponse>.Response("404", "Attempt not found", null);
                }

                if (attempt.CompletedAt == null)
                {
                    return ObjectResponse<CalculateGradingResponse>.Response("404", "Attempt not completed yet", null);
                }

                // 2. Lấy assessment để biết tổng số câu hỏi
                var assessment = await _unitOfWork.AssessmentRepository.GetByIdAsync(attempt.AssessmentId);
                if (assessment == null)
                {
                    return ObjectResponse<CalculateGradingResponse>.Response("404", "Assessment not found", null);
                }

                // 3. Lấy tất cả câu hỏi trong assessment
                var assessmentQuestions = await _unitOfWork.AssessmentQuestionRepository
                    .GetAllByAsync(aq => aq.AssessmentId == attempt.AssessmentId && aq.IsActive == true);
                
                var totalQuestions = assessmentQuestions.Count;
                if (totalQuestions == 0)
                {
                    return ObjectResponse<CalculateGradingResponse>.Response("404", "No questions found in assessment", null);
                }

                // 4. Lấy tất cả answers của attempt này
                var assessmentAnswers = await _unitOfWork.AssessmentAnswerRepository
                    .GetAllByAsync(a => a.AttemptsId == command.AttemptId);

                // 5. Group answers by question to avoid counting duplicates
                // For each question, check if it has at least one correct answer
                var answersByQuestion = assessmentAnswers
                    .GroupBy(a => a.AssessmentQuestionId)
                    .ToDictionary(g => g.Key, g => g.ToList());

                // Count correct and wrong questions (not answers)
                // A question is correct if it has at least one correct answer
                // A question is wrong if it has answers but none are correct
                var answeredQuestionIds = answersByQuestion.Keys.ToHashSet();
                var correctQuestionCount = 0;
                var wrongQuestionCount = 0;

                foreach (var questionId in answeredQuestionIds)
                {
                    var questionAnswers = answersByQuestion[questionId];
                    // If at least one answer is correct, the question is correct
                    if (questionAnswers.Any(a => a.IsCorrect))
                    {
                        correctQuestionCount++;
                    }
                    else
                    {
                        // Has answers but none are correct
                        wrongQuestionCount++;
                    }
                }

                var unansweredCount = totalQuestions - answeredQuestionIds.Count;
                var correctCount = correctQuestionCount;
                var wrongCount = wrongQuestionCount;

                // Tính phần trăm trên tổng số câu (câu chưa trả lời được tính là sai)
                var correctPercentage = Math.Round((decimal)correctCount / totalQuestions * 100, 2);
                var wrongPercentage = Math.Round((decimal)(wrongCount + unansweredCount) / totalQuestions * 100, 2);

                // Tính điểm trên thang 10 (câu chưa trả lời = 0 điểm, tức là sai)
                var totalScore = Math.Round((decimal)correctCount / totalQuestions * 10, 2);

                // Xác định grade
                var grade = CalculateGrade(totalScore);

                // 4. Kiểm tra xem đã có feedback chưa
                var existingFeedbacks = await _unitOfWork.GradingFeedbackRepository
                    .GetAllByAsync(f => f.AttemptsId == command.AttemptId);

                var existingFeedback = existingFeedbacks.FirstOrDefault();

                Domain.Entities.GradingFeedback feedback;

                if (existingFeedback != null)
                {
                    // Update existing feedback
                    existingFeedback.TotalScore = totalScore;
                    existingFeedback.CorrectCount = correctCount;
                    existingFeedback.WrongCount = wrongCount;
                    existingFeedback.CorrectPercentage = correctPercentage;
                    existingFeedback.WrongPercentage = wrongPercentage;
                    existingFeedback.UpdatedAt = DateTime.UtcNow;

                    _unitOfWork.GradingFeedbackRepository.Update(existingFeedback);
                    feedback = existingFeedback;
                }
                else
                {
                    // Create new feedback
                    feedback = new Domain.Entities.GradingFeedback
                    {
                        AttemptsId = command.AttemptId,
                        TotalScore = totalScore,
                        CorrectCount = correctCount,
                        WrongCount = wrongCount,
                        CorrectPercentage = correctPercentage,
                        WrongPercentage = wrongPercentage,
                        CreatedAt = DateTime.UtcNow,
                        UpdatedAt = DateTime.UtcNow
                    };

                    await _unitOfWork.GradingFeedbackRepository.AddAsync(feedback);
                }

                await _unitOfWork.SaveChangesAsync(cancellationToken);

                // Invalidate cache
                await _redisService.RemoveAsync($"grading:attempt:{command.AttemptId}");

                var response = _mapper.Map<CalculateGradingResponse>(feedback);
                response.Grade = grade;

                return ObjectResponse<CalculateGradingResponse>.SuccessResponse(response);
            }
            catch (Exception e)
            {
                return ObjectResponse<CalculateGradingResponse>.FailureResponse(e);
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
    }
}
