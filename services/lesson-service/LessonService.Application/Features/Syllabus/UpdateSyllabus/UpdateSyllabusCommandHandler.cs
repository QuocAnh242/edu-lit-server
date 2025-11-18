using LessonService.Application.Abstractions.Messaging;
using LessonService.Domain.Commons;
using LessonService.Domain.Entities;
using LessonService.Domain.Interfaces;
using System.Text.Json;

namespace LessonService.Application.Features.Syllabus.UpdateSyllabus;

public class UpdateSyllabusCommandHandler : ICommandHandler<UpdateSyllabusCommand>
{
    private readonly IUnitOfWork _unitOfWork;

    public UpdateSyllabusCommandHandler(IUnitOfWork unitOfWork)
    {
        _unitOfWork = unitOfWork;
    }

    public async Task<ApiResponse<object>> Handle(UpdateSyllabusCommand command, CancellationToken cancellationToken)
    {
        var syllabus = await _unitOfWork.SyllabusRepository.GetByIdAsync(command.Id);

        if (syllabus is null)
        {
            return ApiResponse<object>.FailureResponse("Syllabus not found", 404);
        }

        // Update only provided fields
        if (!string.IsNullOrWhiteSpace(command.Title))
            syllabus.Title = command.Title;

        if (!string.IsNullOrWhiteSpace(command.AcademicYear))
            syllabus.AcademicYear = command.AcademicYear;

        if (command.Semester.HasValue)
            syllabus.Semester = command.Semester.Value;

        if (command.Description is not null)
            syllabus.Description = command.Description;

        if (command.IsActive.HasValue)
            syllabus.IsActive = command.IsActive.Value;

        syllabus.UpdatedAt = DateTime.UtcNow;

        _unitOfWork.SyllabusRepository.Update(syllabus);

        // Create outbox message
        var outboxMessage = new OutboxMessage
        {
            Id = Guid.NewGuid(),
            Type = "SyllabusUpdated",
            Exchange = "syllabus-events",
            RoutingKey = "syllabus.updated",
            Payload = JsonSerializer.Serialize(new
            {
                Id = syllabus.Id,
                Title = syllabus.Title,
                AcademicYear = syllabus.AcademicYear,
                Semester = syllabus.Semester.ToString(),
                Description = syllabus.Description,
                IsActive = syllabus.IsActive,
                UpdatedAt = syllabus.UpdatedAt,
                EventType = "SyllabusUpdated",
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

        return ApiResponse<object>.SuccessResponse("Syllabus updated successfully");
    }
}

