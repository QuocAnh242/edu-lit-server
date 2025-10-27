using QuestionService.Domain.Commons;

namespace QuestionService.Application.Abstractions.Messaging
{
    public interface IQueryHandler<in TQuery, TResponse>
        where TQuery : IQuery<TResponse>
    {
        Task<ApiResponse<TResponse>> Handle(TQuery query, CancellationToken cancellationToken);
    }
}

