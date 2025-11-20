using System.Security.Claims;

namespace QuestionService.Api.Extensions
{
    public static class ClaimsPrincipalExtensions
    {
        /// <summary>
        /// Gets the user ID from JWT claims
        /// </summary>
        public static Guid? GetUserId(this ClaimsPrincipal user)
        {
            var userIdClaim = user.FindFirst(ClaimTypes.NameIdentifier) 
                           ?? user.FindFirst("sub");
            
            if (userIdClaim != null && Guid.TryParse(userIdClaim.Value, out var userId))
            {
                return userId;
            }
            
            return null;
        }

        /// <summary>
        /// Gets the username from JWT claims
        /// </summary>
        public static string? GetUsername(this ClaimsPrincipal user)
        {
            return user.FindFirst(ClaimTypes.Name)?.Value 
                ?? user.FindFirst("unique_name")?.Value;
        }

        /// <summary>
        /// Gets the user email from JWT claims
        /// </summary>
        public static string? GetEmail(this ClaimsPrincipal user)
        {
            return user.FindFirst(ClaimTypes.Email)?.Value
                ?? user.FindFirst("email")?.Value;
        }

        /// <summary>
        /// Gets the user role from JWT claims
        /// </summary>
        public static string? GetRole(this ClaimsPrincipal user)
        {
            return user.FindFirst(ClaimTypes.Role)?.Value
                ?? user.FindFirst("role")?.Value;
        }

        /// <summary>
        /// Checks if the user has a specific role
        /// </summary>
        public static bool HasRole(this ClaimsPrincipal user, string role)
        {
            var userRole = user.GetRole();
            return userRole != null && userRole.Equals(role, StringComparison.OrdinalIgnoreCase);
        }

        /// <summary>
        /// Checks if the user is a teacher
        /// </summary>
        public static bool IsTeacher(this ClaimsPrincipal user)
        {
            return user.HasRole("TEACHER");
        }

        /// <summary>
        /// Checks if the user is a student
        /// </summary>
        public static bool IsStudent(this ClaimsPrincipal user)
        {
            return user.HasRole("STUDENT");
        }

        /// <summary>
        /// Checks if the user is an admin
        /// </summary>
        public static bool IsAdmin(this ClaimsPrincipal user)
        {
            return user.HasRole("ADMIN");
        }

        /// <summary>
        /// Checks if the user can manage questions (TEACHER or ADMIN)
        /// </summary>
        public static bool CanManageQuestions(this ClaimsPrincipal user)
        {
            return user.IsTeacher() || user.IsAdmin();
        }
    }
}


