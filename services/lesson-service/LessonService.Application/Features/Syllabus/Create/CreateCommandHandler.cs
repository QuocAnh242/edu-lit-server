using FluentValidation;
using LessonService.Application.Abstractions.Messaging;
using LessonService.Domain.Commons;
using LessonService.Domain.Interfaces;

namespace LessonService.Application.Features.Syllabus.Create
{
    public class CreateCommandHandler : ICommandHandler<CreateCommand, Guid>
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IValidator<CreateCommand> _createCommandValidator;

        public CreateCommandHandler(IUnitOfWork unitOfWork, IValidator<CreateCommand> createCommandValidator)
        {
            _unitOfWork = unitOfWork;
            _createCommandValidator = createCommandValidator;
        }

        public async Task<Result<Guid>> Handle(CreateCommand command, CancellationToken cancellationToken)
        {
            throw new NotImplementedException();
        }
    }
}
