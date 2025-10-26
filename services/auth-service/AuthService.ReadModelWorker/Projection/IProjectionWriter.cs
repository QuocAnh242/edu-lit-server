using System;
using System.Threading;
using System.Threading.Tasks;

namespace AuthService.ReadModelWorker.Projection
{
    public interface IProjectionWriter
    {
        Task UpsertUserAsync(Guid id, string username, string email, string? fullName, Guid? roleId, DateTime? createdAtUtc, CancellationToken ct);
        Task DeleteUserAsync(Guid id, CancellationToken ct);
    }
}