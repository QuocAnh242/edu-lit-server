using AssessmentService.Application.Abstractions.Messaging;
using AssessmentService.Application.IServices;
using AssessmentService.Domain.Commons;
using AssessmentService.Domain.Interfaces;
using AutoMapper;
using FluentValidation;

namespace AssessmentService.Application.Features.AssessmentAnswer.UpdateAssessmentAnswer
{
    public class UpdateAssessmentAnswerCommandHandler : ICommandHandler<UpdateAssessmentAnswerCommand, bool>
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IValidator<UpdateAssessmentAnswerCommand> _validator;
        private readonly IMapper _mapper;
        private readonly IRedisService _redisService;
        private const string CacheKey = "assessmentAnswer:all";

        public UpdateAssessmentAnswerCommandHandler(IUnitOfWork unitOfWork,
            IValidator<UpdateAssessmentAnswerCommand> validator,
            IMapper mapper,
            IRedisService redisService)
        {
            _unitOfWork = unitOfWork;
            _validator = validator;
            _mapper = mapper;
            _redisService = redisService;
        }

        public async Task<ObjectResponse<bool>> Handle(UpdateAssessmentAnswerCommand command, CancellationToken cancellationToken)
        {
            // validation
            var validationResult = await _validator.ValidateAsync(command, cancellationToken);
            if (!validationResult.IsValid)
            {
                var errors = validationResult.Errors
                    .Select(e => new Error("AssessmentAnswer.Update.Validation", e.ErrorMessage))
                    .ToList();
                return ObjectResponse<bool>.Response("400", errors.First().Message, false);
            }

            var question = await _unitOfWork.AssessmentQuestionRepository.GetByIdAsync(command.AssessmentQuestionId);
            if (question == null)
            {
                return ObjectResponse<bool>.Response("404", "AssessmentQuestion not found", false);
            }

            var attempt = await _unitOfWork.AssignmentAttemptRepository.GetByIdAsync(command.AttemptsId);
            if (attempt == null)
            {
                return ObjectResponse<bool>.Response("404", "AssignmentAttempt not found", false);
            }

            try
            {
                var existingAnswer = await _unitOfWork.AssessmentAnswerRepository.GetByIdAsync(command.AnswerId);
                if (existingAnswer == null)
                {
                    return ObjectResponse<bool>.Response("404", "Assessment answer not found", false);
                }
                
                _mapper.Map(command, existingAnswer);

                // chek answer for the question in the db to set IsCorrect
                existingAnswer.IsCorrect = (question.CorrectAnswer == command.SelectedAnswer);

                _unitOfWork.AssessmentAnswerRepository.Update(existingAnswer);
                await _unitOfWork.SaveChangesAsync();

                // Invalidate cache
                await _redisService.RemoveAsync(CacheKey);
                await _redisService.RemoveAsync($"assessmentAnswers:attemptId:{command.AttemptsId}");
                await _redisService.RemoveAsync($"assessmentAnswer:{command.AnswerId}");
                await _redisService.RemoveAsync($"grading:attempt:{command.AttemptsId}");

                return ObjectResponse<bool>.SuccessResponse(true);
            }
            catch (Exception e)
            {
                return ObjectResponse<bool>.Response("400", e.Message, false);
            }
        }
    }
}
