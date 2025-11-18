using LessonService.Application.Abstractions.Messaging;
using LessonService.Domain.Commons;
using LessonService.Domain.Entities;
using LessonService.Domain.Interfaces;
using System.Text.Json;

namespace LessonService.Application.Features.Activities.DeleteActivity;

public class DeleteActivityCommandHandler : ICommandHandler<DeleteActivityCommand>
{
    private readonly IUnitOfWork _unitOfWork;

    public DeleteActivityCommandHandler(IUnitOfWork unitOfWork)
    {
        _unitOfWork = unitOfWork;
    }

    public async Task<ApiResponse<object>> Handle(DeleteActivityCommand command, CancellationToken cancellationToken)
    {
        var activity = await _unitOfWork.ActivityRepository.GetByIdAsync(command.Id);

        if (activity is null)
        {
            return ApiResponse<object>.FailureResponse("Activity not found", 404);
        }

        _unitOfWork.ActivityRepository.Remove(activity);

        // Create outbox message
        var outboxMessage = new OutboxMessage
        {
            Id = Guid.NewGuid(),
            Type = "ActivityDeleted",
            Exchange = "activity-events",
            RoutingKey = "activity.deleted",
            Payload = JsonSerializer.Serialize(new
            {
                Id = activity.Id,
                Title = activity.Title,
                EventType = "ActivityDeleted",
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

        return ApiResponse<object>.SuccessResponse("Delete Activity Successfully");
    }
}

