using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using Microsoft.IdentityModel.Tokens;
using CivicRegistry.API.Models;
using MongoDB.Driver;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection;

namespace CivicRegistry.API.Middleware
{
    /// <summary>
    /// Middleware để xử lý JWT Authentication
    /// </summary>
    public class JwtAuthenticationMiddleware
    {
        private readonly RequestDelegate _next;
        private readonly IConfiguration _configuration;
        private readonly IServiceScopeFactory _serviceScopeFactory;

        public JwtAuthenticationMiddleware(RequestDelegate next, IConfiguration configuration, IServiceScopeFactory serviceScopeFactory)
        {
            _next = next;
            _configuration = configuration;
            _serviceScopeFactory = serviceScopeFactory;
        }

        public async Task InvokeAsync(HttpContext context)
        {
            // Bỏ qua authentication cho các endpoint công khai
            var path = context.Request.Path.Value?.ToLower();
            var isPublicEndpoint = path != null && (
                path.StartsWith("/api/auth/") ||
                path.StartsWith("/swagger") ||
                path.StartsWith("/api-docs")
            );

            if (isPublicEndpoint)
            {
                await _next(context);
                return;
            }

            // Lấy token từ Authorization header
            var token = ExtractTokenFromHeader(context);

            if (!string.IsNullOrEmpty(token))
            {
                try
                {
                    // Validate và decode token
                    var principal = ValidateToken(token);
                    if (principal != null)
                    {
                        // Lấy thông tin user từ token
                        var userId = principal.FindFirst(ClaimTypes.NameIdentifier)?.Value;
                        if (!string.IsNullOrEmpty(userId))
                        {
                            // Tạo scope để resolve MongoDbContext (scoped service)
                            using (var scope = _serviceScopeFactory.CreateScope())
                            {
                                var dbContext = scope.ServiceProvider.GetRequiredService<MongoDbContext>();
                                
                                // Tìm user trong database
                                var user = await dbContext.Users
                                    .Find(u => u.Id == userId && u.IsActive == true)
                                    .FirstOrDefaultAsync();

                                if (user != null)
                                {
                                    // Lưu user vào HttpContext để các controller có thể sử dụng
                                    context.Items["User"] = user;
                                    context.Items["UserId"] = user.Id;
                                    context.Items["UserRole"] = user.Role;
                                }
                            }
                        }

                        // Lưu claims vào HttpContext
                        context.User = principal;
                    }
                }
                catch (Exception)
                {
                    // Token không hợp lệ, nhưng không throw exception
                    // Để các controller có thể xử lý riêng
                }
            }

            await _next(context);
        }

        /// <summary>
        /// Lấy token từ Authorization header
        /// </summary>
        private string? ExtractTokenFromHeader(HttpContext context)
        {
            var authHeader = context.Request.Headers["Authorization"].FirstOrDefault();
            if (string.IsNullOrEmpty(authHeader))
            {
                return null;
            }

            // Hỗ trợ cả "Bearer {token}" và chỉ "{token}"
            if (authHeader.StartsWith("Bearer ", StringComparison.OrdinalIgnoreCase))
            {
                return authHeader.Substring("Bearer ".Length).Trim();
            }

            return authHeader.Trim();
        }

        /// <summary>
        /// Validate JWT token
        /// </summary>
        private ClaimsPrincipal? ValidateToken(string token)
        {
            try
            {
                var jwtSettings = _configuration.GetSection("JwtSettings");
                var secretKey = jwtSettings["SecretKey"] ?? "YourSuperSecretKeyForJWTTokenGeneration123456789";
                var issuer = jwtSettings["Issuer"] ?? "CivicRegistryAPI";
                var audience = jwtSettings["Audience"] ?? "CivicRegistryClient";

                var tokenHandler = new JwtSecurityTokenHandler();
                var key = Encoding.UTF8.GetBytes(secretKey);

                var validationParameters = new TokenValidationParameters
                {
                    ValidateIssuerSigningKey = true,
                    IssuerSigningKey = new SymmetricSecurityKey(key),
                    ValidateIssuer = true,
                    ValidIssuer = issuer,
                    ValidateAudience = true,
                    ValidAudience = audience,
                    ValidateLifetime = true,
                    ClockSkew = TimeSpan.Zero
                };

                var principal = tokenHandler.ValidateToken(token, validationParameters, out SecurityToken validatedToken);
                return principal;
            }
            catch
            {
                return null;
            }
        }
    }

    /// <summary>
    /// Extension method để đăng ký middleware
    /// </summary>
    public static class JwtAuthenticationMiddlewareExtensions
    {
        public static IApplicationBuilder UseJwtAuthentication(this IApplicationBuilder builder)
        {
            return builder.UseMiddleware<JwtAuthenticationMiddleware>();
        }
    }
}

