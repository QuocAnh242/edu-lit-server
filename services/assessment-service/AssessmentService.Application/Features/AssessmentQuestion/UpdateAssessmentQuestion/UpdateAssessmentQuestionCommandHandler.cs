using AssessmentService.Application.Abstractions.Messaging;
using AssessmentService.Application.IServices;
using AssessmentService.Domain.Commons;
using AssessmentService.Domain.Entities;
using AssessmentService.Domain.Interfaces;
using AutoMapper;
using FluentValidation;

namespace AssessmentService.Application.Features.AssessmentQuestion.UpdateAssessmentQuestion
{
    public class UpdateAssessmentQuestionCommandHandler : ICommandHandler<UpdateAssessmentQuestionCommand, bool>
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IValidator<UpdateAssessmentQuestionCommand> _updateAssessmentCommandValidator;
        private readonly IMapper _mapper;
        private readonly IRedisService _redisService;
        private const string CacheKey = "assessmentQuestions:all";

        public UpdateAssessmentQuestionCommandHandler(
            IUnitOfWork unitOfWork,
            IValidator<UpdateAssessmentQuestionCommand> updateAssessmentCommandValidator,
            IMapper mapper,
            IRedisService redisService)
        {
            _unitOfWork = unitOfWork;
            _updateAssessmentCommandValidator = updateAssessmentCommandValidator;
            _mapper = mapper;
            _redisService = redisService;
        }

        public async Task<ObjectResponse<bool>> Handle(UpdateAssessmentQuestionCommand request, CancellationToken cancellationToken)
        {
            // validation
            var validationResult = await _updateAssessmentCommandValidator.ValidateAsync(request, cancellationToken);
            if (!validationResult.IsValid)
            {
                var errors = validationResult.Errors
                    .Select(e => new Error("AssessmentQuestion.Update.Validation", e.ErrorMessage))
                    .ToList();
                return ObjectResponse<bool>.Response("400", errors.First().Message, false);
            }

            try
            {
                var existingAssessmentQuestion = await _unitOfWork.AssessmentQuestionRepository.GetByIdAsync(request.AssessmentQuestionId);
                if (existingAssessmentQuestion == null)
                {
                    return ObjectResponse<bool>.Response("404", "AssessmentQuestion not found", false);
                }

                var assessment = await _unitOfWork.AssessmentRepository.GetByIdAsync(request.AssessmentId);
                if (assessment == null)
                {
                    return ObjectResponse<bool>.Response("404", "Assessment not found", false);
                }

                // Map updated fields
                _mapper.Map(request, existingAssessmentQuestion);
                _unitOfWork.AssessmentQuestionRepository.Update(existingAssessmentQuestion);
                await _unitOfWork.SaveChangesAsync();

                // Invalidate cache
                await _redisService.RemoveAsync(CacheKey);
                await _redisService.RemoveAsync($"assessmentQuestions:assessmentId:{request.AssessmentId}");

                await _redisService.RemoveAsync($"assessmentQuestion:{request.AssessmentQuestionId}");

                return ObjectResponse<bool>.SuccessResponse(true);
            }
            catch (Exception e)
            {
                return ObjectResponse<bool>.Response("400", e.Message, false);
            }
        }
    }
}
