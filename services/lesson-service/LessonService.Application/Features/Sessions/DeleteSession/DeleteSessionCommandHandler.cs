using LessonService.Application.Abstractions.Messaging;
using LessonService.Domain.Commons;
using LessonService.Domain.Entities;
using LessonService.Domain.Interfaces;
using System.Text.Json;

namespace LessonService.Application.Features.Sessions.DeleteSession;

public class DeleteSessionCommandHandler : ICommandHandler<DeleteSessionCommand>
{
    private readonly IUnitOfWork _unitOfWork;

    public DeleteSessionCommandHandler(IUnitOfWork unitOfWork)
    {
        _unitOfWork = unitOfWork;
    }

    public async Task<ApiResponse<object>> Handle(DeleteSessionCommand command, CancellationToken cancellationToken)
    {
        var session = await _unitOfWork.SessionRepository.GetByIdAsync(command.Id);

        if (session is null)
        {
            return ApiResponse<object>.FailureResponse("Session not found", 404);
        }

        _unitOfWork.SessionRepository.Remove(session);

        // Create outbox message
        var outboxMessage = new OutboxMessage
        {
            Id = Guid.NewGuid(),
            Type = "SessionDeleted",
            Exchange = "session-events",
            RoutingKey = "session.deleted",
            Payload = JsonSerializer.Serialize(new
            {
                Id = session.Id,
                Title = session.Title,
                EventType = "SessionDeleted",
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

        return ApiResponse<object>.SuccessResponse("Delete Session Successfully");
    }
}

