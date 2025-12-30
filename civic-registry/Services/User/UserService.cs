using CivicRegistry.API.Models;
using CivicRegistry.API.DTOs;
using MongoDB.Driver;

namespace CivicRegistry.API.Services
{
    /// <summary>
    /// Service xử lý quản lý users
    /// </summary>
    public class UserService
    {
        private readonly MongoDbContext _context;
        private readonly AuthService _authService;

        public UserService(MongoDbContext context, AuthService authService)
        {
            _context = context;
            _authService = authService;
        }

        /// <summary>
        /// Lấy danh sách tất cả users
        /// </summary>
        public async Task<List<UserInfo>> GetAllUsersAsync()
        {
            var users = await _context.Users
                .Find(_ => true)
                .ToListAsync();

            return users.Select(u => new UserInfo
            {
                Id = u.Id,
                Username = u.Username,
                FullName = u.FullName,
                Role = u.Role,
                IsActive = u.IsActive
            }).ToList();
        }

        /// <summary>
        /// Lấy user theo ID
        /// </summary>
        public async Task<UserInfo?> GetUserByIdAsync(string id)
        {
            var user = await _context.Users
                .Find(u => u.Id == id)
                .FirstOrDefaultAsync();

            if (user == null)
            {
                return null;
            }

            return new UserInfo
            {
                Id = user.Id,
                Username = user.Username,
                FullName = user.FullName,
                Role = user.Role,
                IsActive = user.IsActive
            };
        }

        /// <summary>
        /// Tạo user mới
        /// </summary>
        public async Task<UserInfo?> CreateUserAsync(CreateUserRequest request)
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
                PasswordHash = _authService.HashPassword(request.Password),
                Role = request.Role,
                ProvinceId = request.ProvinceId,
                DistrictId = request.DistrictId,
                WardId = request.WardId,
                IsActive = request.IsActive,
                CreatedAt = DateTime.UtcNow
            };

            // Lưu vào database
            await _context.Users.InsertOneAsync(newUser);

            return new UserInfo
            {
                Id = newUser.Id,
                Username = newUser.Username,
                FullName = newUser.FullName,
                Role = newUser.Role,
                IsActive = newUser.IsActive
            };
        }

        /// <summary>
        /// Cập nhật user
        /// </summary>
        public async Task<UserInfo?> UpdateUserAsync(string id, UpdateUserRequest request)
        {
            // Validate role
            var validRoles = new[] { "SuperAdmin", "ProvinceAdmin", "DistrictAdmin", "WardAdmin", "Citizen" };
            if (!validRoles.Contains(request.Role))
            {
                return null; // Role không hợp lệ
            }

            // Tìm user
            var user = await _context.Users
                .Find(u => u.Id == id)
                .FirstOrDefaultAsync();

            if (user == null)
            {
                return null; // User không tồn tại
            }

            // Cập nhật thông tin
            user.Role = request.Role;
            user.ProvinceId = request.ProvinceId;
            user.DistrictId = request.DistrictId;
            user.WardId = request.WardId;
            user.IsActive = request.IsActive;

            // Lưu vào database
            await _context.Users.ReplaceOneAsync(u => u.Id == id, user);

            return new UserInfo
            {
                Id = user.Id,
                Username = user.Username,
                FullName = user.FullName,
                Role = user.Role,
                IsActive = user.IsActive
            };
        }

        /// <summary>
        /// Xóa user (soft delete - set IsActive = false)
        /// </summary>
        public async Task<bool> DeleteUserAsync(string id)
        {
            var user = await _context.Users
                .Find(u => u.Id == id)
                .FirstOrDefaultAsync();

            if (user == null)
            {
                return false; // User không tồn tại
            }

            // Soft delete - set IsActive = false
            user.IsActive = false;
            await _context.Users.ReplaceOneAsync(u => u.Id == id, user);

            return true;
        }

        /// <summary>
        /// Reset password cho user
        /// </summary>
        public async Task<bool> ResetPasswordAsync(string id, ResetPasswordRequest request)
        {
            var user = await _context.Users
                .Find(u => u.Id == id)
                .FirstOrDefaultAsync();

            if (user == null)
            {
                return false; // User không tồn tại
            }

            // Cập nhật password
            user.PasswordHash = _authService.HashPassword(request.NewPassword);
            await _context.Users.ReplaceOneAsync(u => u.Id == id, user);

            return true;
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
    }
}

