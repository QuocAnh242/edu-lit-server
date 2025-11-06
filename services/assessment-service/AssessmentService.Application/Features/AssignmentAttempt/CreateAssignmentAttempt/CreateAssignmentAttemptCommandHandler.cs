using AssessmentService.Application.Abstractions.Messaging;
using AssessmentService.Application.IServices;
using AssessmentService.Domain.Commons;
using AssessmentService.Domain.Interfaces;
using AutoMapper;
using FluentValidation;

namespace AssessmentService.Application.Features.AssignmentAttempt.CreateAssignmentAttempt
{
    public class CreateAssignmentAttemptCommandHandler : ICommandHandler<CreateAssignmentAttemptCommand, int>
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IValidator<CreateAssignmentAttemptCommand> _validator;
        private readonly IMapper _mapper;
        private readonly IRedisService _redisService;
        private const string CacheKey = "assignmentAttempts:all";

        public CreateAssignmentAttemptCommandHandler(
            IUnitOfWork unitOfWork,
            IValidator<CreateAssignmentAttemptCommand> validator,
            IMapper mapper,
            IRedisService redisService)
        {
            _unitOfWork = unitOfWork;
            _validator = validator;
            _mapper = mapper;
            _redisService = redisService;
        }

        public async Task<ObjectResponse<int>> Handle(CreateAssignmentAttemptCommand command, CancellationToken cancellationToken)
        {
            // validation
            var validationResult = await _validator.ValidateAsync(command);
            if (!validationResult.IsValid)
            {
                var errors = validationResult.Errors
                    .Select(e => new Error("AssignmentAttempt.Create.Validation", e.ErrorMessage))
                    .ToList();
                return ObjectResponse<int>.Response("400", errors.First().Message, 0);
            }

            var assessment = await _unitOfWork.AssessmentRepository.GetByIdAsync(command.AssessmentId);
            if (assessment == null)
            {
                return ObjectResponse<int>.Response("404", "Assessment not found", 0);
            }

            // sẽ có hàm check valid userId sau khi có service của user
            //

            var assignmentAttempt = _mapper.Map<Domain.Entities.AssignmentAttempt>(command);
            await _unitOfWork.AssignmentAttemptRepository.AddAsync(assignmentAttempt);
            try
            {
                await _unitOfWork.SaveChangesAsync();

                // Invalidate cache
                await _redisService.RemoveAsync(CacheKey);
            }
            catch (Exception e)
            {
                return ObjectResponse<int>.Response("400", e.Message, 0);
            }
            return ObjectResponse<int>.SuccessResponse(assignmentAttempt.AttemptsId);
        }
    }
}
