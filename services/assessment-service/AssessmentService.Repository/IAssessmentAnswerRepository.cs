using AssessmentService.Db.Models;

namespace AssessmentService.Repository
{
    public interface IAssessmentAnswerRepository
    {
        Task CreateAssessmentAnswerAsync(AssessmentAnswer assessment);
        Task<AssessmentAnswer> GetAssessmentAnswerByIdAsync(int id);
        Task<IEnumerable<AssessmentAnswer>> GetAllAssessmentAnswerAsync();
        Task<bool> UpdateAssessmentAnswerAsync(AssessmentAnswer assessment);
        Task<bool> DeleteAssessmentAnswerAsync(int id);
    }
}
