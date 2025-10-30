using LessonService.Application.Abstractions.Messaging;
using LessonService.Application.IServices;
using LessonService.Domain.Commons;
using LessonService.Domain.Interfaces;

namespace LessonService.Application.Features.Sessions.UpdateSession;

public class UpdateSessionCommandHandler : ICommandHandler<UpdateSessionCommand>
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly IRedisService _redisService;

    public UpdateSessionCommandHandler(IUnitOfWork unitOfWork, IRedisService redisService)
    {
        _unitOfWork = unitOfWork;
        _redisService = redisService;
    }

    public async Task<ApiResponse> Handle(UpdateSessionCommand command, CancellationToken cancellationToken)
    {
        var session = await _unitOfWork.SessionRepository.GetByIdAsync(command.Id);

        if (session is null)
        {
            return ApiResponse.FailureResponse("Session not found", 404);
        }

        // Update only provided fields
        if (!string.IsNullOrWhiteSpace(command.Title))
            session.Title = command.Title;

        if (command.Description is not null)
            session.Description = command.Description;

        if (command.Position.HasValue)
            session.Position = command.Position.Value;

        if (command.DurationMinutes.HasValue)
            session.DurationMinutes = command.DurationMinutes.Value;

        session.UpdatedAt = DateTime.UtcNow;

        _unitOfWork.SessionRepository.Update(session);

        try
        {
            await _unitOfWork.SaveChangesAsync(cancellationToken);
            
            // Invalidate cache
            var cacheKey = $"session:{command.Id}";
            await _redisService.RemoveAsync(cacheKey);
        }
        catch (Exception e)
        {
            return ApiResponse.FailureResponse(e.Message, 500);
        }

        return ApiResponse.SuccessResponse("Update Session Successfully");
    }
}


