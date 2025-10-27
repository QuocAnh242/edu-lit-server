using AuthService.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Npgsql;

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

                    try
                    {
                        await dbContext.Database.MigrateAsync();
                    }
                    catch (PostgresException pgEx) when (pgEx.SqlState == "42P07")
                    {
                        // Table already exists; skip applying migrations to avoid fatal startup loop.
                        logger.LogWarning("Skipping EF migrations because schema objects already exist (42P07). Consider baselining EF migrations to the current schema.");
                    }

                    // Ensure outbox schema exists even when migrations are skipped
                    await dbContext.Database.EnsureOutboxSchemaAsync(logger);

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

        // Ensure the uuid extension and outbox table exist
        private static async Task EnsureOutboxSchemaAsync(this DatabaseFacade database, ILogger logger)
        {
            // uuid-ossp needed for default uuid_generate_v4()
            await database.ExecuteNonQueryAsync(@"CREATE EXTENSION IF NOT EXISTS ""uuid-ossp"";");

            var exists = await database.ExecuteScalarAsync<int>(
                @"SELECT CASE WHEN EXISTS (
                      SELECT 1 FROM information_schema.tables
                      WHERE table_schema = 'public' AND table_name = 'outbox_messages'
                  ) THEN 1 ELSE 0 END");

            if (exists == 0)
            {
                logger.LogInformation("Creating table outbox_messages (not found).");
                await database.ExecuteNonQueryAsync(
                    @"CREATE TABLE IF NOT EXISTS public.outbox_messages (
                        id uuid PRIMARY KEY DEFAULT uuid_generate_v4(),
                        type varchar(255) NOT NULL,
                        payload text NOT NULL,
                        occurred_on_utc timestamptz NOT NULL DEFAULT (now() at time zone 'utc'),
                        processed_on_utc timestamptz NULL,
                        error text NULL
                      );");
            }
        }

        // Helpers
        private static async Task<int> ExecuteScalarAsync<T>(this DatabaseFacade database, string sql)
        {
            using var command = database.GetDbConnection().CreateCommand();
            command.CommandText = sql;

            if (command.Connection.State != System.Data.ConnectionState.Open)
                await command.Connection.OpenAsync();

            var result = await command.ExecuteScalarAsync();
            return Convert.ToInt32(result);
        }

        private static async Task ExecuteNonQueryAsync(this DatabaseFacade database, string sql)
        {
            using var command = database.GetDbConnection().CreateCommand();
            command.CommandText = sql;

            if (command.Connection.State != System.Data.ConnectionState.Open)
                await command.Connection.OpenAsync();

            await command.ExecuteNonQueryAsync();
        }
    }
}
