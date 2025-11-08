using AssessmentService.Application.Abstractions.Messaging;
using AssessmentService.Application.IServices;
using AssessmentService.Domain.Commons;
using AssessmentService.Domain.Interfaces;

namespace AssessmentService.Application.Features.Assessment.DeleteAssessment
{
    public class DeleteAssessmentCommandHandler : ICommandHandler<DeleteAssessmentCommand, bool>
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IRedisService _redisService;
        private const string CacheKey = "assessments:all";

        public DeleteAssessmentCommandHandler(IUnitOfWork unitOfWork, IRedisService redisService)
        {
            _unitOfWork = unitOfWork;
            _redisService = redisService;
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
                await _unitOfWork.SaveChangesAsync(cancellationToken);

                // Invalidate cache
                await _redisService.RemoveAsync(CacheKey);
                await _redisService.RemoveAsync($"assessment:{command.AssessmentId}");

                return ObjectResponse<bool>.SuccessResponse(true);
            }
            catch (Exception ex)
            {
                return ObjectResponse<bool>.FailureResponse(ex);
            }
        }
    }
}
