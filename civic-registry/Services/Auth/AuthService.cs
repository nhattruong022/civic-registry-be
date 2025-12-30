using CivicRegistry.API.Models;
using CivicRegistry.API.DTOs;
using MongoDB.Driver;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using Microsoft.IdentityModel.Tokens;

namespace CivicRegistry.API.Services
{
    /// <summary>
    /// Service xử lý authentication
    /// </summary>
    public class AuthService
    {
        private readonly MongoDbContext _context;
        private readonly IConfiguration _configuration;

        public AuthService(MongoDbContext context, IConfiguration configuration)
        {
            _context = context;
            _configuration = configuration;
        }

        /// <summary>
        /// Đăng nhập
        /// </summary>
        public async Task<LoginResponse?> LoginAsync(LoginRequest request)
        {
            // Tìm user theo username
            var user = await _context.Users
                .Find(u => u.Username == request.Username && u.IsActive == true)
                .FirstOrDefaultAsync();

            if (user == null)
            {
                return null; // User không tồn tại hoặc không active
            }

            // Verify password
            if (!BCrypt.Net.BCrypt.Verify(request.Password, user.PasswordHash))
            {
                return null; // Password sai
            }

            // Tạo JWT token
            var token = GenerateJwtToken(user);

            // Tạo response
            return new LoginResponse
            {
                Token = token,
                User = new UserInfo
                {
                    Id = user.Id,
                    Username = user.Username,
                    FullName = null,
                    Role = user.Role,
                    IsActive = user.IsActive
                },
                ExpiresAt = DateTimeOffset.UtcNow.AddHours(24).ToUnixTimeSeconds()
            };
        }

        /// <summary>
        /// Đăng ký user mới
        /// </summary>
        /// <returns>RegisterResponse nếu thành công, null nếu có lỗi</returns>
        public async Task<RegisterResponse?> RegisterAsync(RegisterRequest request)
        {
            // Validate role
            var validRoles = new[] { "SuperAdmin", "ProvinceAdmin", "DistrictAdmin", "WardAdmin", "Citizen" };
            if (!validRoles.Contains(request.Role))
            {
                return null; // Role không hợp lệ
            }

            // Kiểm tra username đã tồn tại chưa
            var existingUser = await _context.Users
                .Find(u => u.Username == request.Username)
                .FirstOrDefaultAsync();

            if (existingUser != null)
            {
                return null; // Username đã tồn tại
            }

            // Tạo user mới
            var newUser = new User
            {
                Username = request.Username,
                PasswordHash = HashPassword(request.Password),
                Role = request.Role,
                IsActive = true
            };

            // Lưu vào database
            await _context.Users.InsertOneAsync(newUser);

            // Tạo response
            return new RegisterResponse
            {
                Message = "Đăng ký thành công",
                User = new UserInfo
                {
                    Id = newUser.Id,
                    Username = newUser.Username,
                    FullName = null,
                    Role = newUser.Role,
                    IsActive = newUser.IsActive
                }
            };
        }

        /// <summary>
        /// Refresh JWT token
        /// </summary>
        public async Task<LoginResponse?> RefreshTokenAsync(RefreshTokenRequest request)
        {
            try
            {
                // Validate và decode token
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
                    ValidateLifetime = false, // Cho phép token đã hết hạn để refresh
                    ClockSkew = TimeSpan.Zero
                };

                // Validate token
                var principal = tokenHandler.ValidateToken(request.Token, validationParameters, out SecurityToken validatedToken);

                // Lấy thông tin user từ claims
                var userId = principal.FindFirst(ClaimTypes.NameIdentifier)?.Value;
                if (string.IsNullOrEmpty(userId))
                {
                    return null; // Không tìm thấy user ID trong token
                }

                // Tìm user trong database
                var user = await _context.Users
                    .Find(u => u.Id == userId && u.IsActive == true)
                    .FirstOrDefaultAsync();

                if (user == null)
                {
                    return null; // User không tồn tại hoặc không active
                }

                // Tạo token mới
                var newToken = GenerateJwtToken(user);

                // Tạo response
                return new LoginResponse
                {
                    Token = newToken,
                    User = new UserInfo
                    {
                        Id = user.Id,
                        Username = user.Username,
                        FullName = null,
                        Role = user.Role,
                        IsActive = user.IsActive
                    },
                    ExpiresAt = DateTimeOffset.UtcNow.AddHours(24).ToUnixTimeSeconds()
                };
            }
            catch (SecurityTokenException)
            {
                // Token không hợp lệ
                return null;
            }
            catch (Exception)
            {
                // Lỗi khác
                return null;
            }
        }

