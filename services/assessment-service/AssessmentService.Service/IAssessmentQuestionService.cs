using AssessmentService.Database.Models;

namespace AssessmentService.Service
{
    public interface IAssessmentQuestionService
    {
        Task<AssessmentQuestion> AddAsync(AssessmentQuestion question);
        Task<AssessmentQuestion> GetByIdAsync(int id);
        Task<IEnumerable<AssessmentQuestion>> GetAllAsync();
        Task<bool> UpdateAsync(AssessmentQuestion question);
        Task<bool> DeleteAsync(int id);
    }
}
