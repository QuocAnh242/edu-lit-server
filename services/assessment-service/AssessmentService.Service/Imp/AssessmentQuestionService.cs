using AssessmentService.Database.Models;
using AssessmentService.Repository;

namespace AssessmentService.Service.Imp
{
    public class AssessmentQuestionService : IAssessmentQuestionService
    {
        private readonly IAssessmentQuestionRepository _repo;
        public AssessmentQuestionService(IAssessmentQuestionRepository repo)
        {
            _repo = repo;
        }
        public Task<AssessmentQuestion> AddAsync(AssessmentQuestion question)
        {
            throw new NotImplementedException();
        }

        public Task<bool> DeleteAsync(int id)
        {
            throw new NotImplementedException();
        }

        public Task<IEnumerable<AssessmentQuestion>> GetAllAsync()
        {
            throw new NotImplementedException();
        }

        public Task<AssessmentQuestion> GetByIdAsync(int id)
        {
            throw new NotImplementedException();
        }

        public Task<bool> UpdateAsync(AssessmentQuestion question)
        {
            throw new NotImplementedException();
        }
    }
}
