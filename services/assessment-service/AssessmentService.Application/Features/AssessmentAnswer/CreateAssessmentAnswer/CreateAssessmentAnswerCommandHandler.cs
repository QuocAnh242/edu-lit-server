using AssessmentService.Application.Abstractions.Messaging;
using AssessmentService.Application.IServices;
using AssessmentService.Domain.Commons;
using AssessmentService.Domain.Interfaces;
using AutoMapper;
using FluentValidation;

namespace AssessmentService.Application.Features.AssessmentAnswer.CreateAssessmentAnswer
{
    public class CreateAssessmentAnswerCommandHandler : ICommandHandler<CreateAssessmentAnswerCommand, int>
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IValidator<CreateAssessmentAnswerCommand> _createAssessmentCommandValidator;
        private readonly IMapper _mapper;
        private readonly IRedisService _redisService;
        private readonly IQuestionServiceClient _questionServiceClient;
        private const string CacheKey = "assessmentAnswer:all";

        public CreateAssessmentAnswerCommandHandler(
            IUnitOfWork unitOfWork,
            IValidator<CreateAssessmentAnswerCommand> createAssessmentCommandValidator,
            IMapper mapper,
            IRedisService redisService,
            IQuestionServiceClient questionServiceClient)
        {
            _unitOfWork = unitOfWork;
            _createAssessmentCommandValidator = createAssessmentCommandValidator;
            _mapper = mapper;
            _redisService = redisService;
            _questionServiceClient = questionServiceClient;
        }

        public async Task<ObjectResponse<int>> Handle(CreateAssessmentAnswerCommand command, CancellationToken cancellationToken)
        {
            // validation
            var validationResult = await _createAssessmentCommandValidator.ValidateAsync(command, cancellationToken);
            if (!validationResult.IsValid)
            {
                var errors = validationResult.Errors
                    .Select(e => new Error("AssessmentAnswer.Create.Validation", e.ErrorMessage))
                    .ToList();
                return ObjectResponse<int>.Response("400", errors.First().Message, 0);
            }

            var question = await _unitOfWork.AssessmentQuestionRepository.GetByIdAsync(command.AssessmentQuestionId);
            if (question == null)
            {
                return ObjectResponse<int>.Response("404", "AssessmentQuestion not found", 0);
            }

            var attempt = await _unitOfWork.AssignmentAttemptRepository.GetByIdAsync(command.AttemptsId);
            if (attempt == null)
            {
                return ObjectResponse<int>.Response("404", "AssignmentAttempt not found", 0);
            }

            // Validate SelectedOptionId belongs to this question
            var questionOptions = await _questionServiceClient.GetQuestionOptionsByQuestionIdAsync(
                Guid.Parse(question.QuestionId), cancellationToken);

            var selectedOption = questionOptions.FirstOrDefault(opt => opt.QuestionOptionId == command.SelectedOptionId);
            if (selectedOption == null)
            {
                return ObjectResponse<int>.Response("400", "SelectedOptionId does not belong to this question", 0);
            }

            var createdAssessmentAnswer = new Domain.Entities.AssessmentAnswer
            {
                AssessmentQuestionId = command.AssessmentQuestionId,
                AttemptsId = command.AttemptsId,
                SelectedOptionId = command.SelectedOptionId.ToString(),
                IsCorrect = selectedOption.IsCorrect
            };

            await _unitOfWork.AssessmentAnswerRepository.AddAsync(createdAssessmentAnswer);
            try
            {
                await _unitOfWork.SaveChangesAsync();

                // Invalidate cache
                await _redisService.RemoveAsync(CacheKey);
                await _redisService.RemoveAsync($"assessmentAnswers:attemptId:{command.AttemptsId}");
                await _redisService.RemoveAsync($"grading:attempt:{command.AttemptsId}");
            }
            catch (Exception e)
            {
                return ObjectResponse<int>.Response("400", e.Message, 0);
            }
            return ObjectResponse<int>.SuccessResponse(createdAssessmentAnswer.AnswerId);
        }
    }
}
