using LessonService.Application.Abstractions.Messaging;
using LessonService.Application.IServices;
using LessonService.Domain.Commons;
using LessonService.Domain.Interfaces;

namespace LessonService.Application.Features.LessonContexts.DeleteLessonContext;

public class DeleteLessonContextCommandHandler : ICommandHandler<DeleteLessonContextCommand>
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly IRedisService _redisService;

    public DeleteLessonContextCommandHandler(IUnitOfWork unitOfWork, IRedisService redisService)
    {
        _unitOfWork = unitOfWork;
        _redisService = redisService;
    }

    public async Task<ApiResponse> Handle(DeleteLessonContextCommand command, CancellationToken cancellationToken)
    {
        var lessonContext = await _unitOfWork.LessonContextRepository.GetByIdAsync(command.Id);

        if (lessonContext is null)
        {
            return ApiResponse.FailureResponse("LessonContext not found", 404);
        }

        _unitOfWork.LessonContextRepository.Remove(lessonContext);

        try
        {
            await _unitOfWork.SaveChangesAsync(cancellationToken);
            
            // Invalidate cache
            var cacheKey = $"lessoncontext:{command.Id}";
            await _redisService.RemoveAsync(cacheKey);
        }
        catch (Exception e)
        {
            return ApiResponse.FailureResponse(e.Message, 500);
        }

        return ApiResponse.SuccessResponse("Delete LessonContext Successfully");
    }
}


