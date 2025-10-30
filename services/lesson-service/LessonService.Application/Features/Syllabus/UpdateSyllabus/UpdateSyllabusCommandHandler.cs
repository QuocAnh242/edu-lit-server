using LessonService.Application.Abstractions.Messaging;
using LessonService.Application.IServices;
using LessonService.Domain.Commons;
using LessonService.Domain.Interfaces;

namespace LessonService.Application.Features.Syllabus.UpdateSyllabus;

public class UpdateSyllabusCommandHandler : ICommandHandler<UpdateSyllabusCommand>
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly IRedisService _redisService;

    public UpdateSyllabusCommandHandler(IUnitOfWork unitOfWork, IRedisService redisService)
    {
        _unitOfWork = unitOfWork;
        _redisService = redisService;
    }

    public async Task<ApiResponse> Handle(UpdateSyllabusCommand command, CancellationToken cancellationToken)
    {
        var syllabus = await _unitOfWork.SyllabusRepository.GetByIdAsync(command.Id);

        if (syllabus is null)
        {
            return ApiResponse.FailureResponse("Syllabus not found", 404);
        }

        // Update only provided fields
        if (!string.IsNullOrWhiteSpace(command.Title))
            syllabus.Title = command.Title;

        if (!string.IsNullOrWhiteSpace(command.AcademicYear))
            syllabus.AcademicYear = command.AcademicYear;

        if (command.Semester.HasValue)
            syllabus.Semester = command.Semester.Value;

        if (command.Description is not null)
            syllabus.Description = command.Description;

        if (command.IsActive.HasValue)
            syllabus.IsActive = command.IsActive.Value;

        syllabus.UpdatedAt = DateTime.UtcNow;

        _unitOfWork.SyllabusRepository.Update(syllabus);

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

        return ApiResponse.SuccessResponse();
    }
}

