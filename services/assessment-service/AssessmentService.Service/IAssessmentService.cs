using AssessmentService.Database.Models;
using AssessmentService.Database.Models.DTOs;

namespace AssessmentService.Service
{
    public interface IAssessmentService
    {
        Task<ObjectResponse<Assessment>> CreateAsync(Assessment asse);
        Task<ObjectResponse<Assessment>> GetByIdAsync(int id);
        Task<ObjectResponse<IEnumerable<Assessment>>> GetAllAssessmentsAsync();
        Task<ObjectResponse<bool>> UpdateAssessmentAsync(Assessment assessment);
        Task<ObjectResponse<bool>> DeleteAssessmentAsync(int id);
    }
}
