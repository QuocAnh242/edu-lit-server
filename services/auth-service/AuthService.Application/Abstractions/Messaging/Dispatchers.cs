using AuthService.Application.DTOs.Response;
using Microsoft.Extensions.DependencyInjection;

namespace AuthService.Application.Abstractions.Messaging;

public interface ICommandDispatcher
{
    Task<ApiResponse<object>> Send<TCommand>(TCommand command, CancellationToken ct = default) where TCommand : ICommand;
    Task<ApiResponse<TResponse>> Send<TCommand, TResponse>(TCommand command, CancellationToken ct = default) where TCommand : ICommand<TResponse>;
}

public interface IQueryDispatcher
{
    Task<ApiResponse<TResponse>> Query<TQuery, TResponse>(TQuery query, CancellationToken ct = default) where TQuery : IQuery<TResponse>;
}

internal sealed class CommandDispatcher(IServiceProvider sp) : ICommandDispatcher
{
    public async Task<ApiResponse<object>> Send<TCommand>(TCommand command, CancellationToken ct = default) where TCommand : ICommand
    {
        var handler = sp.GetRequiredService<ICommandHandler<TCommand>>();
        return await handler.Handle(command, ct);
    }

    public async Task<ApiResponse<TResponse>> Send<TCommand, TResponse>(TCommand command, CancellationToken ct = default) where TCommand : ICommand<TResponse>
    {
        var handler = sp.GetRequiredService<ICommandHandler<TCommand, TResponse>>();
        return await handler.Handle(command, ct);
    }
}

internal sealed class QueryDispatcher(IServiceProvider sp) : IQueryDispatcher
{
    public async Task<ApiResponse<TResponse>> Query<TQuery, TResponse>(TQuery query, CancellationToken ct = default) where TQuery : IQuery<TResponse>
    {
        var handler = sp.GetRequiredService<IQueryHandler<TQuery, TResponse>>();
        return await handler.Handle(query, ct);
    }
}