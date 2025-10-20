using AssessmentService.Database.Models;

namespace AssessmentService.Repository
{
    public interface IAssessmentQuestionRepository
    {  
        Task AddAssessmentQuestionAsync(AssessmentQuestion question);
        Task<AssessmentQuestion> GetAssessmentQuestionByIdAsync(int id);
        Task<IEnumerable<AssessmentQuestion>> GetAllAssessmentQuestionsAsync();
        Task<bool> UpdateAssessmentQuestionAsync(AssessmentQuestion question);
        Task<bool> DeleteAssessmentQuestionAsync(int id);
    }
}
