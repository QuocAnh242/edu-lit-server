using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AssessmentService.Database.Models.DTOs
{
    public class AssessmentAnswerDTO
    {
        public int AssessmentQuestionId { get; set; }
        public int AttemptsId { get; set; }
        /// <summary>
        /// A, B, C, D mà student chọn
        /// </summary>
        public string SelectedAnswer { get; set; } = null!;
    }
}
