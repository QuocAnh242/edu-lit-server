using LessonService.Application.Abstractions.Messaging;
using LessonService.Domain.Commons;
using LessonService.Domain.Entities;
using LessonService.Domain.Interfaces;
using System.Text.Json;

namespace LessonService.Application.Features.Syllabus.DeleteSyllabus;

public class DeleteSyllabusCommandHandler : ICommandHandler<DeleteSyllabusCommand>
{
    private readonly IUnitOfWork _unitOfWork;

    public DeleteSyllabusCommandHandler(IUnitOfWork unitOfWork)
    {
        _unitOfWork = unitOfWork;
    }

    public async Task<ApiResponse<object>> Handle(DeleteSyllabusCommand command, CancellationToken cancellationToken)
    {
        var syllabus = await _unitOfWork.SyllabusRepository.GetByIdAsync(command.Id);

        if (syllabus is null)
        {
            return ApiResponse<object>.FailureResponse("Syllabus not found", 404);
        }

        _unitOfWork.SyllabusRepository.Remove(syllabus);

        // Create outbox message
        var outboxMessage = new OutboxMessage
        {
            Id = Guid.NewGuid(),
            Type = "SyllabusDeleted",
            Exchange = "syllabus-events",
            RoutingKey = "syllabus.deleted",
            Payload = JsonSerializer.Serialize(new
            {
                Id = syllabus.Id,
                Title = syllabus.Title,
                EventType = "SyllabusDeleted",
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

        return ApiResponse<object>.SuccessResponse("Delete Syllabus Successfully");
    }
}

