using CivicRegistry.API.Models;

namespace CivicRegistry.API.Middleware
{
    /// <summary>
    /// Extension methods cho HttpContext để lấy thông tin user từ JWT Authentication Middleware
    /// </summary>
    public static class HttpContextExtensions
    {
        /// <summary>
        /// Lấy thông tin User từ HttpContext (được set bởi JwtAuthenticationMiddleware)
        /// </summary>
        public static User? GetCurrentUser(this HttpContext context)
        {
            if (context.Items.TryGetValue("User", out var userObj) && userObj is User user)
            {
                return user;
            }
            return null;
        }

        /// <summary>
        /// Lấy UserId từ HttpContext
        /// </summary>
        public static string? GetCurrentUserId(this HttpContext context)
        {
            if (context.Items.TryGetValue("UserId", out var userIdObj) && userIdObj is string userId)
            {
                return userId;
            }
            return null;
        }

        /// <summary>
        /// Lấy UserRole từ HttpContext
        /// </summary>
        public static string? GetCurrentUserRole(this HttpContext context)
        {
            if (context.Items.TryGetValue("UserRole", out var roleObj) && roleObj is string role)
            {
                return role;
            }
            return null;
        }

        /// <summary>
        /// Kiểm tra user hiện tại có role cụ thể không
        /// </summary>
        public static bool HasRole(this HttpContext context, string role)
        {
            var currentRole = context.GetCurrentUserRole();
            return !string.IsNullOrEmpty(currentRole) && 
                   currentRole.Equals(role, StringComparison.OrdinalIgnoreCase);
        }

        /// <summary>
        /// Kiểm tra user hiện tại có một trong các role được chỉ định không
        /// </summary>
        public static bool HasAnyRole(this HttpContext context, params string[] roles)
        {
            var currentRole = context.GetCurrentUserRole();
            if (string.IsNullOrEmpty(currentRole))
            {
                return false;
            }

            return roles.Any(role => currentRole.Equals(role, StringComparison.OrdinalIgnoreCase));
        }
    }
}

