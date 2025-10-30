using AuthService.Application.DTOs;
using AuthService.Application.DTOs.Response;
using AuthService.Domain.Interfaces;
using AuthService.Application.Abstractions.Messaging;

namespace AuthService.Application.Services.Auth.Handlers;

public sealed class RegisterHandler(IAuthRepository repo, IJwtTokenGenerator jwt, IOutbox outbox)
    : ICommandHandler<RegisterCommand, UserDto>
{
    public async Task<ApiResponse<UserDto>> Handle(RegisterCommand command, CancellationToken cancellationToken)
    {
        var user = await repo.RegisterAsync(command.Username, command.Email, command.Password, command.FullName);
        if (user is null)
            return ApiResponse<UserDto>.FailureResponse("Registration failed", 400);

        await outbox.EnqueueAsync("auth.user.registered", new
        {
            user.Id,
            user.Username,
            user.Email,
            user.FullName,
            user.RoleId,
            user.CreatedAt
        }, cancellationToken);

        var dto = new UserDto(user)
        {
            Token = jwt.GenerateToken(user)
        };

        return ApiResponse<UserDto>.SuccessResponse(dto, "Register successfully ");
    }
}