using AssessmentService.Database.Models;

namespace AssessmentService.Dao.DAOs
{
    public class AssessmentQuestionDAO
    {
        private readonly AssessmentDbContext _context;
        public AssessmentQuestionDAO(AssessmentDbContext context)
        {
            _context = context;
        }
        public static AssessmentQuestionDAO GetInstance(AssessmentDbContext context)
        {
            return new AssessmentQuestionDAO(context);
        }
        public async Task<AssessmentQuestion> GetAssessmentQuestionByIdAsync(int id)
        {
            return await _context.AssessmentQuestions.FindAsync(id);
        }
        public async Task<List<AssessmentQuestion>> GetAllAssessmentQuestionsAsync()
        {
            return _context.AssessmentQuestions.ToList();
        }
        public async Task AddAssessmentQuestionAsync(AssessmentQuestion assessmentQuestion)
        {
            _context.AssessmentQuestions.Add(assessmentQuestion);
            await _context.SaveChangesAsync();
        }
        public async Task UpdateAssessmentQuestionAsync(AssessmentQuestion assessmentQuestion)
        {
            _context.AssessmentQuestions.Update(assessmentQuestion);
            await _context.SaveChangesAsync();
        }
        public async Task DeleteAssessmentQuestionAsync(int id)
        {
            var assessmentQuestion = await GetAssessmentQuestionByIdAsync(id);
            if (assessmentQuestion != null)
            {
                _context.AssessmentQuestions.Remove(assessmentQuestion);
                await _context.SaveChangesAsync();
            }
        }
    }
}
