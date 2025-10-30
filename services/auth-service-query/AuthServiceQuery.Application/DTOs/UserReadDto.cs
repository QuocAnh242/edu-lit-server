namespace AuthService.Application.DTOs
{
    public sealed class UserReadDto
    {
        public Guid Id { get; init; }
        public string Username { get; init; } = string.Empty;
        public string Email { get; init; } = string.Empty;
        public string? FullName { get; init; }
        public Guid? RoleId { get; init; }
        public string? RoleName { get; init; }
        public DateTime? CreatedAt { get; init; }
    }
}