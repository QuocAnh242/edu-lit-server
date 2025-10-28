using AssessmentService.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AssessmentService.Application.Services.Interfaces
{
    public interface IAssignmentAttemptService
    {
        Task<AssignmentAttempt> CreateAsync(AssignmentAttempt asse);
        Task<AssignmentAttempt> GetByIdAsync(int id);
        Task<IEnumerable<AssignmentAttempt>> GetAllAsync();
        Task<bool> UpdateAsync(AssignmentAttempt assessment);
        Task<bool> DeleteAsync(int id);
    }
}
