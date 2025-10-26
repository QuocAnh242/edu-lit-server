using AuthService.Application.DTOs;
using AuthService.Application.DTOs.Response;
using AuthService.Application.Services.Interfaces;
using AuthService.Domain.Interfaces;
using System;
using System.Linq;
using System.Threading.Tasks;
using System.Collections.Generic;
using AuthService.Domain.Entities.ReadModels;

namespace AuthService.Application.Services
{
    public class UserService : IUserService
    {
        private readonly IUserRepository _repo;                 // write (PostgreSQL / EF)
        private readonly IUserReadRepository _readRepo;         // read (MongoDB)
        private readonly IUserReadProjection _projection;       // project writes -> Mongo

        public UserService(IUserRepository repo, IUserReadRepository readRepo, IUserReadProjection projection)
        {
            _repo = repo;
            _readRepo = readRepo;
            _projection = projection;
        }

        public async Task<ApiResponse<UserDto>> GetByIdAsync(Guid id)
        {
            var read = await _readRepo.GetByIdAsync(id);
            if (read == null) return ApiResponse<UserDto>.FailureResponse("Not found", 404);
            return ApiResponse<UserDto>.SuccessResponse(ToDto(read), "Get UserById Successfully!");
        }

        public async Task<ApiResponse<List<UserDto>>> GetAllAsync()
        {
            var users = await _readRepo.GetAllAsync();
            return ApiResponse<List<UserDto>>.SuccessResponse(users.Select(ToDto).ToList(), "Get All Users Successfully!");
        }

        public async Task<ApiResponse<PageResponse<UserDto>>> GetPagedAsync(int page, int size)
        {
            var (items, totalCount) = await _readRepo.GetPagedAsync(page, size);

            var dtoItems = items.Select(ToDto).ToList();
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
            await _repo.AddAsync(user);                 // write to PostgreSQL
            await _projection.UpsertAsync(user);       // mirror into Mongo
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
            await _repo.UpdateAsync(user);             // write to PostgreSQL
            await _projection.UpsertAsync(user);       // mirror into Mongo
            return ApiResponse<UserDto>.SuccessResponse(new UserDto(user), "Update User Successfully!");
        }

        public async Task<ApiResponse<bool>> DeleteAsync(Guid id)
        {
            await _repo.DeleteAsync(id);               // write to PostgreSQL
            await _projection.DeleteAsync(id);         // delete from Mongo
            return ApiResponse<bool>.SuccessResponse(true, "Delete Successfully!");
        }

        private static UserDto ToDto(UserReadModel read)
            => new UserDto
            {
                Id = read.Id,
                Username = read.Username,
                Email = read.Email,
                FullName = read.FullName,
                RoleId = read.RoleId
            };
    }
}
