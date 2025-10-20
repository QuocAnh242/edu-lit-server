using AssessmentService.Database.Models;

namespace AssessmentService.Repository
{
    public interface IAssessmentRepository
    {
        Task CreateAssessmentAsync(Assessment assessment);
        Task<Assessment> GetAssessmentByIdAsync(int id);
        Task<IEnumerable<Assessment>> GetAllAssessmentsAsync();
        Task<bool> UpdateAssessmentAsync(Assessment assessment);
        Task<bool> DeleteAssessmentAsync(int id);
    }
}
