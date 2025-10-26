using System;
using System.Threading.Tasks;
using AuthService.Domain.Entities;

namespace AuthService.Application.Services.Interfaces
{
    public interface IUserReadProjection
    {
        Task UpsertAsync(User user);
        Task DeleteAsync(Guid id);
    }
}