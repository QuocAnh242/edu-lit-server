using AssessmentService.Application.Abstractions.Messaging;
using AssessmentService.Domain.Commons;
using AssessmentService.Domain.Interfaces;
using AutoMapper;

namespace AssessmentService.Application.Features.Assessment.GetAllAssessment
{
    public class GetAllAssessmentQueryHandler : IQueryHandler<GetAllAssessmentQuery, List<GetAllAssessmentResponse>>
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;
        public GetAllAssessmentQueryHandler(IUnitOfWork unitOfWork, IMapper mapper)
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
        }
        public async Task<ObjectResponse<List<GetAllAssessmentResponse>>> Handle(GetAllAssessmentQuery query, CancellationToken cancellationToken)
        {
            try
            {
                var assessmentEntities = await _unitOfWork.AssessmentRepository.GetAllAsync();
                var assessments = _mapper.Map<List<GetAllAssessmentResponse>>(assessmentEntities);
                return ObjectResponse<List<GetAllAssessmentResponse>>.SuccessResponse(assessments);
            }
            catch (Exception e)
            {
                return ObjectResponse<List<GetAllAssessmentResponse>>.FailureResponse(e);
            }
        }
    }
}
