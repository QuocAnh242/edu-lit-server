using AssessmentService.Db.Models;

namespace AssessmentService.Repository
{
    public interface IGradingFeedbackRepository
    {
        Task CreateGradingFeedbackAsync(GradingFeedback gradingFeedback);
        Task<GradingFeedback> GetGradingFeedbackByIdAsync(int id);
        Task<IEnumerable<GradingFeedback>> GetAllGradingFeedbacksAsync();
        Task<bool> UpdateGradingFeedbackAsync(GradingFeedback question);
        Task<bool> DeleteGradingFeedbackAsync(int id);
    }
}
