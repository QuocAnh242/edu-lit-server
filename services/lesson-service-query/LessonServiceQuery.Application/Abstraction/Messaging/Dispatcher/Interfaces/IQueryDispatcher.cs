using LessonService.Domain.Commons;

namespace LessonServiceQuery.Application.Abstractions.Messaging.Dispatcher.Interfaces
{
    public interface IQueryDispatcher
    {
        Task<ApiResponse<TResponse>> Query<TResponse>(
            IQuery<TResponse> query,
            CancellationToken ct = default);
    }
}