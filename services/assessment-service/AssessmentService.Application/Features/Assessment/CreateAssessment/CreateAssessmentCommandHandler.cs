using AssessmentService.Application.Abstractions.Messaging;
using AssessmentService.Application.IServices;
using AssessmentService.Domain.Commons;
using AssessmentService.Domain.Enums;
using AssessmentService.Domain.Interfaces;
using AutoMapper;
using FluentValidation;

namespace AssessmentService.Application.Features.Assessment.CreateAssessment
{
    public class CreateAssessmentCommandHandler : ICommandHandler<CreateAssessmentCommand, int>
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IValidator<CreateAssessmentCommand> _createAssessmentCommandValidator;
        private readonly IMapper _mapper;
        private readonly IRedisService _redisService;
        private const string CacheKey = "assessments:all";

        public CreateAssessmentCommandHandler(
            IUnitOfWork unitOfWork,
            IValidator<CreateAssessmentCommand> createAssessmentCommandValidator,
            IMapper mapper,
            IRedisService redisService)
        {
            _unitOfWork = unitOfWork;
            _createAssessmentCommandValidator = createAssessmentCommandValidator;
            _mapper = mapper;
            _redisService = redisService;
        }

        public async Task<ObjectResponse<int>> Handle(CreateAssessmentCommand assessmentCommand, CancellationToken cancellationToken)
        {
            // validation
            var validationResult = await _createAssessmentCommandValidator.ValidateAsync(assessmentCommand);
            if (!validationResult.IsValid)
            {
                var errors = validationResult.Errors
                    .Select(e => new Error("Assessment.Create.Validation", e.ErrorMessage))
                    .ToList();
                return ObjectResponse<int>.Response("400", errors.First().Message, 0);
            }

            // sẽ có hàm check valid courseId và creatorId sau khi có service của user và course

            var createdAssessment = _mapper.Map<Domain.Entities.Assessment>(assessmentCommand);

            createdAssessment.Status = AssessmentStatus.Public.ToString();
            createdAssessment.IsActive = true;

            await _unitOfWork.AssessmentRepository.AddAsync(createdAssessment);

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

            return ObjectResponse<int>.SuccessResponse(createdAssessment.AssessmentId);
        }

    }
}
