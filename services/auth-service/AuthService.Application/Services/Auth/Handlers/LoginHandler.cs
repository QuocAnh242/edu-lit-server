using AuthService.Application.DTOs;
using AuthService.Application.DTOs.Response;
using AuthService.Domain.Entities;
using AuthService.Domain.Interfaces;
using AuthService.Application.Abstractions.Messaging;
using System;

namespace AuthService.Application.Services.Auth.Handlers;

public sealed class LoginHandler : ICommandHandler<LoginCommand, UserDto>
{
    private readonly IAuthRepository _repo;
    private readonly IJwtTokenGenerator _jwt;
    private readonly IRefreshTokenRepository _refreshTokenRepository;
    private readonly IOutbox _outbox;
    private readonly IUnitOfWork _unitOfWork;

    public LoginHandler(
        IAuthRepository repo, 
        IJwtTokenGenerator jwt,
        IRefreshTokenRepository refreshTokenRepository,
        IOutbox outbox,
        IUnitOfWork unitOfWork)
    {
        _repo = repo;
        _jwt = jwt;
        _refreshTokenRepository = refreshTokenRepository;
        _outbox = outbox;
        _unitOfWork = unitOfWork;
    }

    public async Task<ApiResponse<UserDto>> Handle(LoginCommand command, CancellationToken cancellationToken)
    {
        // Connect to repository to validate user credentials
        var user = await _repo.LoginAsync(command.Username, command.Password);
        if (user is null)
            return ApiResponse<UserDto>.FailureResponse("Invalid username or password", 401);

        // Generate token pair
        var (accessToken, refreshToken) = _jwt.GenerateTokenPair(user);

        // Save refresh token
        var refreshTokenEntity = new RefreshToken
        {
            Id = Guid.NewGuid(),
            UserId = user.Id,
            Token = refreshToken,
            ExpiresAt = DateTime.UtcNow.AddDays(7),
            CreatedAt = DateTime.UtcNow,
            IsRevoked = false
        };

        await _refreshTokenRepository.AddAsync(refreshTokenEntity);

        var dto = new UserDto(user)
        {
            Token = accessToken,
            RefreshToken = refreshToken
        };

        return ApiResponse<UserDto>.SuccessResponse(dto, "Login successful");
    }
}