using AssessmentService.Application.Abstractions.Messaging;

namespace AssessmentService.Application.Features.AssessmentQuestion.GetAssessmentQuestionById
{
    public class GetAssessmentQuestionByIdQuery : IQuery<GetAssessmentQuestionByIdResponse>
    {
        public int Id { get; }
        public GetAssessmentQuestionByIdQuery(int id)
        {
            Id = id;
        }
    }
}
