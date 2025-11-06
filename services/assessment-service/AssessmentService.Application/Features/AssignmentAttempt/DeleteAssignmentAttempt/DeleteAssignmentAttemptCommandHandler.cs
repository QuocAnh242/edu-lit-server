using AssessmentService.Application.Abstractions.Messaging;
using AssessmentService.Application.IServices;
using AssessmentService.Domain.Commons;
using AssessmentService.Domain.Interfaces;

namespace AssessmentService.Application.Features.AssignmentAttempt.DeleteAssignmentAttempt
{
    public class DeleteAssignmentAttemptCommandHandler : ICommandHandler<DeleteAssignmentAttemptCommand, bool>
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IRedisService _redisService;
        private const string CacheKey = "assignmentAttempts:all";

        public DeleteAssignmentAttemptCommandHandler(IUnitOfWork unitOfWork, IRedisService redisService)
        {
            _unitOfWork = unitOfWork;
            _redisService = redisService;
        }

        public async Task<ObjectResponse<bool>> Handle(DeleteAssignmentAttemptCommand command, CancellationToken cancellationToken)
        {
            try
            {
                var assignAttemptEntity = await _unitOfWork.AssignmentAttemptRepository.GetByIdAsync(command.Id);
                if (assignAttemptEntity is null)
                {
                    return ObjectResponse<bool>.Response("404", "Assignment Attempt Not Found", false);
                }
                _unitOfWork.AssignmentAttemptRepository.Remove(assignAttemptEntity);

                // Invalidate cache
                await _redisService.RemoveAsync(CacheKey);

                return ObjectResponse<bool>.SuccessResponse(true);
            }
            catch (Exception ex)
            {
                return ObjectResponse<bool>.FailureResponse(ex);
            }
        }
    }
}
