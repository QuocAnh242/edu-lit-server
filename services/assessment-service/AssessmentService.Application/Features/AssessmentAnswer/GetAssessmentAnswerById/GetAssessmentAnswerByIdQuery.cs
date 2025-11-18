using AssessmentService.Application.Abstractions.Messaging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AssessmentService.Application.Features.AssessmentAnswer.GetAssessmentAnswerById
{
    public class GetAssessmentAnswerByIdQuery : IQuery<GetAssessmentAnswerByIdResponse>
    {
        public int Id { get; }
        public GetAssessmentAnswerByIdQuery(int id)
        {
            Id = id;
        }
    }
}
