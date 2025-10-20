using AssessmentService.Db.Models;

namespace AssessmentService.Dao.DAOs
{
    public class AssessmentDAO
    {
        private readonly AssessmentDbContext _context;
        private readonly AssessmentDAO _instance;

        private AssessmentDAO(AssessmentDbContext context)
        {
            _context = context;
        }

        public static AssessmentDAO GetInstance(AssessmentDbContext context)
        {
            return new AssessmentDAO(context);
        }

        public async Task<Assessment> GetAssessmentByIdAsync(int id)
        {
            return _context.Assessments.Find(id);
        }

        public async Task<List<Assessment>> GetAllAssessmentsAsync()
        {
            return _context.Assessments.ToList();
        }
        
        public async Task AddAssessmentAsync(Assessment assessment)
        {
            _context.Assessments.Add(assessment);
            await _context.SaveChangesAsync();
        }

        public async Task UpdateAssessmentAsync(Assessment assessment)
        {
            _context.Assessments.Update(assessment);
            await _context.SaveChangesAsync();
        }

        public async Task DeleteAssessmentAsync(int id)
        {
            var assessment = await GetAssessmentByIdAsync(id);
            if (assessment != null)
            {
                _context.Assessments.Remove(assessment);
                await _context.SaveChangesAsync();
            }
        }
    }
}
