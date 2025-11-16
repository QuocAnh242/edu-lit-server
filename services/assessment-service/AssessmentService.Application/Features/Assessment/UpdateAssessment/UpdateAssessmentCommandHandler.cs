using AssessmentService.Application.Abstractions.Messaging;
using AssessmentService.Application.IServices;
using AssessmentService.Domain.Commons;
using AssessmentService.Domain.Interfaces;
using AutoMapper;
using FluentValidation;

namespace AssessmentService.Application.Features.Assessment.UpdateAssessment
{
    public class UpdateAssessmentCommandHandler : ICommandHandler<UpdateAssessmentCommand, bool>
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IValidator<UpdateAssessmentCommand> _updateAssessmentCommandValidator;
        private readonly IMapper _mapper;
        private readonly IRedisService _redisService;
        private const string CacheKey = "assessments:all";

        public UpdateAssessmentCommandHandler(
            IUnitOfWork unitOfWork,
            IValidator<UpdateAssessmentCommand> updateAssessmentCommandValidator,
            IMapper mapper,
            IRedisService redisService)
        {
            _unitOfWork = unitOfWork;
            _updateAssessmentCommandValidator = updateAssessmentCommandValidator;
            _mapper = mapper;
            _redisService = redisService;
        }

        public async Task<ObjectResponse<bool>> Handle(UpdateAssessmentCommand assessmentCommand, CancellationToken cancellationToken)
        {
            // validation
            var validationResult = await _updateAssessmentCommandValidator.ValidateAsync(assessmentCommand, cancellationToken);
            if (!validationResult.IsValid)
            {
                var errors = validationResult.Errors
                    .Select(e => new Error("Assessment.Update.Validation", e.ErrorMessage))
                    .ToList();
                return ObjectResponse<bool>.Response("400", errors.First().Message, false);
            }

            var existingAssessment = await _unitOfWork.AssessmentRepository.GetByIdAsync(assessmentCommand.Id);
            if (existingAssessment == null)
            {
                return ObjectResponse<bool>.Response("404", "Assessment not found", false);
            }

            try
            {
                _mapper.Map(assessmentCommand, existingAssessment);
                _unitOfWork.AssessmentRepository.Update(existingAssessment);
                await _unitOfWork.SaveChangesAsync();

                // Invalidate cache
                await _redisService.RemoveAsync(CacheKey);
                await _redisService.RemoveAsync($"assessment:{assessmentCommand.Id}");

                return ObjectResponse<bool>.SuccessResponse(true);
                //sẽ có hàm commit để tự động thêm vào redis sau khi nhận được thông báo của rabbit, chưa làm liền để test thử cái redis cái đã.
            }
            catch (Exception e)
            {
                return ObjectResponse<bool>.Response("400", e.Message, false);
            }
        }

    }
}
