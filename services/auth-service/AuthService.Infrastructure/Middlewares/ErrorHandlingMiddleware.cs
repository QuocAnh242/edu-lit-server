using AuthService.Application.DTOs.Response;
using AuthService.Application.Enums;
using AuthService.Application.Exceptions;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using Npgsql;
using System.Net;
using System.Text.Json;

namespace AuthService.Infrastructure.Middlewares
{
    public class ErrorHandlingMiddleware
    {
        private readonly RequestDelegate _next;

        public ErrorHandlingMiddleware(RequestDelegate next)
        {
            _next = next;
        }

        public async Task Invoke(HttpContext context)
        {
            try
            {
                await _next(context);
            }
            catch (Exception ex)
            {
                await WriteErrorAsync(context, ex);
            }
        }

        private static async Task WriteErrorAsync(HttpContext context, Exception ex)
        {
            context.Response.ContentType = "application/json";
            var (httpStatus, apiCode, message) = MapException(ex);
            context.Response.StatusCode = (int)httpStatus;
            var payload = ApiResponse<string>.Error(apiCode, message);
            var json = JsonSerializer.Serialize(payload);
            await context.Response.WriteAsync(json);
        }

        private static (HttpStatusCode, ApiStatusCode, string) MapException(Exception ex)
        {
            return ex switch
            {
                ArgumentNullException ane => (HttpStatusCode.BadRequest, ApiStatusCode.HB40001, ane.Message),
                ArgumentException ae => (HttpStatusCode.BadRequest, ApiStatusCode.HB40001, ae.Message),
                KeyNotFoundException kne => (HttpStatusCode.NotFound, ApiStatusCode.HB40401, kne.Message),
                UnauthorizedAccessException => (HttpStatusCode.Unauthorized, ApiStatusCode.HB40101, "Unauthorized"),
                AuthException authEx when authEx.ErrorCode == AuthErrorCode.UserAlreadyExists => 
                    (HttpStatusCode.Conflict, ApiStatusCode.HB40901, authEx.Message),
                AuthException authEx => 
                    (HttpStatusCode.BadRequest, ApiStatusCode.HB40001, authEx.Message),
                DbUpdateException dbEx when dbEx.InnerException is PostgresException pgEx && pgEx.SqlState == "23505" => 
                    MapDuplicateEntryError(dbEx.InnerException as PostgresException),
                DbUpdateException dbEx when dbEx.InnerException is PostgresException pgEx => 
                    (HttpStatusCode.InternalServerError, ApiStatusCode.HB50001, pgEx.Message),
                _ => (HttpStatusCode.InternalServerError, ApiStatusCode.HB50001, "Internal server error")
            };
        }

        private static (HttpStatusCode, ApiStatusCode, string) MapDuplicateEntryError(PostgresException? pgEx)
        {
            if (pgEx == null)
                return (HttpStatusCode.Conflict, ApiStatusCode.HB40901, "Duplicate entry detected");

            // Extract which field caused the duplicate
            var constraintName = pgEx.ConstraintName ?? "";
            var message = "Duplicate entry detected";
            
            if (constraintName.Contains("username", StringComparison.OrdinalIgnoreCase))
                message = "Username is already taken";
            else if (constraintName.Contains("email", StringComparison.OrdinalIgnoreCase))
                message = "Email is already registered";
            else if (pgEx.Message.Contains("username", StringComparison.OrdinalIgnoreCase))
                message = "Username is already taken";
            else if (pgEx.Message.Contains("email", StringComparison.OrdinalIgnoreCase))
                message = "Email is already registered";
            
            return (HttpStatusCode.Conflict, ApiStatusCode.HB40901, message);
        }
    }
}

