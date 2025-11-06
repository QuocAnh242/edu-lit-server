using AssessmentService.Application.Abstractions.Messaging;

namespace AssessmentService.Application.Features.AssessmentAnswer.GetAllAssessmentAnswerByAttemptId
{
    public class GetAllAssessmentAnswerByAttemptIdQuery : IQuery<List<GetAllAssessmentAnswerByAttemptIdResponse>>
    {
        public int Id { get; }
        public GetAllAssessmentAnswerByAttemptIdQuery(int attemptId)
        {
            Id = attemptId;
        }
    }
}
