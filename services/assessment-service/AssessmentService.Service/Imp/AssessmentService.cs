using AssessmentService.Database.Models;
using AssessmentService.Repository;

namespace AssessmentService.Service.Imp
{
    public class AssessmentService : IAssessmentService
    {
        private readonly IAssessmentRepository _repo;
        public AssessmentService(IAssessmentRepository repo)
        {
            _repo = repo;
        }
        public Task<Assessment> CreateAsync(Assessment asse)
        {
            throw new NotImplementedException();
        }

        public Task<bool> DeleteAssessmentAsync(int id)
        {
            throw new NotImplementedException();
        }

        public Task<IEnumerable<Assessment>> GetAllAssessmentsAsync()
        {
            throw new NotImplementedException();
        }

        public Task<Assessment> GetByIdAsync(int id)
        {
            throw new NotImplementedException();
        }

        public Task<bool> UpdateAssessmentAsync(Assessment assessment)
        {
            throw new NotImplementedException();
        }
    }
}
