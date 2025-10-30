using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace AuthService.Infrastructure.Data.Entities
{
    public sealed class UserRead
    {
        public Guid Id { get; set; }
        public string Username { get; set; } = null!;
        public string Email { get; set; } = null!;
        public string? FullName { get; set; }
        public Guid? RoleId { get; set; }
        public string? RoleName { get; set; }
        public DateTime? CreatedAt { get; set; }
    }

    internal sealed class UserReadConfiguration : IEntityTypeConfiguration<UserRead>
    {
        public void Configure(EntityTypeBuilder<UserRead> b)
        {
            b.ToTable("users_read");
            b.HasKey(x => x.Id);
            b.Property(x => x.Username).HasMaxLength(128).IsRequired();
            b.Property(x => x.Email).HasMaxLength(256).IsRequired();
            b.Property(x => x.FullName).HasMaxLength(256);
            b.Property(x => x.RoleName).HasMaxLength(128);
            b.HasIndex(x => x.Username).IsUnique();
            b.HasIndex(x => x.Email).IsUnique();
        }
    }
}