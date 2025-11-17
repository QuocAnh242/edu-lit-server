using AssessmentService.Application.Abstractions.Messaging;

namespace AssessmentService.Application.Features.AssignmentAttempt.DeleteAssignmentAttempt
{
    public class DeleteAssignmentAttemptCommand : ICommand<bool>
    {
        public int Id { get; }
        public DeleteAssignmentAttemptCommand(int id)
        {
            Id = id;
        }
    }
}
