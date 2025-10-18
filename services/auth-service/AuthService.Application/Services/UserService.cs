using AuthService.Application.DTOs;
using AuthService.Application.DTOs.Response;
using AuthService.Application.Services.Interfaces;
using AuthService.Domain.Interfaces;
using System;
using System.Linq;
using System.Threading.Tasks;
using System.Collections.Generic;

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
            return ApiResponse<UserDto>.SuccessResponse(new UserDto(user), "Get UserById Successfully!");
        }

        public async Task<ApiResponse<List<UserDto>>> GetAllAsync()
        {
            var users = await _repo.GetAllAsync();
            return ApiResponse<List<UserDto>>.SuccessResponse(users.Select(u => new UserDto(u)).ToList(), "Get All Users Successfully!");
        }

        public async Task<ApiResponse<PageResponse<UserDto>>> GetPagedAsync(int page, int size)
        {
            var (items, totalCount) = await _repo.GetPagedAsync(page, size);

            var dtoItems = items.Select(u => new UserDto(u)).ToList();
            var totalPages = (int)Math.Ceiling(totalCount / (double)size);

            var pageResponse = new PageResponse<UserDto>
            {
                Content = dtoItems,
                TotalElements = totalCount,
                TotalPages = totalPages,
                Page = page,
                Size = size
            };

            return ApiResponse<PageResponse<UserDto>>.SuccessResponse(pageResponse, "Get Users (Paged) Successfully!");
        }

        public async Task<ApiResponse<UserDto>> CreateAsync(UserDto dto)
        {
            var user = dto.ToEntity();
            await _repo.AddAsync(user);
            return ApiResponse<UserDto>.SuccessResponse(new UserDto(user), "Create User Successfully!");
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
            return ApiResponse<UserDto>.SuccessResponse(new UserDto(user), "Update User Successfully!");
        }

        public async Task<ApiResponse<bool>> DeleteAsync(Guid id)
        {
            await _repo.DeleteAsync(id);
            return ApiResponse<bool>.SuccessResponse(true, "Delete Successfully!");
        }
    }
}
