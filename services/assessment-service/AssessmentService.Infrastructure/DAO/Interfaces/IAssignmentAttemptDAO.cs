using AssessmentService.Domain.Entities;

namespace AssessmentService.Infrastructure.DAO.Interfaces
{
    public interface IAssignmentAttemptDAO
    {
        Task<AssignmentAttempt> GetAssignmentAttemptByIdAsync(int id);
        Task<List<AssignmentAttempt>> GetAllAssignmentAttemptsAsync();
        Task AddAssignmentAttemptAsync(AssignmentAttempt at);
        Task UpdateAssignmentAttemptAsync(AssignmentAttempt at);
        Task DeleteAssignmentAttemptAsync(int id);
    }
}
