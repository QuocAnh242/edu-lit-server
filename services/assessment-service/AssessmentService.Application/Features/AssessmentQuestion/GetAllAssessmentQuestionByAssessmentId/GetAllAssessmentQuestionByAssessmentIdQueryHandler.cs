using AssessmentService.Application.Abstractions.Messaging;
using AssessmentService.Domain.Commons;
using AssessmentService.Domain.Interfaces;
using AutoMapper;

namespace AssessmentService.Application.Features.AssessmentQuestion.GetAllAssessmentQuestionByAssessmentId
{
    public class GetAllAssessmentQuestionByAssessmentIdQueryHandler : IQueryHandler<GetAllAssessmentQuestionByAssessmentIdQuery, List<GetAllAssessmentQuestionByAssessmentIdResponse>>
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;
        public GetAllAssessmentQuestionByAssessmentIdQueryHandler(IUnitOfWork unitOfWork, IMapper mapper)
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
        }

        public async Task<ObjectResponse<List<GetAllAssessmentQuestionByAssessmentIdResponse>>> Handle(GetAllAssessmentQuestionByAssessmentIdQuery query, CancellationToken cancellationToken)
        {
            try
            {
                var assessmentQuestions = await _unitOfWork.AssessmentQuestionRepository.GetAllByAsync(x => x.AssessmentQuestionId == query.Id);
                var response = _mapper.Map<List<GetAllAssessmentQuestionByAssessmentIdResponse>>(assessmentQuestions);
                return ObjectResponse<List<GetAllAssessmentQuestionByAssessmentIdResponse>>.SuccessResponse(response);
            }
            catch (Exception e)
            {
                return ObjectResponse<List<GetAllAssessmentQuestionByAssessmentIdResponse>>.FailureResponse(e);
            }
        }
    }
}
