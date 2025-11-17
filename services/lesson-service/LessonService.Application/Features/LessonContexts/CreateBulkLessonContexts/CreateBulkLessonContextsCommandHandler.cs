using FluentValidation;
using LessonService.Application.Abstractions.Messaging;
using LessonService.Domain.Commons;
using LessonService.Domain.Entities;
using LessonService.Domain.Interfaces;
using System.Text.Json;

namespace LessonService.Application.Features.LessonContexts.CreateBulkLessonContexts;

public class CreateBulkLessonContextsCommandHandler : ICommandHandler<CreateBulkLessonContextsCommand, List<Guid>>
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly IValidator<CreateBulkLessonContextsCommand> _validator;

    public CreateBulkLessonContextsCommandHandler(
        IUnitOfWork unitOfWork, 
        IValidator<CreateBulkLessonContextsCommand> validator)
    {
        _unitOfWork = unitOfWork;
        _validator = validator;
    }

    public async Task<ApiResponse<List<Guid>>> Handle(CreateBulkLessonContextsCommand command, CancellationToken cancellationToken)
    {
        var validationResult = await _validator.ValidateAsync(command, cancellationToken);
        if (!validationResult.IsValid)
        {
            var errors = validationResult.Errors
                .Select(e => new Error("LessonContext.CreateBulk.Validation", e.ErrorMessage))
                .ToList();
            return ApiResponse<List<Guid>>.FailureResponse(errors.First().Message, 400);
        }

        // Check if Session exists
        var sessionExists = await _unitOfWork.SessionRepository.GetByIdAsync(command.SessionId);
        if (sessionExists is null)
        {
            return ApiResponse<List<Guid>>.FailureResponse("Session not found", 404);
        }

        var createdIds = new List<Guid>();
        var now = DateTime.UtcNow;

        // Create all LessonContexts
        foreach (var item in command.LessonContexts)
        {
            var lessonContext = new LessonContext
            {
                Id = Guid.NewGuid(),
                SessionId = command.SessionId,
                ParentLessonId = item.ParentLessonId,
                LessonTitle = item.LessonTitle,
                LessonContent = item.LessonContent,
                Position = item.Position,
                Level = item.Level,
                CreatedAt = now,
                UpdatedAt = now
            };

            await _unitOfWork.LessonContextRepository.AddAsync(lessonContext);
            createdIds.Add(lessonContext.Id);
        }

        // Create single outbox message for bulk operation
        var outboxMessage = new OutboxMessage
        {
            Id = Guid.NewGuid(),
            Type = "LessonContextBulkCreated",
            Exchange = "lesson-context-events",
            RoutingKey = "lessoncontext.created",
            Payload = JsonSerializer.Serialize(new
            {
                SessionId = command.SessionId,
                LessonContexts = command.LessonContexts.Select((item, index) => new
                {
                    Id = createdIds[index],
                    SessionId = command.SessionId,
                    ParentLessonId = item.ParentLessonId,
                    LessonTitle = item.LessonTitle,
                    LessonContent = item.LessonContent,
                    Position = item.Position,
                    Level = item.Level,
                    CreatedAt = now
                }).ToList(),
                TotalCount = createdIds.Count,
                EventType = "LessonContextBulkCreated",
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
            return ApiResponse<List<Guid>>.FailureResponse(e.Message, 500);
        }

        return ApiResponse<List<Guid>>.SuccessResponse(createdIds, $"Created {createdIds.Count} LessonContexts Successfully");
    }
}

