using LessonService.Application.Abstractions.Messaging;
using LessonService.Domain.Commons;
using LessonService.Domain.Entities;
using LessonService.Domain.Interfaces;
using System.Text.Json;

namespace LessonService.Application.Features.Courses.UpdateCourse;

public class UpdateCourseCommandHandler : ICommandHandler<UpdateCourseCommand>
{
    private readonly IUnitOfWork _unitOfWork;

    public UpdateCourseCommandHandler(IUnitOfWork unitOfWork)
    {
        _unitOfWork = unitOfWork;
    }

    public async Task<ApiResponse<object>> Handle(UpdateCourseCommand command, CancellationToken cancellationToken)
    {
        var course = await _unitOfWork.CourseRepository.GetByIdAsync(command.Id);

        if (course is null)
        {
            return ApiResponse<object>.FailureResponse("Course not found", 404);
        }

        // Update only provided fields
        if (!string.IsNullOrWhiteSpace(command.CourseCode))
            course.CourseCode = command.CourseCode;

        if (!string.IsNullOrWhiteSpace(command.Title))
            course.Title = command.Title;

        if (command.Description is not null)
            course.Description = command.Description;

        course.UpdatedAt = DateTime.UtcNow;

        _unitOfWork.CourseRepository.Update(course);

        // Create outbox message
        var outboxMessage = new OutboxMessage
        {
            Id = Guid.NewGuid(),
            Type = "CourseUpdated",
            Exchange = "coursera-events",
            RoutingKey = "course.updated",
            Payload = JsonSerializer.Serialize(new
            {
                Id = course.Id,
                SyllabusId = course.SyllabusId,
                CourseCode = course.CourseCode,
                Title = course.Title,
                Description = course.Description,
                UpdatedAt = course.UpdatedAt,
                EventType = "CourseUpdated",
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

        return ApiResponse<object>.SuccessResponse("Update Course Successfully");
    }
}

