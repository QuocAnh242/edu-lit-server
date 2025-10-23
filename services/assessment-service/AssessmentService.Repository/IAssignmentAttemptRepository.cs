using AssessmentService.Database.Models;

namespace AssessmentService.Repository
{
    public interface IAssignmentAttemptRepository
    {
        Task CreateAssignmentAttemptAsync(AssignmentAttempt assessment);
        Task<AssignmentAttempt> GetAssignmentAttemptByIdAsync(int id);
        Task<IEnumerable<AssignmentAttempt>> GetAllAssignmentAttemptAsync();
        Task<bool> UpdateAssignmentAttemptAsync(AssignmentAttempt assessment);
        Task<bool> DeleteAssignmentAttemptAsync(int id);
    }
}
