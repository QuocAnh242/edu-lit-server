using AssessmentService.Database.Models;

namespace AssessmentService.Service
{
    public interface IAssignmentAttemptService
    {
        Task<AssignmentAttempt> CreateAssignmentAttemptAsync(AssignmentAttempt asse);
        Task<AssignmentAttempt> GetAssignmentAttemptByIdAsync(int id);
        Task<IEnumerable<AssignmentAttempt>> GetAllAssignmentAttemptAsync();
        Task<bool> UpdateAssignmentAttemptAsync(AssignmentAttempt assessment);
        Task<bool> DeleteAssignmentAttemptAsync(int id);
    }
}
