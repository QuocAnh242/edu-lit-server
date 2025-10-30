namespace AuthService.Domain.Entities.ReadModels
{
    public sealed class UserReadModel
    {
        public Guid Id { get; set; }
        public string Username { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;
        public string? FullName { get; set; }
        public Guid? RoleId { get; set; }
        public string? RoleName { get; set; }
        public DateTime? CreatedAt { get; set; }
    }
}