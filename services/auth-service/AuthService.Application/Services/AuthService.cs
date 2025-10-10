using AuthService.Application.DTOs;
using AuthService.Application.DTOs.Response;
using AuthService.Application.Enums;
using AuthService.Application.Exceptions;
using AuthService.Application.Services.Interfaces;
using AuthService.Domain.Interfaces;
using Microsoft.Extensions.Logging;

namespace AuthService.Application.Services
{
    public class AuthService : IAuthService
    {
        private readonly IAuthRepository _authRepository;
        private readonly IJwtTokenGenerator _jwtTokenGenerator;
        private readonly ILogger<AuthService> _logger;

        public AuthService(IAuthRepository authRepository, IJwtTokenGenerator jwtTokenGenerator, ILogger<AuthService> logger)
        {
            _authRepository = authRepository;
            _jwtTokenGenerator = jwtTokenGenerator;
            _logger = logger;
        }

        public async Task<ApiResponse<UserDto>> LoginAsync(LoginCommand command)
        {
            try
            {
                var user = await _authRepository.GetByUsernameAsync(command.Username);
                if (user == null || !BCrypt.Net.BCrypt.Verify(command.Password, user.Password))
                    throw new AuthException(AuthErrorCode.InvalidCredentials, "Invalid username or password!");
                var token = _jwtTokenGenerator.GenerateToken(user);

                var userDto = new UserDto
                {
                    Id = user.Id,
                    Username = user.Username,
                    Email = user.Email,
                    FullName = user.FullName,
                    RoleName = user.Role?.Name ?? "User",
                    Token = token
                };

                return ApiResponse<UserDto>.SuccessResponse(userDto, "Login successfully!");
            }
            catch (AuthException ex)
            {
                return ApiResponse<UserDto>.FailureResponse(ex.Message, (int)ex.ErrorCode);
            }
            catch (Exception)
            {
                return ApiResponse<UserDto>.FailureResponse("An unknown error occurred!", (int)AuthErrorCode.Unknown);
            }
        }

        public async Task<ApiResponse<UserDto>> RegisterAsync(RegisterCommand command)
        {
            try
            {
                var hashedPassword = BCrypt.Net.BCrypt.HashPassword(command.Password);

                var user = await _authRepository.RegisterAsync(
                    command.Username,
                    command.Email,
                    hashedPassword,
                    command.FullName
                );

                if (user == null)
                    throw new AuthException(AuthErrorCode.RegistrationFailed, "Failed to register user!");

                var token = _jwtTokenGenerator.GenerateToken(user);
                var userDto = new UserDto
                {
                    Id = user.Id,
                    Username = user.Username,
                    Email = user.Email,
                    FullName = user.FullName,
                    RoleName = user.Role?.Name ?? "User",
                    Token = token
                };

                return ApiResponse<UserDto>.SuccessResponse(userDto, "Registration successfully!");
            }
            catch (AuthException ex)
            {
                return ApiResponse<UserDto>.FailureResponse(ex.Message, (int)ex.ErrorCode);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An unexpected error occurred during registration.");
                if (ex.Message.Contains("duplicate", StringComparison.OrdinalIgnoreCase))
                    return ApiResponse<UserDto>.FailureResponse("User already exists!", (int)AuthErrorCode.UserAlreadyExists);

                return ApiResponse<UserDto>.FailureResponse("An unknown error occurred!", (int)AuthErrorCode.Unknown);
            }
        }
    }
}
