using AutoMapper;
using FluentValidation;
using LessonService.Application.Abstractions.Messaging;
using LessonService.Domain.Commons;
using LessonService.Domain.Interfaces;

namespace LessonService.Application.Features.LessonContexts.CreateLessonContext;

public class CreateLessonContextCommandHandler : ICommandHandler<CreateLessonContextCommand, Guid>
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly IValidator<CreateLessonContextCommand> _validator;
    private readonly IMapper _mapper;

    public CreateLessonContextCommandHandler(IUnitOfWork unitOfWork, IValidator<CreateLessonContextCommand> validator, IMapper mapper)
    {
        _unitOfWork = unitOfWork;
        _validator = validator;
        _mapper = mapper;
    }

    public async Task<ApiResponse<Guid>> Handle(CreateLessonContextCommand command, CancellationToken cancellationToken)
    {
        var validationResult = await _validator.ValidateAsync(command, cancellationToken);
        if (!validationResult.IsValid)
        {
            var errors = validationResult.Errors
                .Select(e => new Error("LessonContext.Create.Validation", e.ErrorMessage))
                .ToList();
            return ApiResponse<Guid>.FailureResponse(errors.First().Message, 400);
        }

        // Check if Session exists
        var sessionExists = await _unitOfWork.SessionRepository.GetByIdAsync(command.SessionId);
        if (sessionExists is null)
        {
            return ApiResponse<Guid>.FailureResponse("Session not found", 404);
        }

        // Check if ParentLesson exists (if provided)
        if (command.ParentLessonId.HasValue)
        {
            var parentExists = await _unitOfWork.LessonContextRepository.GetByIdAsync(command.ParentLessonId.Value);
            if (parentExists is null)
            {
                return ApiResponse<Guid>.FailureResponse("Parent Lesson not found", 404);
            }
        }

        var lessonContext = _mapper.Map<Domain.Entities.LessonContext>(command);
        lessonContext.Id = Guid.NewGuid();
        lessonContext.CreatedAt = DateTime.UtcNow;

        await _unitOfWork.LessonContextRepository.AddAsync(lessonContext);

        try
        {
            await _unitOfWork.SaveChangesAsync(cancellationToken);
        }
        catch (Exception e)
        {
            return ApiResponse<Guid>.FailureResponse(e.Message, 500);
        }

        return ApiResponse<Guid>.SuccessResponse(lessonContext.Id, "Create LessonContext Successfully");
    }
}


