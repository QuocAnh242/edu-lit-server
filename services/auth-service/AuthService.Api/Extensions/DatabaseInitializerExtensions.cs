using AuthService.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace AuthService.Api.Extensions
{
    public static class DatabaseInitializerExtensions
    {
        /// <summary>
        /// Tự động migrate và seed database khi khởi động service.
        /// </summary>
        public static async Task InitializeDatabaseAsync(this IServiceProvider services)
        {
            using var scope = services.CreateScope();
            var scopedProvider = scope.ServiceProvider;

            var logger = scopedProvider.GetRequiredService<ILoggerFactory>()
                                       .CreateLogger("DatabaseInitializer");
            var dbContext = scopedProvider.GetRequiredService<AuthDbContext>();

            try
            {
                logger.LogInformation("Running database migration...");
                await dbContext.Database.MigrateAsync(); // Apply migrations

                logger.LogInformation("Seeding initial data...");
                DBInitializer.Seed(dbContext); // Seed mặc định
                logger.LogInformation("Database initialization complete.");
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "Error while initializing database.");
                throw;
            }
        }
    }
}
