using AssessmentService.Application.DTOs.Request;
using AssessmentService.Domain.Commons;
using AssessmentService.Domain.Entities;

namespace AssessmentService.Application.Services.Interfaces
{
    public interface IAssessmentService
    {
        Task<ObjectResponse<Assessment>> CreateAsync(AssessmentDTO asse);
        Task<ObjectResponse<Assessment>> GetByIdAsync(int id);
        Task<ObjectResponse<IEnumerable<Assessment>>> GetAllAsync();
        Task<ObjectResponse<bool>> UpdateAsync(AssessmentDTO assessment);
        Task<ObjectResponse<bool>> DeleteAsync(int id);
    }
}
