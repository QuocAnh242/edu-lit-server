using AssessmentService.Database.Models;

namespace AssessmentService.Dao.DAOs
{
    public class GradingFeedbackDAO
    {
        private readonly AssessmentDbContext _context;
        private readonly GradingFeedbackDAO _instance;
        public GradingFeedbackDAO(AssessmentDbContext context)
        {
            _context = context;
        }
        public static GradingFeedbackDAO GetInstance(AssessmentDbContext context)
        {
            return new GradingFeedbackDAO(context);
        }
        public async Task<GradingFeedback> GetGradingFeedbackByIdAsync(int id)
        {
            return _context.GradingFeedbacks.Find(id);
        }
        public async Task<List<GradingFeedback>> GetAllGradingFeedbacksAsync()
        {
            return _context.GradingFeedbacks.ToList();
        }
        public async Task AddGradingFeedbackAsync(GradingFeedback gradingFeedback)
        {
            _context.GradingFeedbacks.Add(gradingFeedback);
            await _context.SaveChangesAsync();
        }
        public async Task UpdateGradingFeedbackAsync(GradingFeedback gradingFeedback)
        {
            _context.GradingFeedbacks.Update(gradingFeedback);
            await _context.SaveChangesAsync();
        }
        public async Task DeleteGradingFeedbackAsync(int id)
        {
            var gradingFeedback = await GetGradingFeedbackByIdAsync(id);
            if (gradingFeedback != null)
            {
                _context.GradingFeedbacks.Remove(gradingFeedback);
                await _context.SaveChangesAsync();
            }
        }
    }
}
