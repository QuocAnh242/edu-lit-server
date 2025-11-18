using LessonService.Application.Abstractions.Messaging;
using LessonService.Domain.Commons;
using LessonService.Domain.Entities;
using LessonService.Domain.Interfaces;
using System.Text.Json;

namespace LessonService.Application.Features.Courses.DeleteCourse;

public class DeleteCourseCommandHandler : ICommandHandler<DeleteCourseCommand>
{
    private readonly IUnitOfWork _unitOfWork;

    public DeleteCourseCommandHandler(IUnitOfWork unitOfWork)
    {
        _unitOfWork = unitOfWork;
    }

    public async Task<ApiResponse<object>> Handle(DeleteCourseCommand command, CancellationToken cancellationToken)
    {
        var course = await _unitOfWork.CourseRepository.GetByIdAsync(command.Id);

        if (course is null)
        {
            return ApiResponse<object>.FailureResponse("Course not found", 404);
        }

        _unitOfWork.CourseRepository.Remove(course);

        // Create outbox message
        var outboxMessage = new OutboxMessage
        {
            Id = Guid.NewGuid(),
            Type = "CourseDeleted",
            Exchange = "coursera-events",
            RoutingKey = "course.deleted",
            Payload = JsonSerializer.Serialize(new
            {
                Id = course.Id,
                Title = course.Title,
                EventType = "CourseDeleted",
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

        return ApiResponse<object>.SuccessResponse("Delete Course Successfully");
    }
}

