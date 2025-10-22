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

        public async Task<Result<Guid>> Handle(CreateSyllabusCommand syllabusCommand, CancellationToken cancellationToken)
        {
            var validationResult = await _createSyllabusCommandValidator.ValidateAsync(syllabusCommand);
            if (!validationResult.IsValid)
            {
                var errors = validationResult.Errors
                    .Select(e => new Error("Syllabus.Create.Validation", e.ErrorMessage))
                    .ToList();
                return Result<Guid>.Failure(errors);
            }

            var createdSyllabus = _mapper.Map<Domain.Entities.Syllabus>(syllabusCommand);
            
            createdSyllabus.Id = Guid.NewGuid();
            
            await _unitOfWork.SyllabusRepository.AddAsync(createdSyllabus);

            try
            {
                await _unitOfWork.SaveChangesAsync();
            }
            catch (Exception e)
            {
                return Result<Guid>.Failure(new Error("Syllabus.Create.Database.Error", e.Message));
            }
            
            return Result<Guid>.Success(createdSyllabus.Id);
        }
        
    }
}
