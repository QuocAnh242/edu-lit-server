using AssessmentService.Domain.Commons;

namespace AssessmentService.Application.Abstractions.Messaging
{
    public interface ICommandHandler<in TCommand>
        where TCommand : ICommand
    {
        Task<ObjectResponse<object>> Handle(TCommand command, CancellationToken cancellationToken);
    }

    public interface ICommandHandler<in TCommand, TResponse>
        where TCommand : ICommand<TResponse>
    {
        Task<ObjectResponse<TResponse>> Handle(TCommand command, CancellationToken cancellationToken);
    }
}
