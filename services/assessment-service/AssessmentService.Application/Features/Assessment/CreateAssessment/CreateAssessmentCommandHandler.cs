using AssessmentService.Application.Abstractions.Messaging;
using AssessmentService.Domain.Commons;
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

        public CreateAssessmentCommandHandler(IUnitOfWork unitOfWork, IValidator<CreateAssessmentCommand> createSyllabusCommandValidator, IMapper mapper)
        {
            _unitOfWork = unitOfWork;
            _createAssessmentCommandValidator = createSyllabusCommandValidator;
            _mapper = mapper;
        }

        public async Task<ObjectResponse<int>> Handle(CreateAssessmentCommand assessmentCommand, CancellationToken cancellationToken)
        {
            var validationResult = await _createAssessmentCommandValidator.ValidateAsync(assessmentCommand);
            if (!validationResult.IsValid)
            {
                var errors = validationResult.Errors
                    .Select(e => new Error("Assessment.Create.Validation", e.ErrorMessage))
                    .ToList();
                return ObjectResponse<int>.Response("400", errors.First().Message, 0);
            }

            var createdAssessment = _mapper.Map<Domain.Entities.Assessment>(assessmentCommand);

            await _unitOfWork.AssessmentRepository.AddAsync(createdAssessment);

            try
            {
                await _unitOfWork.SaveChangesAsync();
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
