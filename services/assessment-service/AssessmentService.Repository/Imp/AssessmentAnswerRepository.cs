using AssessmentService.Dao.DAOs;
using AssessmentService.Db.Models;

namespace AssessmentService.Repository.Imp
{
    public class AssessmentAnswerRepository : IAssessmentAnswerRepository
    {
        private readonly AssessmentAnswerDAO _assessmentAnswerDAO;
        public async Task CreateAssessmentAnswerAsync(AssessmentAnswer assessment) => await _assessmentAnswerDAO.AddAssessmentAnswerAsync(assessment);

        public async Task<bool> DeleteAssessmentAnswerAsync(int id)
        {
            try
            {
                await _assessmentAnswerDAO.DeleteAssessmentAnswerAsync(id);
                return true;
            }
            catch (Exception ex)
            {
                return false;
            }
        }

        public async Task<IEnumerable<AssessmentAnswer>> GetAllAssessmentAnswerAsync() => await _assessmentAnswerDAO.GetAllAssessmentAnswersAsync();

        public async Task<AssessmentAnswer> GetAssessmentAnswerByIdAsync(int id) => await _assessmentAnswerDAO.GetAssessmentAnswerByIdAsync(id);

        public async Task<bool> UpdateAssessmentAnswerAsync(AssessmentAnswer assessment)
        {
            try
            {
                await _assessmentAnswerDAO.UpdateAssessmentAnswerAsync(assessment);
                return true;
            }
            catch (Exception ex)
            {
                return false;
            }
        }
    }
}
