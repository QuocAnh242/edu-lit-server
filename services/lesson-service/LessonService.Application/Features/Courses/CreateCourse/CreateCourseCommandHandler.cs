using AutoMapper;
using FluentValidation;
using LessonService.Application.Abstractions.Messaging;
using LessonService.Domain.Commons;
using LessonService.Domain.Entities;
using LessonService.Domain.Interfaces;
using System.Text.Json;

namespace LessonService.Application.Features.Courses.CreateCourse;

public class CreateCourseCommandHandler : ICommandHandler<CreateCourseCommand, Guid>
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly IValidator<CreateCourseCommand> _validator;
    private readonly IMapper _mapper;

    public CreateCourseCommandHandler(IUnitOfWork unitOfWork, IValidator<CreateCourseCommand> validator, IMapper mapper)
    {
        _unitOfWork = unitOfWork;
        _validator = validator;
        _mapper = mapper;
    }

    public async Task<ApiResponse<Guid>> Handle(CreateCourseCommand command, CancellationToken cancellationToken)
    {
        var validationResult = await _validator.ValidateAsync(command, cancellationToken);
        if (!validationResult.IsValid)
        {
            var errors = validationResult.Errors
                .Select(e => new Error("Course.Create.Validation", e.ErrorMessage))
                .ToList();
            return ApiResponse<Guid>.FailureResponse(errors.First().Message, 400);
        }

        // Check if Syllabus exists
        var syllabusExists = await _unitOfWork.SyllabusRepository.GetByIdAsync(command.SyllabusId);
        if (syllabusExists is null)
        {
            return ApiResponse<Guid>.FailureResponse("Syllabus not found", 404);
        }

        var course = _mapper.Map<Domain.Entities.Course>(command);
        course.Id = Guid.NewGuid();
        course.CreatedAt = DateTime.UtcNow;

        await _unitOfWork.CourseRepository.AddAsync(course);

        // Create outbox message
        var outboxMessage = new OutboxMessage
        {
            Id = Guid.NewGuid(),
            Type = "CourseCreated",
            Exchange = "coursera-events",
            RoutingKey = "course.created",
            Payload = JsonSerializer.Serialize(new
            {
                Id = course.Id,
                SyllabusId = course.SyllabusId,
                CourseCode = course.CourseCode,
                Title = course.Title,
                Description = course.Description,
                CreatedAt = course.CreatedAt,
                EventType = "CourseCreated",
                Timestamp = DateTime.UtcNow
            }),
            CreatedAt = DateTime.UtcNow,
            IsProcessed = false,
            RetryCount = 0
        };

        await _unitOfWork.OutboxRepository.AddAsync(outboxMessage);

        try
        {
            await _unitOfWork.SaveChangesAsync(cancellationToken);
        }
        catch (Exception e)
        {
            return ApiResponse<Guid>.FailureResponse(e.Message, 500);
        }

        return ApiResponse<Guid>.SuccessResponse(course.Id, "Create Course Successfully");
    }
}


