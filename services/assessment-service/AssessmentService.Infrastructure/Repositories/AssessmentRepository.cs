using AssessmentService.Domain.Entities;
using AssessmentService.Domain.Interfaces;
using AssessmentService.Infrastructure.DAO;

namespace AssessmentService.Infrastructure.Repositories
{
    public class AssessmentRepository : IAssessmentRepository
    {
        private readonly AssessmentDAO _assessmentDAO;
        public AssessmentRepository(AssessmentDAO assessmentDAO)
        {
            _assessmentDAO = assessmentDAO;
        }
        public async Task CreateAssessmentAsync(Assessment assessment) => await _assessmentDAO.AddAssessmentAsync(assessment);

        public async Task<bool> DeleteAssessmentAsync(int id)
        {
            try
            {
                await _assessmentDAO.DeleteAssessmentAsync(id);
                return true;
            }
            catch (Exception ex)
            {
                return false;
            }
        }

        public async Task<IEnumerable<Assessment>> GetAllAssessmentsAsync() => await _assessmentDAO.GetAllAssessmentsAsync();

        public async Task<Assessment> GetAssessmentByIdAsync(int id) => await _assessmentDAO.GetAssessmentByIdAsync(id);

        public async Task<bool> UpdateAssessmentAsync(Assessment assessment)
        {
            try
            {
                await _assessmentDAO.UpdateAssessmentAsync(assessment);
                return true;
            }
            catch (Exception ex)
            {
                return false;
            }
        }
    }
}
