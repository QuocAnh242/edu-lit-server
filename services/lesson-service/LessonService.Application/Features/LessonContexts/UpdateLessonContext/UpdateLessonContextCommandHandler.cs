using LessonService.Application.Abstractions.Messaging;
using LessonService.Application.IServices;
using LessonService.Domain.Commons;
using LessonService.Domain.Interfaces;

namespace LessonService.Application.Features.LessonContexts.UpdateLessonContext;

public class UpdateLessonContextCommandHandler : ICommandHandler<UpdateLessonContextCommand>
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly IRedisService _redisService;

    public UpdateLessonContextCommandHandler(IUnitOfWork unitOfWork, IRedisService redisService)
    {
        _unitOfWork = unitOfWork;
        _redisService = redisService;
    }

    public async Task<ApiResponse> Handle(UpdateLessonContextCommand command, CancellationToken cancellationToken)
    {
        var lessonContext = await _unitOfWork.LessonContextRepository.GetByIdAsync(command.Id);

        if (lessonContext is null)
        {
            return ApiResponse.FailureResponse("LessonContext not found", 404);
        }

        // Update only provided fields
        if (!string.IsNullOrWhiteSpace(command.LessonTitle))
            lessonContext.LessonTitle = command.LessonTitle;

        if (command.LessonContent is not null)
            lessonContext.LessonContent = command.LessonContent;

        if (command.Position.HasValue)
            lessonContext.Position = command.Position.Value;

        if (command.Level.HasValue)
            lessonContext.Level = command.Level.Value;

        lessonContext.UpdatedAt = DateTime.UtcNow;

        _unitOfWork.LessonContextRepository.Update(lessonContext);

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

        return ApiResponse.SuccessResponse("Update LessonContext Successfully");
    }
}


