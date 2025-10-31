using LessonService.Application.Abstractions.Messaging;
using LessonService.Application.IServices;
using LessonService.Domain.Commons;
using LessonService.Domain.Interfaces;

namespace LessonService.Application.Features.Activities.UpdateActivity;

public class UpdateActivityCommandHandler : ICommandHandler<UpdateActivityCommand>
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly IRedisService _redisService;

    public UpdateActivityCommandHandler(IUnitOfWork unitOfWork, IRedisService redisService)
    {
        _unitOfWork = unitOfWork;
        _redisService = redisService;
    }

    public async Task<ApiResponse> Handle(UpdateActivityCommand command, CancellationToken cancellationToken)
    {
        var activity = await _unitOfWork.ActivityRepository.GetByIdAsync(command.Id);

        if (activity is null)
        {
            return ApiResponse.FailureResponse("Activity not found", 404);
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

        try
        {
            await _unitOfWork.SaveChangesAsync(cancellationToken);
            
            // Invalidate cache
            var cacheKey = $"activity:{command.Id}";
            await _redisService.RemoveAsync(cacheKey);
        }
        catch (Exception e)
        {
            return ApiResponse.FailureResponse(e.Message, 500);
        }

        return ApiResponse.SuccessResponse("Update Activity Successfully");
    }
}


