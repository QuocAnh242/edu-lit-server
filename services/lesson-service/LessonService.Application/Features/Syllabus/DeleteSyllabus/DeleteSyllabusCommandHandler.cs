using LessonService.Application.Abstractions.Messaging;
using LessonService.Application.IServices;
using LessonService.Domain.Commons;
using LessonService.Domain.Interfaces;

namespace LessonService.Application.Features.Syllabus.DeleteSyllabus;

public class DeleteSyllabusCommandHandler : ICommandHandler<DeleteSyllabusCommand>
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly IRedisService _redisService;

    public DeleteSyllabusCommandHandler(IUnitOfWork unitOfWork, IRedisService redisService)
    {
        _unitOfWork = unitOfWork;
        _redisService = redisService;
    }

    public async Task<ApiResponse> Handle(DeleteSyllabusCommand command, CancellationToken cancellationToken)
    {
        var syllabus = await _unitOfWork.SyllabusRepository.GetByIdAsync(command.Id);

        if (syllabus is null)
        {
            return ApiResponse.FailureResponse("Syllabus not found", 404);
        }

        _unitOfWork.SyllabusRepository.Remove(syllabus);

        try
        {
            await _unitOfWork.SaveChangesAsync(cancellationToken);
            
            // Invalidate cache
            var cacheKey = $"syllabus:{command.Id.ToString()}";
            await _redisService.RemoveAsync(cacheKey);
        }
        catch (Exception e)
        {
            return ApiResponse.FailureResponse(e.Message, 500);
        }

        return ApiResponse.SuccessResponse("Delete Syllabus Successfully");
    }
}
