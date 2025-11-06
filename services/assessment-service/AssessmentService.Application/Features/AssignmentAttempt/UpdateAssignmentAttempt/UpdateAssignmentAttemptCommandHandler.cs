using AssessmentService.Application.Abstractions.Messaging;
using AssessmentService.Application.IServices;
using AssessmentService.Domain.Commons;
using AssessmentService.Domain.Interfaces;
using AutoMapper;
using FluentValidation;

namespace AssessmentService.Application.Features.AssignmentAttempt.UpdateAssignmentAttempt
{
    public class UpdateAssignmentAttemptCommandHandler : ICommandHandler<UpdateAssignmentAttemptCommand, bool>
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IValidator<UpdateAssignmentAttemptCommand> _validator;
        private readonly IMapper _mapper;
        private readonly IRedisService _redisService;
        private const string CacheKey = "assignmentAttempts:all";

        public UpdateAssignmentAttemptCommandHandler(
            IUnitOfWork unitOfWork,
            IValidator<UpdateAssignmentAttemptCommand> validator,
            IMapper mapper,
            IRedisService redisService)
        {
            _unitOfWork = unitOfWork;
            _validator = validator;
            _mapper = mapper;
            _redisService = redisService;
        }

        public async Task<ObjectResponse<bool>> Handle(UpdateAssignmentAttemptCommand command, CancellationToken cancellationToken)
        {
            // validation
            var validationResult = await _validator.ValidateAsync(command);
            if (!validationResult.IsValid)
            {
                var errors = validationResult.Errors
                    .Select(e => new Error("AssignmentAttempt.Update.Validation", e.ErrorMessage))
                    .ToList();
                return ObjectResponse<bool>.Response("400", errors.First().Message, false);
            }

            var existingAssignmentAttempt = await _unitOfWork.AssignmentAttemptRepository.GetByIdAsync(command.AttemptsId);
            if (existingAssignmentAttempt == null)
            {
                return ObjectResponse<bool>.Response("404", "AssignmentAttempt not found", false);
            }

            var existingAssessment = await _unitOfWork.AssessmentRepository.GetByIdAsync(command.AssessmentId);
            if (existingAssessment == null)
            {
                return ObjectResponse<bool>.Response("404", "Assessment not found", false);
            }

            // sẽ có hàm check valid userId sau khi có service của user
            //

            try
            {
                // map updated fields
                var assignmentAttemptToUpdate = _mapper.Map<Domain.Entities.AssignmentAttempt>(command);
                _unitOfWork.AssignmentAttemptRepository.Update(assignmentAttemptToUpdate);
                await _unitOfWork.SaveChangesAsync();

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
