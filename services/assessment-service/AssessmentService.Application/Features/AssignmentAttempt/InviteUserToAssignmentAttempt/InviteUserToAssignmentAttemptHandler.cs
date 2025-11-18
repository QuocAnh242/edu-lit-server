using AssessmentService.Application.Abstractions.Messaging;
using AssessmentService.Application.IServices;
using AssessmentService.Domain.Commons;
using AssessmentService.Domain.Interfaces;

namespace AssessmentService.Application.Features.AssignmentAttempt.InviteUserToAssignmentAttempt
{
    public class InviteUserToAssignmentAttemptHandler : ICommandHandler<InviteUserToAssignmentAttemptCommand, bool>
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IEmailService _emailService;

        public InviteUserToAssignmentAttemptHandler(IUnitOfWork unitOfWork, IEmailService emailService)
        {
            _unitOfWork = unitOfWork;
            _emailService = emailService;
        }

        public async Task<ObjectResponse<bool>> Handle(InviteUserToAssignmentAttemptCommand command, CancellationToken cancellationToken)
        {
            var subject = "You have been invited to an assessment attempt";
            var body = $"""
            Hi,

            You have been invited to start an assessment attempt.

            Attempt ID: {command.AssignmentAttemptId}

            Click the link to start: https://yourfrontend.com/attempt/{command.AssignmentAttemptId}

            Regards,
            Assessment System
            """;

            var existingAssignmentAttempt = await _unitOfWork.AssignmentAttemptRepository.GetByIdAsync(command.AssignmentAttemptId);
            if (existingAssignmentAttempt == null)
            {
                return ObjectResponse<bool>.Response("404", "Assignment Attempt not found", false);
            }

            await _emailService.SendEmailAsync(command.UserEmail, subject, body);

            return ObjectResponse<bool>.SuccessResponse(true);
        }
    }
}
