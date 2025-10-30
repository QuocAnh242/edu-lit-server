using AssessmentService.Domain.Commons;

namespace AssessmentService.Application.Abstractions.Messaging
{
    public interface IQueryHandler<in TQuery, TResponse>
        where TQuery : IQuery<TResponse>
    {
        Task<ObjectResponse<TResponse>> Handle(TQuery query, CancellationToken cancellationToken);
    }
}
