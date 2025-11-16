using AuthService.Application.Abstractions.Messaging;
using AuthService.Application.Abstractions.Messaging.Dispatcher.Interfaces;
using AuthService.Application.DTOs;
using AuthService.Application.DTOs.Response;
using AuthService.Application.Services.Role.Commands;
using AuthService.Application.Services.Users.Commands;
using AuthService.Domain.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace AuthService.Api.Controllers
{
    [Route("api/v1/[controller]")]
    [ApiController]
    public class SeedController : ControllerBase
    {
        private readonly ICommandDispatcher _commands;
        private readonly IRoleRepository _roleRepository;

        public SeedController(ICommandDispatcher commands, IRoleRepository roleRepository)
        {
            _commands = commands;
            _roleRepository = roleRepository;
        }

        // POST: /api/v1/seed/default-users
        [HttpPost("default-users")]
        public async Task<IActionResult> SeedDefaultUsers(CancellationToken ct)
        {
            var results = new List<object>();

            try
            {
                // Get or create ADMIN role
                var adminRole = await _roleRepository.GetByNameAsync("ADMIN");
                if (adminRole == null)
                {
                    var createAdminRoleCmd = new CreateRoleCommand { Name = "ADMIN" };
                    var adminRoleRes = await _commands.Send<CreateRoleCommand, Guid>(createAdminRoleCmd, ct);
                    if (adminRoleRes.Success)
                    {
                        adminRole = await _roleRepository.GetByIdAsync(adminRoleRes.Data);
                        results.Add(new { action = "Create ADMIN role", success = true, roleId = adminRoleRes.Data });
                    }
                    else
                    {
                        results.Add(new { action = "Create ADMIN role", success = false, error = adminRoleRes.Message });
                    }
                }
                else
                {
                    results.Add(new { action = "ADMIN role exists", success = true, roleId = adminRole.Id });
                }

                // Get or create TEACHER role
                var teacherRole = await _roleRepository.GetByNameAsync("TEACHER");
                if (teacherRole == null)
                {
                    var createTeacherRoleCmd = new CreateRoleCommand { Name = "TEACHER" };
                    var teacherRoleRes = await _commands.Send<CreateRoleCommand, Guid>(createTeacherRoleCmd, ct);
                    if (teacherRoleRes.Success)
                    {
                        teacherRole = await _roleRepository.GetByIdAsync(teacherRoleRes.Data);
                        results.Add(new { action = "Create TEACHER role", success = true, roleId = teacherRoleRes.Data });
                    }
                    else
                    {
                        results.Add(new { action = "Create TEACHER role", success = false, error = teacherRoleRes.Message });
                    }
                }
                else
                {
                    results.Add(new { action = "TEACHER role exists", success = true, roleId = teacherRole.Id });
                }

                // Create admin user (admin@server.com, username: admin404, password: 123456)
                if (adminRole != null)
                {
                    var createAdminUserCmd = new CreateUserCommand
                    {
                        Username = "admin404",
                        Email = "admin@server.com",
                        FullName = "System Administrator",
                        Password = "123456",
                        RoleId = adminRole.Id
                    };
                    var adminUserRes = await _commands.Send<CreateUserCommand, Guid>(createAdminUserCmd, ct);
                    results.Add(new { 
                        action = "Create admin user", 
                        success = adminUserRes.Success, 
                        userId = adminUserRes.Data,
                        message = adminUserRes.Message 
                    });
                }

                // Create teacher user (teacher@server.com, username: teacher404, password: 123456)
                if (teacherRole != null)
                {
                    var createTeacherUserCmd = new CreateUserCommand
                    {
                        Username = "teacher404",
                        Email = "teacher@server.com",
                        FullName = "Default Teacher",
                        Password = "123456",
                        RoleId = teacherRole.Id
                    };
                    var teacherUserRes = await _commands.Send<CreateUserCommand, Guid>(createTeacherUserCmd, ct);
                    results.Add(new { 
                        action = "Create teacher user", 
                        success = teacherUserRes.Success, 
                        userId = teacherUserRes.Data,
                        message = teacherUserRes.Message 
                    });
                }

                return Ok(ApiResponse<object>.SuccessResponse(results, "Default users seeded successfully"));
            }
            catch (Exception ex)
            {
                return StatusCode(500, ApiResponse<object>.FailureResponse($"Error seeding default users: {ex.Message}", 500));
            }
        }
    }
}

