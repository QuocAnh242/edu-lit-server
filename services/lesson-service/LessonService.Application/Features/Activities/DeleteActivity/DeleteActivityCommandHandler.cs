using LessonService.Application.Abstractions.Messaging;
using LessonService.Application.IServices;
using LessonService.Domain.Commons;
using LessonService.Domain.Interfaces;

namespace LessonService.Application.Features.Activities.DeleteActivity;

public class DeleteActivityCommandHandler : ICommandHandler<DeleteActivityCommand>
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly IRedisService _redisService;

    public DeleteActivityCommandHandler(IUnitOfWork unitOfWork, IRedisService redisService)
    {
        _unitOfWork = unitOfWork;
        _redisService = redisService;
    }

    public async Task<ApiResponse> Handle(DeleteActivityCommand command, CancellationToken cancellationToken)
    {
        var activity = await _unitOfWork.ActivityRepository.GetByIdAsync(command.Id);

        if (activity is null)
        {
            return ApiResponse.FailureResponse("Activity not found", 404);
        }

        _unitOfWork.ActivityRepository.Remove(activity);

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

        return ApiResponse.SuccessResponse("Delete Activity Successfully");
    }
}


