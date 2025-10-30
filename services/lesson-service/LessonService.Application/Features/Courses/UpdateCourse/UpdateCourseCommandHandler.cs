using LessonService.Application.Abstractions.Messaging;
using LessonService.Application.IServices;
using LessonService.Domain.Commons;
using LessonService.Domain.Interfaces;

namespace LessonService.Application.Features.Courses.UpdateCourse;

public class UpdateCourseCommandHandler : ICommandHandler<UpdateCourseCommand>
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly IRedisService _redisService;

    public UpdateCourseCommandHandler(IUnitOfWork unitOfWork, IRedisService redisService)
    {
        _unitOfWork = unitOfWork;
        _redisService = redisService;
    }

    public async Task<ApiResponse> Handle(UpdateCourseCommand command, CancellationToken cancellationToken)
    {
        var course = await _unitOfWork.CourseRepository.GetByIdAsync(command.Id);

        if (course is null)
        {
            return ApiResponse.FailureResponse("Course not found", 404);
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

        try
        {
            await _unitOfWork.SaveChangesAsync(cancellationToken);
            
            // Invalidate cache
            var cacheKey = $"course:{command.Id}";
            await _redisService.RemoveAsync(cacheKey);
        }
        catch (Exception e)
        {
            return ApiResponse.FailureResponse(e.Message, 500);
        }

        return ApiResponse.SuccessResponse("Update Course Successfully");
    }
}


