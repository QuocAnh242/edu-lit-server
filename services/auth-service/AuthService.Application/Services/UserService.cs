using AuthService.Application.DTOs;
using AuthService.Application.DTOs.Response;
using AuthService.Application.Services.Interfaces;
using AuthService.Domain.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AuthService.Application.Services
{
    public class UserService : IUserService
    {
        private readonly IUserRepository _repo;
        public UserService(IUserRepository repo) => _repo = repo;

        public async Task<ApiResponse<UserDto>> GetByIdAsync(Guid id)
        {
            var user = await _repo.GetByIdAsync(id);
            if (user == null) return ApiResponse<UserDto>.FailureResponse("Not found", 404);
            return ApiResponse<UserDto>.SuccessResponse(new UserDto(user));
        }

        public async Task<ApiResponse<List<UserDto>>> GetAllAsync()
        {
            var users = await _repo.GetAllAsync();
            return ApiResponse<List<UserDto>>.SuccessResponse(users.Select(u => new UserDto(u)).ToList());
        }

        public async Task<ApiResponse<UserDto>> CreateAsync(UserDto dto)
        {
            var user = dto.ToEntity();
            await _repo.AddAsync(user);
            return ApiResponse<UserDto>.SuccessResponse(new UserDto(user));
        }

        public async Task<ApiResponse<UserDto>> UpdateAsync(Guid id, UserDto dto)
        {
            var user = await _repo.GetByIdAsync(id);
            if (user == null) return ApiResponse<UserDto>.FailureResponse("Not found", 404);
            user.Username = dto.Username;
            user.Email = dto.Email;
            user.FullName = dto.FullName;
            user.RoleId = dto.RoleId;
            await _repo.UpdateAsync(user);
            return ApiResponse<UserDto>.SuccessResponse(new UserDto(user));
        }

        public async Task<ApiResponse<bool>> DeleteAsync(Guid id)
        {
            await _repo.DeleteAsync(id);
            return ApiResponse<bool>.SuccessResponse(true);
        }
    }
}
