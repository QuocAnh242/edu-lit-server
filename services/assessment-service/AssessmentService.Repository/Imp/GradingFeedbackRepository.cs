using AssessmentService.Dao.DAOs;
using AssessmentService.Db.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AssessmentService.Repository.Imp
{
    public class GradingFeedbackRepository : IGradingFeedbackRepository
    {
        private readonly GradingFeedbackDAO _gradingFeedbackDAO;
        public GradingFeedbackRepository(GradingFeedbackDAO gradingFeedbackDAO)
        {
            _gradingFeedbackDAO = gradingFeedbackDAO;
        }
        public async Task CreateGradingFeedbackAsync(GradingFeedback gradingFeedback) => await _gradingFeedbackDAO.AddGradingFeedbackAsync(gradingFeedback);
        public async Task<bool> DeleteGradingFeedbackAsync(int id)
        {
            try
            {
                await _gradingFeedbackDAO.DeleteGradingFeedbackAsync(id);
                return true;
            }
            catch (Exception ex)
            {
                return false;
            }
        }
        public async Task<IEnumerable<GradingFeedback>> GetAllGradingFeedbacksAsync() => await _gradingFeedbackDAO.GetAllGradingFeedbacksAsync();
        public async Task<GradingFeedback> GetGradingFeedbackByIdAsync(int id) => await _gradingFeedbackDAO.GetGradingFeedbackByIdAsync(id);
        public async Task<bool> UpdateGradingFeedbackAsync(GradingFeedback gradingFeedback)
        {
            try
            {
                await _gradingFeedbackDAO.UpdateGradingFeedbackAsync(gradingFeedback);
                return true;
            }
            catch (Exception ex)
            {
                return false;
            }
        }
    }
}
