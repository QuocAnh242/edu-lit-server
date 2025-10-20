using AuthService.Application.Enums;
using AuthService.Domain.Entities;
using Microsoft.Extensions.Logging;

namespace AuthService.Infrastructure.Data
{
    public static class DBInitializer
    {
        public static void Seed(AuthDbContext context)
        {
            try
            {
                if (!context.UserRoles.Any())
                {
                    var roles = new[]
                    {
                        new UserRole { Id = Guid.NewGuid(), Name = RoleType.STUDENT.ToString() },
                        new UserRole { Id = Guid.NewGuid(), Name = RoleType.TEACHER.ToString() },
                        new UserRole { Id = Guid.NewGuid(), Name = RoleType.ADMIN.ToString() }
                    };

                    context.UserRoles.AddRange(roles);
                    context.SaveChanges();
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"[DBInitializer] Seeding failed: {ex.Message}");
            }
        }
    }
}
