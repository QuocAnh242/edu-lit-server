using LessonService.Application.Abstractions.Messaging;
using LessonService.Domain.Commons;
using LessonService.Domain.Entities;
using LessonService.Domain.Interfaces;
using System.Text.Json;

namespace LessonService.Application.Features.Sessions.UpdateSession;

public class UpdateSessionCommandHandler : ICommandHandler<UpdateSessionCommand>
{
    private readonly IUnitOfWork _unitOfWork;

    public UpdateSessionCommandHandler(IUnitOfWork unitOfWork)
    {
        _unitOfWork = unitOfWork;
    }

    public async Task<ApiResponse<object>> Handle(UpdateSessionCommand command, CancellationToken cancellationToken)
    {
        var session = await _unitOfWork.SessionRepository.GetByIdAsync(command.Id);

        if (session is null)
        {
            return ApiResponse<object>.FailureResponse("Session not found", 404);
        }

        // Update only provided fields
        if (!string.IsNullOrWhiteSpace(command.Title))
            session.Title = command.Title;

        if (command.Description is not null)
            session.Description = command.Description;

        if (command.Position.HasValue)
            session.Position = command.Position.Value;

        if (command.DurationMinutes.HasValue)
            session.DurationMinutes = command.DurationMinutes.Value;

        session.UpdatedAt = DateTime.UtcNow;

        _unitOfWork.SessionRepository.Update(session);

        // Create outbox message
        var outboxMessage = new OutboxMessage
        {
            Id = Guid.NewGuid(),
            Type = "SessionUpdated",
            Exchange = "session-events",
            RoutingKey = "session.updated",
            Payload = JsonSerializer.Serialize(new
            {
                Id = session.Id,
                CourseId = session.CourseId,
                Title = session.Title,
                Description = session.Description,
                Position = session.Position,
                DurationMinutes = session.DurationMinutes,
                UpdatedAt = session.UpdatedAt,
                EventType = "SessionUpdated",
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

        return ApiResponse<object>.SuccessResponse("Update Session Successfully");
    }
}

