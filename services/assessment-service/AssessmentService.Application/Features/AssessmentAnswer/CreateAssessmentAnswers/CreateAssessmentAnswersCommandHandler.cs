using AssessmentService.Application.Abstractions.Messaging;
using AssessmentService.Application.IServices;
using AssessmentService.Domain.Commons;
using AssessmentService.Domain.Interfaces;
using FluentValidation;

namespace AssessmentService.Application.Features.AssessmentAnswer.CreateAssessmentAnswers
{
    public class CreateAssessmentAnswersCommandHandler : ICommandHandler<CreateAssessmentAnswersCommand, List<int>>
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IValidator<CreateAssessmentAnswersCommand> _validator;
        private readonly IRedisService _redisService;
        private readonly IQuestionServiceClient _questionServiceClient;
        private const string CacheKey = "assessmentAnswer:all";

        public CreateAssessmentAnswersCommandHandler(
            IUnitOfWork unitOfWork,
            IValidator<CreateAssessmentAnswersCommand> validator,
            IRedisService redisService,
            IQuestionServiceClient questionServiceClient)
        {
            _unitOfWork = unitOfWork;
            _validator = validator;
            _redisService = redisService;
            _questionServiceClient = questionServiceClient;
        }

        public async Task<ObjectResponse<List<int>>> Handle(CreateAssessmentAnswersCommand command, CancellationToken cancellationToken)
        {
            // Validation
            var validationResult = await _validator.ValidateAsync(command, cancellationToken);
            if (!validationResult.IsValid)
            {
                var errors = validationResult.Errors
                    .Select(e => new Error("AssessmentAnswers.Create.Validation", e.Message))
                    .ToList();
                return ObjectResponse<List<int>>.Response("400", errors.First().Message, new List<int>());
            }

            // Check if AssignmentAttempt exists
            var attempt = await _unitOfWork.AssignmentAttemptRepository.GetByIdAsync(command.AttemptsId);
            if (attempt == null)
            {
                return ObjectResponse<List<int>>.Response("404", "AssignmentAttempt not found", new List<int>());
            }

            // Get all AssessmentQuestions for this attempt's assessment
            var assessmentQuestions = await _unitOfWork.AssessmentQuestionRepository
                .GetAllByAsync(aq => aq.AssessmentId == attempt.AssessmentId);

            if (!assessmentQuestions.Any())
            {
                return ObjectResponse<List<int>>.Response("404", "Không tìm thấy câu hỏi nào cho assessment này", new List<int>());
            }

            // Create a mapping from QuestionId (Guid) to AssessmentQuestionId (int)
            var questionIdToAssessmentQuestionIdMap = assessmentQuestions
                .ToDictionary(aq => Guid.Parse(aq.QuestionId), aq => aq.AssessmentQuestionId);

            // Get existing answers for this attempt to avoid duplicates
            var existingAnswers = await _unitOfWork.AssessmentAnswerRepository
                .GetAllByAsync(aa => aa.AttemptsId == command.AttemptsId);

            var existingKeys = existingAnswers
                .Select(aa => (aa.AttemptsId, aa.AssessmentQuestionId))
                .ToHashSet();

            var assessmentAnswers = new List<Domain.Entities.AssessmentAnswer>();
            var createdIds = new List<int>();

            // Process each SelectedOptionId
            foreach (var selectedOptionId in command.SelectedOptionIds)
            {
                // Get QuestionOption from Question Service
                var questionOption = await _questionServiceClient.GetQuestionOptionByIdAsync(selectedOptionId, cancellationToken);
                if (questionOption == null)
                {
                    return ObjectResponse<List<int>>.Response("404", $"QuestionOption với id {selectedOptionId} không tồn tại", new List<int>());
                }

                // Find the AssessmentQuestionId by mapping QuestionId
                if (!questionIdToAssessmentQuestionIdMap.TryGetValue(questionOption.QuestionId, out var assessmentQuestionId))
                {
                    return ObjectResponse<List<int>>.Response("400", $"Question với id {questionOption.QuestionId} không thuộc assessment này", new List<int>());
                }

                var key = (command.AttemptsId, assessmentQuestionId);

                // Skip if already exists
                if (existingKeys.Contains(key))
                {
                    continue;
                }

                var assessmentAnswer = new Domain.Entities.AssessmentAnswer
                {
                    AssessmentQuestionId = assessmentQuestionId,
                    AttemptsId = command.AttemptsId,
                    SelectedOptionId = selectedOptionId.ToString(),
                    IsCorrect = questionOption.IsCorrect
                };

                assessmentAnswers.Add(assessmentAnswer);
                existingKeys.Add(key);
            }

            if (assessmentAnswers.Count == 0)
            {
                return ObjectResponse<List<int>>.Response("400", "Tất cả các câu trả lời đã tồn tại cho attempt này", new List<int>());
            }

            // Add all at once
            await _unitOfWork.AssessmentAnswerRepository.AddRangeAsync(assessmentAnswers);

            try
            {
                await _unitOfWork.SaveChangesAsync();

                // Get created IDs
                createdIds = assessmentAnswers.Select(aa => aa.AnswerId).ToList();

                // Invalidate cache
                await _redisService.RemoveAsync(CacheKey);
                await _redisService.RemoveAsync($"assessmentAnswers:attemptId:{command.AttemptsId}");
                await _redisService.RemoveAsync($"grading:attempt:{command.AttemptsId}");

                return ObjectResponse<List<int>>.SuccessResponse(createdIds);
            }
            catch (Exception e)
            {
                return ObjectResponse<List<int>>.Response("400", e.Message, new List<int>());
            }
        }
    }
}

