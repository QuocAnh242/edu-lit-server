using AssessmentService.Dao.DAOs;
using AssessmentService.Db.Models;

namespace AssessmentService.Repository.Imp
{
    public class AssessmentQuestionRepository : IAssessmentQuestionRepository
    {
        private readonly AssessmentQuestionDAO _assessmentQuestionDAO;
        public async Task AddAssessmentQuestionAsync(AssessmentQuestion question) => await _assessmentQuestionDAO.AddAssessmentQuestionAsync(question);

        public async Task<bool> DeleteAssessmentQuestionAsync(int id)
        {
            try
            {
                await _assessmentQuestionDAO.DeleteAssessmentQuestionAsync(id);
                return true;
            }
            catch (Exception ex)
            {
                return false;
            }
        }

        public async Task<IEnumerable<AssessmentQuestion>> GetAllAssessmentQuestionsAsync() => await _assessmentQuestionDAO.GetAllAssessmentQuestionsAsync();

        public async Task<AssessmentQuestion> GetAssessmentQuestionByIdAsync(int id) => await _assessmentQuestionDAO.GetAssessmentQuestionByIdAsync(id);

        public async Task<bool> UpdateAssessmentQuestionAsync(AssessmentQuestion question)
        {
            try
            {
                await _assessmentQuestionDAO.UpdateAssessmentQuestionAsync(question);
                return true;
            }
            catch (Exception ex)
            {
                return false;
            }
        }
    }
}
