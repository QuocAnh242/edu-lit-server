using AssessmentService.Database.Models;
using AssessmentService.Database.Models.DTOs;

namespace AssessmentService.Service
{
    public interface IAssignmentAttemptService
    {
        Task<ObjectResponse<AssignmentAttempt>> CreateAssignmentAttemptAsync(AssignmentAttempt asse);
        Task<ObjectResponse<AssignmentAttempt>> GetAssignmentAttemptByIdAsync(int id);
        Task<ObjectResponse<IEnumerable<AssignmentAttempt>>> GetAllAssignmentAttemptAsync();
        Task<ObjectResponse<bool>> UpdateAssignmentAttemptAsync(AssignmentAttempt assessment);
        Task<ObjectResponse<bool>> DeleteAssignmentAttemptAsync(int id);
    }
}
