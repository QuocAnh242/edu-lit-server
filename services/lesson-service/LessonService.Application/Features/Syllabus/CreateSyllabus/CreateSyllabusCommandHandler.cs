using AutoMapper;
using FluentValidation;
using LessonService.Application.Abstractions.Messaging;
using LessonService.Domain.Commons;
using LessonService.Domain.Entities;
using LessonService.Domain.Interfaces;
using System.Text.Json;

namespace LessonService.Application.Features.Syllabus.CreateSyllabus
{
    public class CreateSyllabusCommandHandler : ICommandHandler<CreateSyllabusCommand, Guid>
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IValidator<CreateSyllabusCommand> _createSyllabusCommandValidator;
        private readonly IMapper _mapper;

        public CreateSyllabusCommandHandler(
            IUnitOfWork unitOfWork, 
            IValidator<CreateSyllabusCommand> createSyllabusCommandValidator, 
            IMapper mapper)
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

            // Create outbox message - will be published by background service
            var outboxMessage = new OutboxMessage
            {
                Id = Guid.NewGuid(),
                Type = "SyllabusCreated",
                Exchange = "syllabus-events",
                RoutingKey = "syllabus.created", // Routing key for Topic exchange
                Payload = JsonSerializer.Serialize(new
                {
                    Id = createdSyllabus.Id,
                    Title = createdSyllabus.Title,
                    AcademicYear = createdSyllabus.AcademicYear,
                    Semester = createdSyllabus.Semester.ToString(),
                    Description = createdSyllabus.Description,
                    OwnerId = createdSyllabus.OwnerId,
                    IsActive = createdSyllabus.IsActive,
                    CreatedAt = createdSyllabus.CreatedAt,
                    EventType = "SyllabusCreated",
                    Timestamp = DateTime.UtcNow
                }),
                CreatedAt = DateTime.UtcNow,
                IsProcessed = false,
                RetryCount = 0
            };

            await _unitOfWork.OutboxRepository.AddAsync(outboxMessage);

            try
            {
                // Save both Syllabus and OutboxMessage in same transaction
                await _unitOfWork.SaveChangesAsync(cancellationToken);
            }
            catch (Exception e)
            {
                return ApiResponse<Guid>.FailureResponse(e.Message, 500);
            }
            
            return ApiResponse<Guid>.SuccessResponse(createdSyllabus.Id, "Create Syllabus Successfully");
        }
    }
}
