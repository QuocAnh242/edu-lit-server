using AssessmentService.Domain.Entities;

namespace AssessmentService.Infrastructure.Dao.Interfaces
{
    public interface IAssessmentDAO
    {
        Task<Assessment> GetAssessmentByIdAsync(int id);
        Task<List<Assessment>> GetAllAssessmentsAsync();
        Task AddAssessmentAsync(Assessment asse);
        Task UpdateAssessmentAsync(Assessment asse);
        Task DeleteAssessmentAsync(int id);
    }
}
