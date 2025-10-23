using AssessmentService.Database.Models;

namespace AssessmentService.Service
{
    public interface IGradingFeedbackService
    {
        Task<GradingFeedback> CreateGradingFeedbackAsync(GradingFeedback gradingFeedback);
        Task<GradingFeedback> GetGradingFeedbackByIdAsync(int id);
        Task<IEnumerable<GradingFeedback>> GetAllGradingFeedbacksAsync();
        Task<bool> UpdateGradingFeedbackAsync(GradingFeedback question);
        Task<bool> DeleteGradingFeedbackAsync(int id);
    }
}
