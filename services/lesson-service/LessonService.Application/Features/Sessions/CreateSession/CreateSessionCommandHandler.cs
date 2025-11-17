using AutoMapper;
using FluentValidation;
using LessonService.Application.Abstractions.Messaging;
using LessonService.Domain.Commons;
using LessonService.Domain.Entities;
using LessonService.Domain.Interfaces;
using System.Text.Json;

namespace LessonService.Application.Features.Sessions.CreateSession;

public class CreateSessionCommandHandler : ICommandHandler<CreateSessionCommand, Guid>
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly IValidator<CreateSessionCommand> _validator;
    private readonly IMapper _mapper;

    public CreateSessionCommandHandler(IUnitOfWork unitOfWork, IValidator<CreateSessionCommand> validator, IMapper mapper)
    {
        _unitOfWork = unitOfWork;
        _validator = validator;
        _mapper = mapper;
    }

    public async Task<ApiResponse<Guid>> Handle(CreateSessionCommand command, CancellationToken cancellationToken)
    {
        var validationResult = await _validator.ValidateAsync(command, cancellationToken);
        if (!validationResult.IsValid)
        {
            var errors = validationResult.Errors
                .Select(e => new Error("Session.Create.Validation", e.ErrorMessage))
                .ToList();
            return ApiResponse<Guid>.FailureResponse(errors.First().Message, 400);
        }

        // Check if Course exists
        var courseExists = await _unitOfWork.CourseRepository.GetByIdAsync(command.CourseId);
        if (courseExists is null)
        {
            return ApiResponse<Guid>.FailureResponse("Course not found", 404);
        }

        var session = _mapper.Map<Domain.Entities.Session>(command);
        session.Id = Guid.NewGuid();
        session.CreatedAt = DateTime.UtcNow;

        await _unitOfWork.SessionRepository.AddAsync(session);

        // Create outbox message
        var outboxMessage = new OutboxMessage
        {
            Id = Guid.NewGuid(),
            Type = "SessionCreated",
            Exchange = "session-events",
            RoutingKey = "session.created",
            Payload = JsonSerializer.Serialize(new
            {
                Id = session.Id,
                CourseId = session.CourseId,
                Title = session.Title,
                Description = session.Description,
                Position = session.Position,
                DurationMinutes = session.DurationMinutes,
                CreatedAt = session.CreatedAt,
                EventType = "SessionCreated",
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

        return ApiResponse<Guid>.SuccessResponse(session.Id, "Create Session Successfully");
    }
}


