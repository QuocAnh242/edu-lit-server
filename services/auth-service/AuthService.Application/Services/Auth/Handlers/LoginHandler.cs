using AuthService.Application.DTOs;
using AuthService.Application.DTOs.Response;
using AuthService.Domain.Interfaces;
using AuthService.Application.Abstractions.Messaging;

namespace AuthService.Application.Services.Auth.Handlers;

public sealed class LoginHandler(IAuthRepository repo, IJwtTokenGenerator jwt)
    : ICommandHandler<LoginCommand, UserDto>
{
    public async Task<ApiResponse<UserDto>> Handle(LoginCommand command, CancellationToken cancellationToken)
    {
        // Connect to repository to validate user credentials
        var user = await repo.LoginAsync(command.Username, command.Password);
        if (user is null)
            return ApiResponse<UserDto>.FailureResponse("Invalid username or password", 401);

        var dto = new UserDto(user)
        {
            Token = jwt.GenerateToken(user)
        };

        return ApiResponse<UserDto>.SuccessResponse(dto, "Login successful");
    }
}