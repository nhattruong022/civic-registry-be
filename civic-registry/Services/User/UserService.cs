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
        /// Lấy danh sách users (có thể lọc theo role)
        /// </summary>
        /// <param name="role">Role để lọc (optional). Nếu null thì lấy tất cả</param>
        public async Task<List<UserInfo>> GetAllUsersAsync(string? role = null)
        {
            FilterDefinition<User> filter;
            
            if (!string.IsNullOrEmpty(role))
            {
                filter = Builders<User>.Filter.Eq(u => u.Role, role);
            }
            else
            {
                filter = Builders<User>.Filter.Empty;
            }

            var users = await _context.Users
                .Find(filter)
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
        /// Lấy danh sách users có phân trang (có thể lọc theo role)
        /// </summary>
        /// <param name="page">Số trang (mặc định: 1)</param>
        /// <param name="pageSize">Số item mỗi trang (mặc định: 20, tối đa: 100)</param>
        /// <param name="role">Role để lọc (optional). Nếu null thì lấy tất cả</param>
        public async Task<PaginatedResponse<UserInfo>> GetUsersPaginatedAsync(int page = 1, int pageSize = 20, string? role = null)
        {
            FilterDefinition<User> filter;
            
            if (!string.IsNullOrEmpty(role))
            {
                filter = Builders<User>.Filter.Eq(u => u.Role, role);
            }
            else
            {
                filter = Builders<User>.Filter.Empty;
            }

            // Đảm bảo page và pageSize hợp lệ
            if (page < 1) page = 1;
            if (pageSize < 1) pageSize = 20;
            if (pageSize > 100) pageSize = 100; // Giới hạn tối đa 100 items mỗi trang

            // Đếm tổng số items
            var totalItems = await _context.Users.CountDocumentsAsync(filter);

            // Tính toán phân trang
            var totalPages = (int)Math.Ceiling((double)totalItems / pageSize);
            var skip = (page - 1) * pageSize;

            // Lấy dữ liệu với phân trang
            var users = await _context.Users
                .Find(filter)
                .SortByDescending(u => u.CreatedAt)
                .Skip(skip)
                .Limit(pageSize)
                .ToListAsync();

            var items = users.Select(u => new UserInfo
            {
                Id = u.Id,
                Username = u.Username,
                FullName = u.FullName,
                Role = u.Role,
                IsActive = u.IsActive
            }).ToList();

            return new PaginatedResponse<UserInfo>
            {
                Items = items,
                Pagination = new PaginationInfo
                {
                    Page = page,
                    PageSize = pageSize,
                    TotalItems = totalItems,
                    TotalPages = totalPages,
                    HasNextPage = page < totalPages,
                    HasPrevPage = page > 1
                }
            };
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

