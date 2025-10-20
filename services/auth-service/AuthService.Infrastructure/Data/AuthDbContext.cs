using AuthService.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;

namespace AuthService.Infrastructure.Data
{
    public partial class AuthDbContext : DbContext
    {
        private readonly IConfiguration _configuration;

        public AuthDbContext()
        {
        }

        public AuthDbContext(DbContextOptions<AuthDbContext> options, IConfiguration configuration)
            : base(options)
        {
            _configuration = configuration;
        }

        public virtual DbSet<Oauthaccount> Oauthaccounts { get; set; }
        public virtual DbSet<User> Users { get; set; }
        public virtual DbSet<UserRole> UserRoles { get; set; }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            if (!optionsBuilder.IsConfigured)
            {
                // Ưu tiên đọc từ environment (Docker), fallback sang appsettings.json khi local
                var config = _configuration ?? new ConfigurationBuilder()
                    .SetBasePath(AppDomain.CurrentDomain.BaseDirectory)
                    .AddJsonFile("appsettings.json", optional: true)
                    .AddEnvironmentVariables()
                    .Build();

                var connectionString = config.GetConnectionString("DefaultConnection");
                optionsBuilder.UseNpgsql(connectionString);
            }
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.HasPostgresExtension("uuid-ossp");

            modelBuilder.Entity<Oauthaccount>(entity =>
            {
                entity.HasKey(e => e.Id).HasName("oauthaccount_pkey");
                entity.ToTable("oauthaccount");

                entity.Property(e => e.Id)
                    .HasDefaultValueSql("uuid_generate_v4()")
                    .HasColumnName("id");
                entity.Property(e => e.Provider).HasColumnName("provider");
                entity.Property(e => e.ProviderAccountId).HasColumnName("provider_account_id");
                entity.Property(e => e.UserId).HasColumnName("user_id");

                entity.HasOne(d => d.User).WithMany(p => p.Oauthaccounts)
                    .HasForeignKey(d => d.UserId)
                    .OnDelete(DeleteBehavior.Cascade)
                    .HasConstraintName("oauthaccount_user_id_fkey");
            });

            modelBuilder.Entity<User>(entity =>
            {
                entity.HasKey(e => e.Id).HasName("users_pkey");
                entity.ToTable("users");

                entity.HasIndex(e => e.Email, "users_email_key").IsUnique();
                entity.HasIndex(e => e.Username, "users_username_key").IsUnique();

                entity.Property(e => e.Id)
                    .HasDefaultValueSql("uuid_generate_v4()")
                    .HasColumnName("id");
                entity.Property(e => e.CreatedAt)
                    .HasDefaultValueSql("now() at time zone 'utc'")
                    .HasColumnType("timestamp with time zone")
                    .HasColumnName("created_at");
                entity.Property(e => e.Email).HasColumnName("email");
                entity.Property(e => e.FullName).HasColumnName("full_name");
                entity.Property(e => e.Password).HasColumnName("password");
                entity.Property(e => e.RoleId).HasColumnName("role_id");
                entity.Property(e => e.Username).HasColumnName("username");

                entity.HasOne(d => d.Role).WithMany(p => p.Users)
                    .HasForeignKey(d => d.RoleId)
                    .HasConstraintName("users_role_id_fkey");
            });

            modelBuilder.Entity<UserRole>(entity =>
            {
                entity.HasKey(e => e.Id).HasName("user_roles_pkey");
                entity.ToTable("user_roles");

                entity.HasIndex(e => e.Name, "user_roles_name_key").IsUnique();

                entity.Property(e => e.Id)
                    .HasDefaultValueSql("uuid_generate_v4()")
                    .HasColumnName("id");
                entity.Property(e => e.Name).HasColumnName("name");
            });

            OnModelCreatingPartial(modelBuilder);
        }

        partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
    }
}
