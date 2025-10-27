using AssessmentService.Application.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AssessmentService.Application.DTOs.Request
{
    public class AssessmentDTO
    {
        public string CourseId { get; set; } = null!;

        public string CreatorId { get; set; } = null!;

        public string Title { get; set; } = null!;

        public string? Description { get; set; }

        public int TotalQuestions { get; set; }

        public int DurationMinutes { get; set; }

        public string? Status { get; set; } = AssessmentStatus.Private.ToString();
    }
}
