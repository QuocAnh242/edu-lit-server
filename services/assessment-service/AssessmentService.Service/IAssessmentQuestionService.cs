using AssessmentService.Database.Models;
using AssessmentService.Database.Models.DTOs;

namespace AssessmentService.Service
{
    public interface IAssessmentQuestionService
    {
        Task<ObjectResponse<AssessmentQuestion>> AddAssessmentQuestionAsync(AssessmentQuestion question);
        Task<ObjectResponse<AssessmentQuestion>> GetAssessmentQuestionByIdAsync(int id);
        Task<ObjectResponse<IEnumerable<AssessmentQuestion>>> GetAllAssessmentQuestionsAsync();
        Task<ObjectResponse<bool>> UpdateAssessmentQuestionAsync(AssessmentQuestion question);
        Task<ObjectResponse<bool>> DeleteAssessmentQuestionAsync(int id);
    }
}
