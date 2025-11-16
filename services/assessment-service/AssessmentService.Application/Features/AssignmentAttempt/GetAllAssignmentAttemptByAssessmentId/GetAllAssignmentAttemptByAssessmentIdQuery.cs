using AssessmentService.Application.Abstractions.Messaging;
using AssessmentService.Application.Features.AssignmentAttempt.GetAllAssignmentAttemptByAssessmentId;

namespace AssessmentService.Application.Features.AssignmentAttempt.GetAssignmentAttemptByAssessmentId
{
    public class GetAllAssignmentAttemptByAssessmentIdQuery : IQuery<List<GetAllAssignmentAttemptByAssessmentIdResponse>>
    {
        public int Id { get; set; }
        public GetAllAssignmentAttemptByAssessmentIdQuery(int id)
        {
            Id = id;
        }
    }
}
