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
                    logger.LogInformation("Running database migrations... (Attempt {Attempt}/{MaxRetries})", attempt, maxRetries);
                    
                    // Use Migrate instead of EnsureCreated to apply migrations
                    await dbContext.Database.MigrateAsync();

                    logger.LogInformation("Seeding initial data...");
                    DBInitializer.Seed(dbContext);

                    logger.LogInformation("Database initialization complete.");
                    return; 
                }
                catch (Exception ex)
                {
                    if (attempt == maxRetries)
                    {
                        logger.LogError(ex, "Database initialization failed after {MaxRetries} attempts. Service will continue running.", maxRetries);
                        return;
                    }

                    logger.LogWarning(ex, "Database connection failed (Attempt {Attempt}/{MaxRetries}). Retrying in {DelayMs}ms...", attempt, maxRetries, delayMs);
                    await Task.Delay(delayMs);
                }
            }
        }
    }
}