using AssessmentService.Domain.Entities;
using AssessmentService.Infrastructure.DAO.Interfaces;
using AssessmentService.Infrastructure.Persistance.DBContext;

namespace AssessmentService.Infrastructure.DAO
{
    public class AssignmentAttemptDAO : IAssignmentAttemptDAO
    {
        private readonly AssessmentDbContext _context;
        public AssignmentAttemptDAO(AssessmentDbContext context)
        {
            _context = context;
        }
        public async Task<AssignmentAttempt> GetAssignmentAttemptByIdAsync(int id)
        {
            return _context.AssignmentAttempts.Find(id);
        }
        public async Task<List<AssignmentAttempt>> GetAllAssignmentAttemptsAsync()
        {
            return _context.AssignmentAttempts.ToList();
        }
        public async Task AddAssignmentAttemptAsync(AssignmentAttempt at)
        {
            _context.AssignmentAttempts.Add(at);
            await _context.SaveChangesAsync();
        }
        public async Task UpdateAssignmentAttemptAsync(AssignmentAttempt at)
        {
            _context.AssignmentAttempts.Update(at);
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
