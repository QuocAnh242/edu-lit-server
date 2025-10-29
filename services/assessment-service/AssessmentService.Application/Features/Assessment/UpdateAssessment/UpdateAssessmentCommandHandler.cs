using AssessmentService.Application.Abstractions.Messaging;
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

        public UpdateAssessmentCommandHandler(IUnitOfWork unitOfWork, IValidator<UpdateAssessmentCommand> updateAssessmentCommandValidator, IMapper mapper)
        {
            _unitOfWork = unitOfWork;
            _updateAssessmentCommandValidator = updateAssessmentCommandValidator;
            _mapper = mapper;
        }

        public async Task<ObjectResponse<bool>> Handle(UpdateAssessmentCommand assessmentCommand, CancellationToken cancellationToken)
        {
            // validation
            var validationResult = await _updateAssessmentCommandValidator.ValidateAsync(assessmentCommand);
            if (!validationResult.IsValid)
            {
                var errors = validationResult.Errors
                    .Select(e => new Error("Assessment.Update.Validation", e.ErrorMessage))
                    .ToList();
                return ObjectResponse<bool>.Response("400", errors.First().Message, false);
            }

            try
            {
                var updatedAssessment = _mapper.Map<Domain.Entities.Assessment>(assessmentCommand);
                _unitOfWork.AssessmentRepository.Update(updatedAssessment);
                await _unitOfWork.SaveChangesAsync();
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
