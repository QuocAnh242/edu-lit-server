using AssessmentService.Database.Models;
using AssessmentService.Repository;

namespace AssessmentService.Service.Imp
{
    public class GradingFeedbackService : IGradingFeedbackService
    {
        private readonly IGradingFeedbackRepository _repo;
        public GradingFeedbackService(IGradingFeedbackRepository repo)
        {
            _repo = repo;
        }

        public Task<GradingFeedback> CreateGradingFeedbackAsync(GradingFeedback gradingFeedback)
        {
            throw new NotImplementedException();
        }

        public Task<bool> DeleteGradingFeedbackAsync(int id)
        {
            throw new NotImplementedException();
        }

        public Task<IEnumerable<GradingFeedback>> GetAllGradingFeedbacksAsync()
        {
            throw new NotImplementedException();
        }

        public Task<GradingFeedback> GetGradingFeedbackByIdAsync(int id)
        {
            throw new NotImplementedException();
        }

        public Task<bool> UpdateGradingFeedbackAsync(GradingFeedback question)
        {
            throw new NotImplementedException();
        }
    }
}
