using AssessmentService.Application.Abstractions.Messaging;
using AssessmentService.Domain.Commons;
using AssessmentService.Domain.Interfaces;
using AutoMapper;

namespace AssessmentService.Application.Features.AssessmentQuestion.GetAllAssessmentQuestion
{
    public class GetAllAssessmentQuestionQueryHandler : IQueryHandler<GetAllAssessmentQuestionQuery, List<GetAllAssessmentQuestionResponse>>
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;
        public GetAllAssessmentQuestionQueryHandler(IUnitOfWork unitOfWork, IMapper mapper)
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
        }
        public async Task<ObjectResponse<List<GetAllAssessmentQuestionResponse>>> Handle(GetAllAssessmentQuestionQuery query, CancellationToken cancellationToken)
        {
            try
            {
                var assessmentQuestionsFromDb = await _unitOfWork.AssessmentQuestionRepository.GetAllAsync();
                var assessmentQuestions = _mapper.Map<List<GetAllAssessmentQuestionResponse>>(assessmentQuestionsFromDb);
                return ObjectResponse<List<GetAllAssessmentQuestionResponse>>.SuccessResponse(assessmentQuestions);
            }
            catch (Exception e)
            {
                return ObjectResponse<List<GetAllAssessmentQuestionResponse>>.FailureResponse(e);
            }
        }
    }
}
