using LessonService.Application.Abstractions.Messaging;
using LessonService.Domain.Commons;
using LessonService.Domain.Entities;
using LessonService.Domain.Interfaces;
using System.Text.Json;

namespace LessonService.Application.Features.LessonContexts.DeleteLessonContext;

public class DeleteLessonContextCommandHandler : ICommandHandler<DeleteLessonContextCommand>
{
    private readonly IUnitOfWork _unitOfWork;

    public DeleteLessonContextCommandHandler(IUnitOfWork unitOfWork)
    {
        _unitOfWork = unitOfWork;
    }

    public async Task<ApiResponse<object>> Handle(DeleteLessonContextCommand command, CancellationToken cancellationToken)
    {
        var lessonContext = await _unitOfWork.LessonContextRepository.GetByIdAsync(command.Id);

        if (lessonContext is null)
        {
            return ApiResponse<object>.FailureResponse("LessonContext not found", 404);
        }

        _unitOfWork.LessonContextRepository.Remove(lessonContext);

        // Create outbox message
        var outboxMessage = new OutboxMessage
        {
            Id = Guid.NewGuid(),
            Type = "LessonContextDeleted",
            Exchange = "lesson-context-events",
            RoutingKey = "lessoncontext.deleted",
            Payload = JsonSerializer.Serialize(new
            {
                Id = lessonContext.Id,
                LessonTitle = lessonContext.LessonTitle,
                EventType = "LessonContextDeleted",
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

        return ApiResponse<object>.SuccessResponse("Delete LessonContext Successfully");
    }
}

