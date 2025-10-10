using AuthService.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AuthService.Application.Services.Interfaces
{
    public interface IRoleService
    {
        Task<UserRole?> GetDefaultRoleAsync();
        Task<UserRole?> GetByIdAsync(Guid roleId);
    }
}
