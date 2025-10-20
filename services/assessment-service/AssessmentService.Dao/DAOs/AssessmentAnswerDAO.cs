using AssessmentService.Db.Models;

namespace AssessmentService.Dao.DAOs
{
    public class AssessmentAnswerDAO
    {
        private readonly AssessmentDbContext _context;
        private readonly AssessmentAnswerDAO _instance;
        private AssessmentAnswerDAO(AssessmentDbContext context)
        {
            _context = context;
        }
        public static AssessmentAnswerDAO GetInstance(AssessmentDbContext context)
        {
            return new AssessmentAnswerDAO(context);
        }
        public async Task<AssessmentAnswer> GetAssessmentAnswerByIdAsync(int id)
        {
            return _context.AssessmentAnswers.Find(id);
        }
        public async Task<List<AssessmentAnswer>> GetAllAssessmentAnswersAsync()
        {
            return _context.AssessmentAnswers.ToList();
        }
        
        public async Task AddAssessmentAnswerAsync(AssessmentAnswer assessmentAnswer)
        {
            _context.AssessmentAnswers.Add(assessmentAnswer);
            await _context.SaveChangesAsync();
        }
        public async Task UpdateAssessmentAnswerAsync(AssessmentAnswer assessmentAnswer)
        {
            _context.AssessmentAnswers.Update(assessmentAnswer);
            await _context.SaveChangesAsync();
        }
        public async Task DeleteAssessmentAnswerAsync(int id)
        {
            var assessmentAnswer = await GetAssessmentAnswerByIdAsync(id);
            if (assessmentAnswer != null)
            {
                _context.AssessmentAnswers.Remove(assessmentAnswer);
                await _context.SaveChangesAsync();
            }
        }
    }
}
