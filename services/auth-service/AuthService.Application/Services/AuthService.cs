using AuthService.Application.DTOs;
using AuthService.Application.DTOs.Response;
using AuthService.Application.Services.Interfaces;
using AuthService.Domain.Interfaces;
using System.Threading.Tasks;

namespace AuthService.Application.Services
{
    public class AuthService : IAuthService
    {
        private readonly IAuthRepository _authRepository;

        public AuthService(IAuthRepository authRepository)
        {
            _authRepository = authRepository;
        }

        public async Task<ApiResponse<UserDto>> LoginAsync(LoginCommand command)
        {
            var user = await _authRepository.LoginAsync(command.Username, command.Password);
            if (user == null)
                return ApiResponse<UserDto>.FailureResponse("Invalid credentials");

            var userDto = new UserDto
            {
                Id = user.Id,
                Username = user.Username,
                Email = user.Email,
                FullName = user.FullName
            };
            return ApiResponse<UserDto>.SuccessResponse(userDto);
        }

        public async Task<ApiResponse<UserDto>> RegisterAsync(RegisterCommand command)
        {
            var user = await _authRepository.RegisterAsync(command.Username, command.Email, command.Password, command.FullName);
            if (user == null)
                return ApiResponse<UserDto>.FailureResponse("Registration failed");

            var userDto = new UserDto
            {
                Id = user.Id,
                Username = user.Username,
                Email = user.Email,
                FullName = user.FullName
            };
            return ApiResponse<UserDto>.SuccessResponse(userDto);
        }
    }
}
