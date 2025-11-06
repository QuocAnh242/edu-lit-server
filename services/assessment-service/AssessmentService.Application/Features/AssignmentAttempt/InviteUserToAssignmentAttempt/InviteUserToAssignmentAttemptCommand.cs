using AssessmentService.Application.Abstractions.Messaging;

namespace AssessmentService.Application.Features.AssignmentAttempt.InviteUserToAssignmentAttempt
{
    public class InviteUserToAssignmentAttemptCommand : ICommand<bool>
    {
        public string UserEmail { get; }
        public int AssignmentAttemptId { get; }
        public InviteUserToAssignmentAttemptCommand(string userEmail, int assignmentAttemptId)
        {
            UserEmail = userEmail;
            AssignmentAttemptId = assignmentAttemptId;
        }
    }
}
