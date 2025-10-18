using AuthService.Application.DTOs;
using AuthService.Application.DTOs.Response;
using AuthService.Application.Queries;
namespace AuthService.Application.Services.Interfaces;
public interface IAuthService
{
    Task<ApiResponse<UserDto>> LoginAsync(LoginCommand command);
    Task<ApiResponse<UserDto>> RegisterAsync(RegisterCommand command);
}
