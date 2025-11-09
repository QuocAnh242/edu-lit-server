using AssessmentService.Application.Abstractions.Messaging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AssessmentService.Application.Features.AssessmentQuestion.UpdateAssessmentQuestion
{
    public class UpdateAssessmentQuestionCommand : ICommand<bool>
    {
        public int AssessmentQuestionId { get; set; }
        public int AssessmentId { get; set; }
        public string QuestionId { get; set; } = null!;
        public int OrderNum { get; set; }
        /// <summary>
        /// A, B, C, D
        /// </summary>
        public string CorrectAnswer { get; set; } = null!;
        public bool? IsActive { get; set; }
    }
}
