using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AssessmentService.Application.Features.AssessmentAnswer.GetAllAssessmentAnswer
{
    public class GetAllAssessmentAnswerResponse
    {
        public int AnswerId { get; set; }

        public int AssessmentQuestionId { get; set; }

        public int AttemptsId { get; set; }

        /// <summary>
        /// A, B, C, D mà student chọn
        /// </summary>
        public string SelectedAnswer { get; set; } = null!;

        public bool IsCorrect { get; set; }

        public DateTime? CreatedAt { get; set; }

    }
}
