using AssessmentService.Application.Abstractions.Messaging;
using AssessmentService.Domain.Commons;
using AssessmentService.Domain.Interfaces;

namespace AssessmentService.Application.Features.Assessment.DeleteAssessment
{
    public class DeleteAssessmentCommandHandler : ICommandHandler<DeleteAssessmentCommand, bool>
    {
        private readonly IUnitOfWork _unitOfWork;
        public DeleteAssessmentCommandHandler(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }
        public async Task<ObjectResponse<bool>> Handle(DeleteAssessmentCommand command, CancellationToken cancellationToken)
        {
            try
            {
                var assessmentEntity = await _unitOfWork.AssessmentRepository.GetByIdAsync(command.AssessmentId);
                if (assessmentEntity is null)
                {
                    return ObjectResponse<bool>.Response("404", "Assessment Not Found", false);
                }

                _unitOfWork.AssessmentRepository.Remove(assessmentEntity);
                return ObjectResponse<bool>.SuccessResponse(true);
            }
            catch (Exception ex)
            {
                return ObjectResponse<bool>.FailureResponse(ex);
            }
        }
    }
}
