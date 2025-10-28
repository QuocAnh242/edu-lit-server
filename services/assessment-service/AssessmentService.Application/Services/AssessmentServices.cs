using AssessmentService.Application.DTOs.Request;
using AssessmentService.Application.Services.Interfaces;
using AssessmentService.Domain.Commons;
using AssessmentService.Domain.Entities;
using AssessmentService.Domain.Enums;
using AssessmentService.Domain.Interfaces;

namespace AssessmentService.Application.Services
{
    public class AssessmentServices : IAssessmentService
    {
        private readonly IAssessmentRepository _repo;
        public AssessmentServices(IAssessmentRepository repo)
        {
            _repo = repo;
        }

        public async Task<ObjectResponse<Assessment>> CreateAsync(AssessmentDTO asse)
        {
            try
            {
                if(asse == null)
                {
                    return ObjectResponse<Assessment>.Response("400", $"Assessment data is null", null);
                }

                // call to api Course from other service to check if course exists
                // ....

                // call to api User from other service to check if creator exists
                // ....

                // Validate Status explicitly
                if (!Enum.IsDefined(typeof(AssessmentStatus), asse.Status))
                {
                    return ObjectResponse<Assessment>.Response("400", $"Invalid status value: {asse.Status}", null);
                }

                Assessment createdAsse = new Assessment 
                {
                    CourseId = asse.CourseId,
                    CreatorId = asse.CreatorId,
                    Title = asse.Title,
                    Description = asse.Description,
                    TotalQuestions = asse.TotalQuestions,
                    DurationMinutes = asse.DurationMinutes,
                    Status = asse.Status,
                    IsActive = true
                };
                await _repo.CreateAssessmentAsync(createdAsse);
                return ObjectResponse<Assessment>.SuccessResponse(createdAsse);

            }
            catch (Exception ex)
            {
                return ObjectResponse<Assessment>.FailureResponse(ex);
            }
        }

        public async Task<ObjectResponse<bool>> DeleteAsync(int id)
        {
            try
            {
                bool deleted = await _repo.DeleteAssessmentAsync(id);
                return ObjectResponse<bool>.SuccessResponse(deleted);
            }
            catch (Exception ex)
            {
                return ObjectResponse<bool>.FailureResponse(ex);
            }
        }

        public async Task<ObjectResponse<IEnumerable<Assessment>>> GetAllAsync()
        {
            try
            {
                IEnumerable<Assessment> answers = await _repo.GetAllAssessmentsAsync();
                return ObjectResponse<IEnumerable<Assessment>>.SuccessResponse(answers);
            }
            catch (Exception ex)
            {
                return ObjectResponse<IEnumerable<Assessment>>.FailureResponse(ex);
            }
        }

        public async Task<ObjectResponse<Assessment>> GetByIdAsync(int id)
        {
            try
            {
                Assessment asse = await _repo.GetAssessmentByIdAsync(id);
                if (asse == null)
                {
                    return ObjectResponse<Assessment>.Response("404", "AssessmentAnswer not found", null);
                }
                return ObjectResponse<Assessment>.SuccessResponse(asse);
            }
            catch (Exception ex)
            {
                return ObjectResponse<Assessment>.FailureResponse(ex);
            }
        }

        public async Task<ObjectResponse<bool>> UpdateAsync(AssessmentDTO asse)
        {
            try
            {
                if (asse == null)
                {
                    return ObjectResponse<bool>.Response("400", $"Assessment data is null", false);
                }

                // call to api Course from other service to check if course exists
                // ....

                // call to api User from other service to check if creator exists
                // ....

                // Validate Status explicitly
                if (!Enum.IsDefined(typeof(AssessmentStatus), asse.Status))
                {
                    return ObjectResponse<bool>.Response("400", $"Invalid status value: {asse.Status}", false);
                }

                Assessment updatedAsse = new Assessment
                {
                    CourseId = asse.CourseId,
                    CreatorId = asse.CreatorId,
                    Title = asse.Title,
                    Description = asse.Description,
                    TotalQuestions = asse.TotalQuestions,
                    DurationMinutes = asse.DurationMinutes,
                    Status = asse.Status,
                    IsActive = true
                };
                bool result = await _repo.UpdateAssessmentAsync(updatedAsse);
                return ObjectResponse<bool>.SuccessResponse(result);
            }
            catch (Exception ex)
            {
                return ObjectResponse<bool>.FailureResponse(ex);
            }
        }
    }
}
