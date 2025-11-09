using System.Threading;

namespace AuthService.Application.Abstractions.Messaging.Dispatcher.Interfaces
{
    public interface IQueryDispatcher
    {
        Task<AuthService.Application.DTOs.Response.ApiResponse<TResponse>> Query<TResponse>(
            IQuery<TResponse> query,
            CancellationToken ct = default);
    }
}