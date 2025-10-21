using AssessmentService.Database.Models;
using AssessmentService.Database.Models.DTOs;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AssessmentService.Service
{
    public interface IGradingFeedback
    {
        Task<ObjectResponse<GradingFeedback>> CreateGradingFeedbackAsync(GradingFeedback gradingFeedback);
        Task<ObjectResponse<GradingFeedback>> GetGradingFeedbackByIdAsync(int id);
        Task<ObjectResponse<IEnumerable<GradingFeedback>>> GetAllGradingFeedbacksAsync();
        Task<ObjectResponse<bool>> UpdateGradingFeedbackAsync(GradingFeedback question);
        Task<ObjectResponse<bool>> DeleteGradingFeedbackAsync(int id);
    }
}
