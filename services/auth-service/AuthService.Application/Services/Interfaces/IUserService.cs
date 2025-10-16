using AuthService.Application.DTOs;
using AuthService.Application.DTOs.Response;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AuthService.Application.Services.Interfaces
{
    public interface IUserService
    {
        Task<ApiResponse<UserDto>> GetByIdAsync(Guid id);
        Task<ApiResponse<List<UserDto>>> GetAllAsync();
        Task<ApiResponse<UserDto>> CreateAsync(UserDto dto);
        Task<ApiResponse<UserDto>> UpdateAsync(Guid id, UserDto dto);
        Task<ApiResponse<bool>> DeleteAsync(Guid id);
    }
}
