using AuthService.Application.Enums;
using AuthService.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace AuthService.Infrastructure.Data
{
    public static class DBInitializer
    {
        public static void Seed(AuthDbContext context)
        {
            try
            {
                var requiredRoleNames = new[]
                {
                    RoleType.STUDENT.ToString(),
                    RoleType.TEACHER.ToString(),
                    RoleType.ADMIN.ToString()
                };

                // Fetch existing role names
                var existingNames = context.UserRoles
                    .AsNoTracking()
                    .Select(r => r.Name)
                    .ToHashSet(StringComparer.OrdinalIgnoreCase);

                // Build only missing roles
                var missingRoles = requiredRoleNames
                    .Where(name => !existingNames.Contains(name))
                    .Select(name => new UserRole
                    {
                        Id = Guid.NewGuid(),
                        Name = name
                    })
                    .ToList();

                if (missingRoles.Count > 0)
                {
                    context.UserRoles.AddRange(missingRoles);
                    context.SaveChanges();
                }
            }
            catch (DbUpdateException ex)
            {
                // In case of concurrent startup or unique index race, ignore duplicates
                Console.WriteLine($"[DBInitializer] Role seeding skipped due to constraint: {ex.Message}");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"[DBInitializer] Seeding failed: {ex.Message}");
            }
        }
    }
}