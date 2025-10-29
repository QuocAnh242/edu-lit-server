using AssessmentService.Application.Abstractions.Messaging;
using AssessmentService.Domain.Commons;
using AssessmentService.Domain.Interfaces;

namespace AssessmentService.Application.Features.Assessment.GetAllAssessment
{
    public class GetAllAssessmentQueryHandler : IQueryHandler<GetAllAssessmentQuery, List<GetAllAssessmentResponse>>
    {
        private readonly IUnitOfWork _unitOfWork;
        public GetAllAssessmentQueryHandler(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }
        public async Task<ObjectResponse<List<GetAllAssessmentResponse>>> Handle(GetAllAssessmentQuery query, CancellationToken cancellationToken)
        {
            try
            {
                var assessmentEntities = await _unitOfWork.AssessmentRepository.GetAllAsync();
                var assessments = assessmentEntities.Select(assessmentEntity => new GetAllAssessmentResponse
                {
                    AssessmentId = assessmentEntity.AssessmentId,
                    CourseId = assessmentEntity.CourseId,
                    CreatorId = assessmentEntity.CreatorId,
                    Title = assessmentEntity.Title,
                    Description = assessmentEntity.Description,
                    TotalQuestions = assessmentEntity.TotalQuestions,
                    DurationMinutes = assessmentEntity.DurationMinutes,
                    Status = assessmentEntity.Status,
                    IsActive = assessmentEntity.IsActive,
                    CreatedAt = assessmentEntity.CreatedAt,
                    UpdatedAt = assessmentEntity.UpdatedAt
                }).ToList();
                return ObjectResponse<List<GetAllAssessmentResponse>>.SuccessResponse(assessments);
            }
            catch (Exception e)
            {
                return ObjectResponse<List<GetAllAssessmentResponse>>.FailureResponse(e);
            }
        }
    }
}
