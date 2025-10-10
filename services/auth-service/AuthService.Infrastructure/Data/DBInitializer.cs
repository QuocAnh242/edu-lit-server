using AuthService.Application.Enums;
using AuthService.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AuthService.Infrastructure.Data
{
    public static class DBInitializer
    {
        public static void Seed(AuthDbContext context)
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
    }
}