        /// <summary>
        /// Kiểm tra token và lấy thông tin user
        /// </summary>
        /// <param name="token">JWT token cần kiểm tra</param>
        /// <returns>UserInfo nếu token hợp lệ, null nếu không hợp lệ</returns>
        public async Task<UserInfo?> CheckTokenAsync(string token)
        {
            try
            {
                // Validate và decode token
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
                    ValidateLifetime = true, // Kiểm tra token còn hạn
                    ClockSkew = TimeSpan.Zero
                };

                // Validate token
                var principal = tokenHandler.ValidateToken(token, validationParameters, out SecurityToken validatedToken);

                // Lấy thông tin user từ claims
                var userId = principal.FindFirst(ClaimTypes.NameIdentifier)?.Value;
                if (string.IsNullOrEmpty(userId))
                {
                    return null; // Không tìm thấy user ID trong token
                }

                // Tìm user trong database
                var user = await _context.Users
                    .Find(u => u.Id == userId && u.IsActive == true)
                    .FirstOrDefaultAsync();

                if (user == null)
                {
                    return null; // User không tồn tại hoặc không active
                }

                // Trả về thông tin user
                return new UserInfo
                {
                    Id = user.Id,
                    Username = user.Username,
                    FullName = user.FullName,
                    Role = user.Role,
                    IsActive = user.IsActive
                };
            }
            catch (SecurityTokenException)
            {
                // Token không hợp lệ hoặc đã hết hạn
                return null;
            }
            catch (Exception)
            {
                // Lỗi khác
                return null;
            }
        }

        /// <summary>
        /// Đăng xuất
        /// </summary>
        /// <param name="token">JWT token cần logout</param>
        /// <returns>LogoutResponse nếu thành công</returns>
        public Task<LogoutResponse> LogoutAsync(string? token)
        {
            // Với JWT stateless, logout chủ yếu là xóa token ở client
            // Có thể thêm logic blacklist token ở đây nếu cần
            // Hiện tại chỉ trả về success message
            
            return Task.FromResult(new LogoutResponse
            {
                Message = "Đăng xuất thành công"
            });
        }

        /// <summary>
        /// Kiểm tra username đã tồn tại chưa
        /// </summary>
        public async Task<bool> CheckUsernameExistsAsync(string username)
        {
            var user = await _context.Users
                .Find(u => u.Username == username)
                .FirstOrDefaultAsync();
            return user != null;
        }

        /// <summary>
        /// Hash password
        /// </summary>
        public string HashPassword(string password)
        {
            return BCrypt.Net.BCrypt.HashPassword(password);
        }

        /// <summary>
        /// Tạo JWT token
        /// </summary>
        private string GenerateJwtToken(User user)
        {
            var jwtSettings = _configuration.GetSection("JwtSettings");
            var secretKey = jwtSettings["SecretKey"] ?? "YourSuperSecretKeyForJWTTokenGeneration123456789";
            var issuer = jwtSettings["Issuer"] ?? "CivicRegistryAPI";
            var audience = jwtSettings["Audience"] ?? "CivicRegistryClient";
            var expiryMinutes = int.Parse(jwtSettings["ExpiryMinutes"] ?? "1440"); // 24 hours

            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(secretKey));
            var credentials = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

            var claims = new[]
            {
                new Claim(ClaimTypes.NameIdentifier, user.Id),
                new Claim(ClaimTypes.Name, user.Username),
                new Claim(ClaimTypes.Role, user.Role)
            };

            var token = new JwtSecurityToken(
                issuer: issuer,
                audience: audience,
                claims: claims,
                expires: DateTime.UtcNow.AddMinutes(expiryMinutes),
                signingCredentials: credentials
            );

            return new JwtSecurityTokenHandler().WriteToken(token);
        }
    }
}

