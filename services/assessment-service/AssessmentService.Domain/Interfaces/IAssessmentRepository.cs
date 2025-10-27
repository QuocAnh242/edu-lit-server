using AssessmentService.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AssessmentService.Domain.Interfaces
{
    public interface IAssessmentRepository
    {
        Task CreateAssessmentAsync(Assessment assessment);
        Task<Assessment> GetAssessmentByIdAsync(int id);
        Task<IEnumerable<Assessment>> GetAllAssessmentsAsync();
        Task<bool> UpdateAssessmentAsync(Assessment assessment);
        Task<bool> DeleteAssessmentAsync(int id);
    }
}
