using AssessmentService.Application.Abstractions.Messaging;
using AssessmentService.Application.Features.Assessment.CreateAssessment;
using AssessmentService.Application.IServices;
using AssessmentService.Domain.Commons;
using AssessmentService.Domain.Interfaces;
using AutoMapper;
using FluentValidation;

namespace AssessmentService.Application.Features.AssessmentQuestion.CreateAssessmentQuestion
{
    public class CreateAssessmentQuestionCommandHandler : ICommandHandler<CreateAssessmentQuestionCommand, int>
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IValidator<CreateAssessmentQuestionCommand> _createAssessmentCommandValidator;
        private readonly IMapper _mapper;
        private readonly IRedisService _redisService;
        private const string CacheKey = "assessmentQuestions:all";

        public CreateAssessmentQuestionCommandHandler(
            IUnitOfWork unitOfWork,
            IValidator<CreateAssessmentQuestionCommand> createAssessmentCommandValidator,
            IMapper mapper,
            IRedisService redisService)
        {
            _unitOfWork = unitOfWork;
            _createAssessmentCommandValidator = createAssessmentCommandValidator;
            _mapper = mapper;
            _redisService = redisService;
        }

        public async Task<ObjectResponse<int>> Handle(CreateAssessmentQuestionCommand assessmentQuestionCommand, CancellationToken cancellationToken)
        {
            // validation
            var validationResult = await _createAssessmentCommandValidator.ValidateAsync(assessmentQuestionCommand);
            if (!validationResult.IsValid)
            {
                var errors = validationResult.Errors
                    .Select(e => new Error("AssessmentQuestion.Create.Validation", e.ErrorMessage))
                    .ToList();
                return ObjectResponse<int>.Response("400", errors.First().Message, 0);
            }

            var assessment = await _unitOfWork.AssessmentRepository.GetByIdAsync(assessmentQuestionCommand.AssessmentId);
            if (assessment == null)
            {
                return ObjectResponse<int>.Response("404", "Assessment not found", 0);
            }

            // sẽ có hàm check valid questionId sau khi có service của question
            //

            var createdAssessmentQuestion = _mapper.Map<Domain.Entities.AssessmentQuestion>(assessmentQuestionCommand);

            createdAssessmentQuestion.IsActive = true;

            await _unitOfWork.AssessmentQuestionRepository.AddAsync(createdAssessmentQuestion);

            try
            {
                await _unitOfWork.SaveChangesAsync();

                // Invalidate cache
                await _redisService.RemoveAsync(CacheKey);

                //sẽ có hàm commit để tự động thêm vào redis sau khi nhận được thông báo của rabbit, chưa làm liền để test thử cái redis cái đã.
            }
            catch (Exception e)
            {
                return ObjectResponse<int>.Response("400", e.Message, 0);
            }
            return ObjectResponse<int>.SuccessResponse(createdAssessmentQuestion.AssessmentQuestionId);
        }
    }
}
