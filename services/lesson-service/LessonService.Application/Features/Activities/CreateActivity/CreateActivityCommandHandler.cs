using AutoMapper;
using FluentValidation;
using LessonService.Application.Abstractions.Messaging;
using LessonService.Domain.Commons;
using LessonService.Domain.Interfaces;

namespace LessonService.Application.Features.Activities.CreateActivity;

public class CreateActivityCommandHandler : ICommandHandler<CreateActivityCommand, Guid>
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly IValidator<CreateActivityCommand> _validator;
    private readonly IMapper _mapper;

    public CreateActivityCommandHandler(IUnitOfWork unitOfWork, IValidator<CreateActivityCommand> validator, IMapper mapper)
    {
        _unitOfWork = unitOfWork;
        _validator = validator;
        _mapper = mapper;
    }

    public async Task<ApiResponse<Guid>> Handle(CreateActivityCommand command, CancellationToken cancellationToken)
    {
        var validationResult = await _validator.ValidateAsync(command, cancellationToken);
        if (!validationResult.IsValid)
        {
            var errors = validationResult.Errors
                .Select(e => new Error("Activity.Create.Validation", e.ErrorMessage))
                .ToList();
            return ApiResponse<Guid>.FailureResponse(errors.First().Message, 400);
        }

        // Check if Session exists
        var sessionExists = await _unitOfWork.SessionRepository.GetByIdAsync(command.SessionId);
        if (sessionExists is null)
        {
            return ApiResponse<Guid>.FailureResponse("Session not found", 404);
        }

        var activity = _mapper.Map<Domain.Entities.Activity>(command);
        activity.Id = Guid.NewGuid();
        activity.CreatedAt = DateTime.UtcNow;

        await _unitOfWork.ActivityRepository.AddAsync(activity);

        try
        {
            await _unitOfWork.SaveChangesAsync(cancellationToken);
        }
        catch (Exception e)
        {
            return ApiResponse<Guid>.FailureResponse(e.Message, 500);
        }

        return ApiResponse<Guid>.SuccessResponse(activity.Id, "Create Activity Successfully");
    }
}


