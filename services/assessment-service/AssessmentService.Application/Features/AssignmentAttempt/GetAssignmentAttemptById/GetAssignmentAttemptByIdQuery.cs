using AssessmentService.Application.Abstractions.Messaging;

namespace AssessmentService.Application.Features.AssignmentAttempt.GetAssignmentAttemptById
{
    public class GetAssignmentAttemptByIdQuery : IQuery<GetAssignmentAttemptByIdResponse>
    {
        public int Id { get; set; }
        public GetAssignmentAttemptByIdQuery(int id)
        {
            Id = id;
        }
    }
}
