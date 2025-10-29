using AssessmentService.Application.Abstractions.Messaging;

namespace AssessmentService.Application.Features.Assessment.GetAssessmentById
{
    public class GetAssessmentByIdQuery : IQuery<GetAssessmentByIdResponse>
    {
        public int Id { get; }
        public GetAssessmentByIdQuery(int id)
        {
            Id = id;
        }
    }
}
