using System;

namespace AuthService.Application.DTOs
{
    public class UserDto
    {
        public Guid Id { get; set; }
        public string Username { get; set; } = null!;
        public string Email { get; set; } = null!;
        public string? FullName { get; set; }
        public string? RoleName { get; set; }
        public string? Token { get; set; } // JWT token property added
    }
}
