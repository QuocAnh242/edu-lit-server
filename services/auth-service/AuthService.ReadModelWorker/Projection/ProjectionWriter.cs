using System;
using System.Threading;
using System.Threading.Tasks;
using Dapper;
using Npgsql;

namespace AuthService.ReadModelWorker.Projection
{
    public sealed class ProjectionWriter : IProjectionWriter
    {
        private readonly string _cs;
        public ProjectionWriter(string connectionString) => _cs = connectionString;

        public async Task UpsertUserAsync(Guid id, string username, string email, string? fullName, Guid? roleId, DateTime? createdAtUtc, CancellationToken ct)
        {
            await using var conn = new NpgsqlConnection(_cs);
            await conn.OpenAsync(ct);

            const string sql = @"
insert into users_read (id, username, email, full_name, role_id, created_at)
values (@Id, @Username, @Email, @FullName, @RoleId, coalesce(@CreatedAt, now()))
on conflict (id) do update
set username = excluded.username,
    email = excluded.email,
    full_name = excluded.full_name,
    role_id = excluded.role_id;";

            await conn.ExecuteAsync(new CommandDefinition(sql, new
            {
                Id = id,
                Username = username,
                Email = email,
                FullName = fullName,
                RoleId = roleId,
                CreatedAt = createdAtUtc
            }, cancellationToken: ct));
        }

        public async Task DeleteUserAsync(Guid id, CancellationToken ct)
        {
            await using var conn = new NpgsqlConnection(_cs);
            await conn.OpenAsync(ct);
            const string sql = "delete from users_read where id = @Id;";
            await conn.ExecuteAsync(new CommandDefinition(sql, new { Id = id }, cancellationToken: ct));
        }
    }
}