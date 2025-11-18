using LessonService.Application.Abstractions.Messaging;
using LessonService.Domain.Commons;
using LessonService.Domain.Entities;
using LessonService.Domain.Interfaces;
using System.Text.Json;

namespace LessonService.Application.Features.LessonContexts.UpdateLessonContext;

public class UpdateLessonContextCommandHandler : ICommandHandler<UpdateLessonContextCommand>
{
    private readonly IUnitOfWork _unitOfWork;

    public UpdateLessonContextCommandHandler(IUnitOfWork unitOfWork)
    {
        _unitOfWork = unitOfWork;
    }

    public async Task<ApiResponse<object>> Handle(UpdateLessonContextCommand command, CancellationToken cancellationToken)
    {
        var lessonContext = await _unitOfWork.LessonContextRepository.GetByIdAsync(command.Id);

        if (lessonContext is null)
        {
            return ApiResponse<object>.FailureResponse("LessonContext not found", 404);
        }

        // Update only provided fields
        if (!string.IsNullOrWhiteSpace(command.LessonTitle))
            lessonContext.LessonTitle = command.LessonTitle;

        if (command.LessonContent is not null)
            lessonContext.LessonContent = command.LessonContent;

        if (command.Position.HasValue)
            lessonContext.Position = command.Position.Value;

        if (command.Level.HasValue)
            lessonContext.Level = command.Level.Value;

        lessonContext.UpdatedAt = DateTime.UtcNow;

        _unitOfWork.LessonContextRepository.Update(lessonContext);

        // Create outbox message
        var outboxMessage = new OutboxMessage
        {
            Id = Guid.NewGuid(),
            Type = "LessonContextUpdated",
            Exchange = "lesson-context-events",
            RoutingKey = "lessoncontext.updated",
            Payload = JsonSerializer.Serialize(new
            {
                Id = lessonContext.Id,
                SessionId = lessonContext.SessionId,
                ParentLessonId = lessonContext.ParentLessonId,
                LessonTitle = lessonContext.LessonTitle,
                LessonContent = lessonContext.LessonContent,
                Position = lessonContext.Position,
                Level = lessonContext.Level,
                UpdatedAt = lessonContext.UpdatedAt,
                EventType = "LessonContextUpdated",
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
            return ApiResponse<object>.FailureResponse(e.Message, 500);
        }

        return ApiResponse<object>.SuccessResponse("Update LessonContext Successfully");
    }
}

