using AssessmentService.Application.Abstractions.Messaging;
using AssessmentService.Application.IServices;
using AssessmentService.Domain.Commons;
using AssessmentService.Domain.Interfaces;
using FluentValidation;

namespace AssessmentService.Application.Features.AssessmentQuestion.CreateAssessmentQuestions
{
    public class CreateAssessmentQuestionsCommandHandler : ICommandHandler<CreateAssessmentQuestionsCommand, List<int>>
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IValidator<CreateAssessmentQuestionsCommand> _validator;
        private readonly IRedisService _redisService;
        private readonly IQuestionServiceClient _questionServiceClient;
        private const string CacheKey = "assessmentQuestions:all";

        public CreateAssessmentQuestionsCommandHandler(
            IUnitOfWork unitOfWork,
            IValidator<CreateAssessmentQuestionsCommand> validator,
            IRedisService redisService,
            IQuestionServiceClient questionServiceClient)
        {
            _unitOfWork = unitOfWork;
            _validator = validator;
            _redisService = redisService;
            _questionServiceClient = questionServiceClient;
        }

        public async Task<ObjectResponse<List<int>>> Handle(CreateAssessmentQuestionsCommand command, CancellationToken cancellationToken)
        {
            // Validation
            var validationResult = await _validator.ValidateAsync(command, cancellationToken);
            if (!validationResult.IsValid)
            {
                var errors = validationResult.Errors
                    .Select(e => new Error("AssessmentQuestions.Create.Validation", e.ErrorMessage))
                    .ToList();
                return ObjectResponse<List<int>>.Response("400", errors.First().Message, new List<int>());
            }

            // Check if Assessment exists
            var assessment = await _unitOfWork.AssessmentRepository.GetByIdAsync(command.AssessmentId);
            if (assessment == null)
            {
                return ObjectResponse<List<int>>.Response("404", "Assessment not found", new List<int>());
            }

            // Validate QuestionIds với Question Service
            var isValid = await _questionServiceClient.ValidateQuestionIdsAsync(command.QuestionIds, cancellationToken);
            if (!isValid)
            {
                return ObjectResponse<List<int>>.Response("400", "Một hoặc nhiều QuestionId không tồn tại trong Question Service", new List<int>());
            }

            var createdIds = new List<int>();
            var assessmentQuestions = new List<Domain.Entities.AssessmentQuestion>();

            // Get existing questions for this assessment to avoid duplicates
            var existingQuestions = await _unitOfWork.AssessmentQuestionRepository
                .GetAllByAsync(aq => aq.AssessmentId == command.AssessmentId);
            
            var existingQuestionIds = existingQuestions
                .Select(aq => aq.QuestionId)
                .ToHashSet();

            // Create AssessmentQuestion for each QuestionId (skip duplicates)
            foreach (var questionId in command.QuestionIds)
            {
                var questionIdString = questionId.ToString();
                
                // Skip if already exists
                if (existingQuestionIds.Contains(questionIdString))
                {
                    continue;
                }

                var assessmentQuestion = new Domain.Entities.AssessmentQuestion
                {
                    AssessmentId = command.AssessmentId,
                    QuestionId = questionIdString,
                    IsActive = true
                };

                assessmentQuestions.Add(assessmentQuestion);
                existingQuestionIds.Add(questionIdString);
            }

            if (assessmentQuestions.Count == 0)
            {
                return ObjectResponse<List<int>>.Response("400", "Tất cả các câu hỏi đã tồn tại trong assessment này", new List<int>());
            }

            // Add all at once
            await _unitOfWork.AssessmentQuestionRepository.AddRangeAsync(assessmentQuestions);

            try
            {
                await _unitOfWork.SaveChangesAsync();

                // Get created IDs
                createdIds = assessmentQuestions.Select(aq => aq.AssessmentQuestionId).ToList();

                // Invalidate cache
                await _redisService.RemoveAsync(CacheKey);
                await _redisService.RemoveAsync($"assessmentQuestions:assessmentId:{command.AssessmentId}");

                return ObjectResponse<List<int>>.SuccessResponse(createdIds);
            }
            catch (Exception e)
            {
                return ObjectResponse<List<int>>.Response("400", e.Message, new List<int>());
            }
        }
    }
}

