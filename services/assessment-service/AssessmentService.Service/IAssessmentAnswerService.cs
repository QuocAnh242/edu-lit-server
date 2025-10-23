using AssessmentService.Database.Models;
using AssessmentService.Database.Models.DTOs;

namespace AssessmentService.Service
{
    public interface IAssessmentAnswerService
    {
        Task<ObjectResponse<AssessmentAnswer>> CreateAsync(AssessmentAnswerDTO assedto);
        Task<ObjectResponse<AssessmentAnswer>> GetByIdAsync(int id);
        Task<ObjectResponse<IEnumerable<AssessmentAnswer>>> GetAllAsync();
        Task<ObjectResponse<bool>> UpdateAsync(AssessmentAnswerDTO assedto);
        Task<ObjectResponse<bool>> DeleteAsync(int id);
    }
}
