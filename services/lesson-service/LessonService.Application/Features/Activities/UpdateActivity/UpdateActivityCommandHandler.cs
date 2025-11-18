using LessonService.Application.Abstractions.Messaging;
using LessonService.Domain.Commons;
using LessonService.Domain.Entities;
using LessonService.Domain.Interfaces;
using System.Text.Json;

namespace LessonService.Application.Features.Activities.UpdateActivity;

public class UpdateActivityCommandHandler : ICommandHandler<UpdateActivityCommand>
{
    private readonly IUnitOfWork _unitOfWork;

    public UpdateActivityCommandHandler(IUnitOfWork unitOfWork)
    {
        _unitOfWork = unitOfWork;
    }

    public async Task<ApiResponse<object>> Handle(UpdateActivityCommand command, CancellationToken cancellationToken)
    {
        var activity = await _unitOfWork.ActivityRepository.GetByIdAsync(command.Id);

        if (activity is null)
        {
            return ApiResponse<object>.FailureResponse("Activity not found", 404);
        }

        // Update only provided fields
        if (!string.IsNullOrWhiteSpace(command.Title))
            activity.Title = command.Title;

        if (command.Description is not null)
            activity.Description = command.Description;

        if (!string.IsNullOrWhiteSpace(command.ActivityType))
            activity.ActivityType = command.ActivityType;

        if (command.Content is not null)
            activity.Content = command.Content;

        if (command.Points.HasValue)
            activity.Points = command.Points.Value;

        if (command.Position.HasValue)
            activity.Position = command.Position.Value;

        if (command.IsRequired.HasValue)
            activity.IsRequired = command.IsRequired.Value;

        activity.UpdatedAt = DateTime.UtcNow;

        _unitOfWork.ActivityRepository.Update(activity);

        // Create outbox message
        var outboxMessage = new OutboxMessage
        {
            Id = Guid.NewGuid(),
            Type = "ActivityUpdated",
            Exchange = "activity-events",
            RoutingKey = "activity.updated",
            Payload = JsonSerializer.Serialize(new
            {
                Id = activity.Id,
                SessionId = activity.SessionId,
                Title = activity.Title,
                Description = activity.Description,
                ActivityType = activity.ActivityType,
                Content = activity.Content,
                Points = activity.Points,
                Position = activity.Position,
                IsRequired = activity.IsRequired,
                UpdatedAt = activity.UpdatedAt,
                EventType = "ActivityUpdated",
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

        return ApiResponse<object>.SuccessResponse("Update Activity Successfully");
    }
}

