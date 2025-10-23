using AssessmentService.Database.Models;

namespace AssessmentService.Service
{
    public interface IAssessmentService
    {
        Task<Assessment> CreateAsync(Assessment asse);
        Task<Assessment> GetByIdAsync(int id);
        Task<IEnumerable<Assessment>> GetAllAssessmentsAsync();
        Task<bool> UpdateAssessmentAsync(Assessment assessment);
        Task<bool> DeleteAssessmentAsync(int id);
    }
}
