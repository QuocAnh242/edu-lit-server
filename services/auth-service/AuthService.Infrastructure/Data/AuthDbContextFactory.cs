using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;
using AuthService.Infrastructure.Data;

namespace AuthService.Infrastructure
{
    public class AuthDbContextFactory : IDesignTimeDbContextFactory<AuthDbContext>
    {
        public AuthDbContext CreateDbContext(string[] args)
        {
            var optionsBuilder = new DbContextOptionsBuilder<AuthDbContext>();

            // ⚙️ Connection string đúng với Docker Compose
            const string connectionString = "Host=postgres;Port=5432;Database=auth_db;Username=postgres;Password=postgres";

            optionsBuilder.UseNpgsql(connectionString);

            return new AuthDbContext(optionsBuilder.Options, null!);
        }
    }
}
