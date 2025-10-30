using LessonService.Application.Abstractions.Messaging;
using LessonService.Application.IServices;
using LessonService.Domain.Commons;
using LessonService.Domain.Interfaces;

namespace LessonService.Application.Features.Sessions.DeleteSession;

public class DeleteSessionCommandHandler : ICommandHandler<DeleteSessionCommand>
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly IRedisService _redisService;

    public DeleteSessionCommandHandler(IUnitOfWork unitOfWork, IRedisService redisService)
    {
        _unitOfWork = unitOfWork;
        _redisService = redisService;
    }

    public async Task<ApiResponse> Handle(DeleteSessionCommand command, CancellationToken cancellationToken)
    {
        var session = await _unitOfWork.SessionRepository.GetByIdAsync(command.Id);

        if (session is null)
        {
            return ApiResponse.FailureResponse("Session not found", 404);
        }

        _unitOfWork.SessionRepository.Remove(session);

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

        return ApiResponse.SuccessResponse("Delete Session Successfully");
    }
}


