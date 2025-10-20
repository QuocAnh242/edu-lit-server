using AuthService.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;
namespace AuthService.Api.Extensions
{
    public static class DatabaseInitializerExtensions
    {
        public static async Task InitializeDatabaseAsync(this IServiceProvider services)
        {
            using var scope = services.CreateScope();
            var scopedProvider = scope.ServiceProvider;
            var logger = scopedProvider.GetRequiredService<ILoggerFactory>()
                                       .CreateLogger("DatabaseInitializer");
            var dbContext = scopedProvider.GetRequiredService<AuthDbContext>();

            const int maxRetries = 10;
            const int delayMs = 3000;

            for (int attempt = 1; attempt <= maxRetries; attempt++)
            {
                try
                {
                    logger.LogInformation($"Ensuring database is created... (Attempt {attempt}/{maxRetries})");
                    await dbContext.Database.EnsureCreatedAsync();

                    logger.LogInformation("Seeding initial data...");
                    DBInitializer.Seed(dbContext);

                    logger.LogInformation("Database initialization complete.");
                    return; 
                }
                catch (Exception ex)
                {
                    if (attempt == maxRetries)
                    {
                        logger.LogError(ex, "Database initialization failed after {maxRetries} attempts. Service will continue running.", maxRetries);
                        return; // Give up, but don't crash
                    }

                    logger.LogWarning(ex, "Database connection failed (Attempt {attempt}/{maxRetries}). Retrying in {delayMs}ms...", attempt, maxRetries, delayMs);
                    await Task.Delay(delayMs);
                }
            }
        }
    }
}