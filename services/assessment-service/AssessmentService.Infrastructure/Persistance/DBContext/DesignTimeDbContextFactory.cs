using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;
using Microsoft.Extensions.Configuration;
using System.IO;

namespace AssessmentService.Infrastructure.Persistance.DBContext;

public class DesignTimeDbContextFactory : IDesignTimeDbContextFactory<AssessmentDbContext>
{
    public AssessmentDbContext CreateDbContext(string[] args)
    {
        // Build configuration from appsettings.json in API project
        var basePath = Path.Combine(Directory.GetCurrentDirectory(), "..", "AssessmentService.Api");
        var configuration = new ConfigurationBuilder()
            .SetBasePath(basePath)
            .AddJsonFile("appsettings.json", optional: false, reloadOnChange: true)
            .AddJsonFile("appsettings.Development.json", optional: true, reloadOnChange: true)
            .AddEnvironmentVariables()
            .Build();

        var connectionString = configuration.GetConnectionString("MySqlConnection");

        if (string.IsNullOrWhiteSpace(connectionString))
        {
            throw new InvalidOperationException("Connection string 'MySqlConnection' not found.");
        }

        var optionsBuilder = new DbContextOptionsBuilder<AssessmentDbContext>();
        optionsBuilder.UseMySql(connectionString, new MySqlServerVersion(new Version(5, 7, 43)),
            mySqlOptions => mySqlOptions.EnableRetryOnFailure(
                maxRetryCount: 5,
                maxRetryDelay: TimeSpan.FromSeconds(30),
                errorNumbersToAdd: null));

        return new AssessmentDbContext(optionsBuilder.Options, configuration);
    }
}

