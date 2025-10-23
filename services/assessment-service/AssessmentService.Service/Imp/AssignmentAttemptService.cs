using AssessmentService.Database.Models;
using AssessmentService.Repository;

namespace AssessmentService.Service.Imp
{
    public class AssignmentAttemptService : IAssignmentAttemptService
    {
        private readonly IAssignmentAttemptRepository _repo;
        public AssignmentAttemptService(IAssignmentAttemptRepository repo)
        {
            _repo = repo;
        }
        public Task<AssignmentAttempt> CreateAssignmentAttemptAsync(AssignmentAttempt asse)
        {
            throw new NotImplementedException();
        }

        public Task<bool> DeleteAssignmentAttemptAsync(int id)
        {
            throw new NotImplementedException();
        }

        public Task<IEnumerable<AssignmentAttempt>> GetAllAssignmentAttemptAsync()
        {
            throw new NotImplementedException();
        }

        public Task<AssignmentAttempt> GetAssignmentAttemptByIdAsync(int id)
        {
            throw new NotImplementedException();
        }

        public Task<bool> UpdateAssignmentAttemptAsync(AssignmentAttempt assessment)
        {
            throw new NotImplementedException();
        }
    }
}
