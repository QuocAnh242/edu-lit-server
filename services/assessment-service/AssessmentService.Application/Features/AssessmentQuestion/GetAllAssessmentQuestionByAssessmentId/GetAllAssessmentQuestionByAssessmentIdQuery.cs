using AssessmentService.Application.Abstractions.Messaging;

namespace AssessmentService.Application.Features.AssessmentQuestion.GetAllAssessmentQuestionByAssessmentId
{
    public class GetAllAssessmentQuestionByAssessmentIdQuery : IQuery<List<GetAllAssessmentQuestionByAssessmentIdResponse>>
    {
        public int Id { get; }
        public GetAllAssessmentQuestionByAssessmentIdQuery(int assessmentId)
        {
            Id = assessmentId;
        }
    }
}
