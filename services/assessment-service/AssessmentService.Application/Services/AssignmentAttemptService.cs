using AssessmentService.Application.Services.Interfaces;
using AssessmentService.Domain.Entities;
using AssessmentService.Domain.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AssessmentService.Application.Services
{
    public class AssignmentAttemptService : IAssignmentAttemptService
    {
        private readonly IAssignmentAttemptRepository _repo;
        public AssignmentAttemptService(IAssignmentAttemptRepository repo)
        {
            _repo = repo;
        }
        public Task<AssignmentAttempt> CreateAsync(AssignmentAttempt asse)
        {
            throw new NotImplementedException();
        }

        public Task<bool> DeleteAsync(int id)
        {
            throw new NotImplementedException();
        }

        public Task<IEnumerable<AssignmentAttempt>> GetAllAsync()
        {
            throw new NotImplementedException();
        }

        public Task<AssignmentAttempt> GetByIdAsync(int id)
        {
            throw new NotImplementedException();
        }

        public Task<bool> UpdateAsync(AssignmentAttempt assessment)
        {
            throw new NotImplementedException();
        }
    }
}
