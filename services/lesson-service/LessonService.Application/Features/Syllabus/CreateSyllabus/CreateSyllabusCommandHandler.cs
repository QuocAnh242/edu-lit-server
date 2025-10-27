using AutoMapper;
using FluentValidation;
using LessonService.Application.Abstractions.Messaging;
using LessonService.Domain.Commons;
using LessonService.Domain.Interfaces;

namespace LessonService.Application.Features.Syllabus.CreateSyllabus
{
    public class CreateSyllabusCommandHandler : ICommandHandler<CreateSyllabusCommand, Guid>
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IValidator<CreateSyllabusCommand> _createSyllabusCommandValidator;
        private readonly IMapper _mapper;

        public CreateSyllabusCommandHandler(IUnitOfWork unitOfWork, IValidator<CreateSyllabusCommand> createSyllabusCommandValidator, IMapper mapper)
        {
            _unitOfWork = unitOfWork;
            _createSyllabusCommandValidator = createSyllabusCommandValidator;
            _mapper = mapper;
        }

        public async Task<ApiResponse<Guid>> Handle(CreateSyllabusCommand syllabusCommand, CancellationToken cancellationToken)
        {
            var validationResult = await _createSyllabusCommandValidator.ValidateAsync(syllabusCommand);
            if (!validationResult.IsValid)
            {
                var errors = validationResult.Errors
                    .Select(e => new Error("Syllabus.Create.Validation", e.ErrorMessage))
                    .ToList();
                return ApiResponse<Guid>.FailureResponse(errors.First().Message, 400);
            }

            var createdSyllabus = _mapper.Map<Domain.Entities.Syllabus>(syllabusCommand);
            
            createdSyllabus.Id = Guid.NewGuid();
            
            await _unitOfWork.SyllabusRepository.AddAsync(createdSyllabus);

            try
            {
                await _unitOfWork.SaveChangesAsync();
                //sẽ có hàm commit để tự động thêm vào redis sau khi nhận được thông báo của rabbit, chưa làm liền để test thử cái redis cái đã.
            }
            catch (Exception e)
            {
                return ApiResponse<Guid>.FailureResponse(e.Message, 500);
            }
            
            return ApiResponse<Guid>.SuccessResponse(createdSyllabus.Id, "Create Syllabus Successfully", 201);
        }
        
    }
}
