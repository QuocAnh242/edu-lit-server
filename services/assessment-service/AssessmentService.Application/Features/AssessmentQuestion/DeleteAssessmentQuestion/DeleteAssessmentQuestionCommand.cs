using AssessmentService.Application.Abstractions.Messaging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AssessmentService.Application.Features.AssessmentQuestion.DeleteAssessmentQuestion
{
    public class DeleteAssessmentQuestionCommand : ICommand<bool>
    {
        public int Id { get; }
        public DeleteAssessmentQuestionCommand(int id)
        {
            Id = id;
        }
    }
}
