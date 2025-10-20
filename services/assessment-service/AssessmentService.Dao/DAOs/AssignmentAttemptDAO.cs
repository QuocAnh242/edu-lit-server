using AssessmentService.Db.Models;

namespace AssessmentService.Dao.DAOs
{
    public class AssignmentAttemptDAO
    {
        private readonly AssessmentDbContext _context;
        private readonly AssignmentAttemptDAO _instance;
        private AssignmentAttemptDAO(AssessmentDbContext context)
        {
            _context = context;
        }
        public static AssignmentAttemptDAO GetInstance(AssessmentDbContext context)
        {
            return new AssignmentAttemptDAO(context);
        }
        public async Task<AssignmentAttempt> GetAssignmentAttemptByIdAsync(int id)
        {
            return _context.AssignmentAttempts.Find(id);
        }
        public async Task<List<AssignmentAttempt>> GetAllAssignmentAttemptsAsync()
        {
            return _context.AssignmentAttempts.ToList();
        }
        public async Task AddAssignmentAttemptAsync(AssignmentAttempt assignmentAttempt)
        {
            _context.AssignmentAttempts.Add(assignmentAttempt);
            await _context.SaveChangesAsync();
        }
        public async Task UpdateAssignmentAttemptAsync(AssignmentAttempt assignmentAttempt)
        {
            _context.AssignmentAttempts.Update(assignmentAttempt);
            await _context.SaveChangesAsync();
        }
        public async Task DeleteAssignmentAttemptAsync(int id)
        {
            var assignmentAttempt = await GetAssignmentAttemptByIdAsync(id);
            if (assignmentAttempt != null)
            {
                _context.AssignmentAttempts.Remove(assignmentAttempt);
                await _context.SaveChangesAsync();
            }
        }
    }
}
