using AuthService.Application.DTOs;
using AuthService.Application.DTOs.Response;
using AuthService.Application.Services.Common;
using AuthService.Domain.Entities;
using AuthService.Domain.Interfaces;
using AuthService.Application.Abstractions.Messaging;
using System;

namespace AuthService.Application.Services.Auth.Handlers;

public sealed class RegisterHandler : ICommandHandler<RegisterCommand, UserDto>
{
    private readonly IAuthRepository _repo;
    private readonly IJwtTokenGenerator _jwt;
    private readonly IRefreshTokenRepository _refreshTokenRepository;
    private readonly IOutbox _outbox;
    private readonly IUnitOfWork _unitOfWork;

    public RegisterHandler(
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

    public async Task<ApiResponse<UserDto>> Handle(RegisterCommand command, CancellationToken cancellationToken)
    {
        try
        {
            // Validate password
            var (isValid, errorMessage) = PasswordValidator.Validate(command.Password);
            if (!isValid)
            {
                return ApiResponse<UserDto>.FailureResponse(errorMessage!, 400);
            }

            var user = await _repo.RegisterAsync(command.Username, command.Email, command.Password, command.FullName);
            if (user is null)
                return ApiResponse<UserDto>.FailureResponse("Registration failed", 400);

            // Ensure Role is loaded for token generation and DTO mapping
            if (user.Role == null && user.RoleId.HasValue)
            {
                // Role should be loaded by RegisterAsync, but if not, we need to reload
                // This shouldn't happen, but adding as safety check
                throw new InvalidOperationException($"User role not loaded for user {user.Id}");
            }

            // Publish event - use "auth.user.created" to match consumer expectations
            await _outbox.EnqueueAsync("auth.user.created", new
            {
                id = user.Id,
                username = user.Username,
                email = user.Email,
                fullName = user.FullName,
                roleId = user.RoleId,
                roleName = user.Role?.Name ?? "STUDENT",
                createdAt = user.CreatedAt
            }, cancellationToken);

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

            return ApiResponse<UserDto>.SuccessResponse(dto, "Register successfully");
        }
        catch (Application.Exceptions.AuthException)
        {
            // Re-throw AuthException so it can be handled by the controller
            throw;
        }
        catch (Exception ex)
        {
            // Log and wrap unexpected exceptions
            throw new InvalidOperationException($"Registration failed: {ex.Message}", ex);
        }
    }
}