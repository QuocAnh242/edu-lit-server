using AssessmentService.Application.Abstractions.Messaging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

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
