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
        private const string CacheKey = "assessmentAnswer:all";

        public CreateAssessmentAnswerCommandHandler(
            IUnitOfWork unitOfWork,
            IValidator<CreateAssessmentAnswerCommand> createAssessmentCommandValidator,
            IMapper mapper,
            IRedisService redisService)
        {
            _unitOfWork = unitOfWork;
            _createAssessmentCommandValidator = createAssessmentCommandValidator;
            _mapper = mapper;
            _redisService = redisService;
        }

        public async Task<ObjectResponse<int>> Handle(CreateAssessmentAnswerCommand command, CancellationToken cancellationToken)
        {
            // validation
            var validationResult = await _createAssessmentCommandValidator.ValidateAsync(command);
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

            var createdAssessmentAnswer = _mapper.Map<Domain.Entities.AssessmentAnswer>(command);

            // chek answer for the question in the db to set IsCorrect
            var quest =  await _unitOfWork.AssessmentQuestionRepository.GetByIdAsync(command.AssessmentQuestionId);
            if (quest.CorrectAnswer == command.SelectedAnswer)
            {
                createdAssessmentAnswer.IsCorrect = true;
            }
            else
            {
                createdAssessmentAnswer.IsCorrect = false;
            }

            await _unitOfWork.AssessmentAnswerRepository.AddAsync(createdAssessmentAnswer);
            try
            {
                await _unitOfWork.SaveChangesAsync();
                // Invalidate cache
                await _redisService.RemoveAsync(CacheKey);
            }
            catch (Exception e)
            {
                return ObjectResponse<int>.Response("400", e.Message, 0);
            }
            return ObjectResponse<int>.SuccessResponse(createdAssessmentAnswer.AnswerId);
        }
    }
}
