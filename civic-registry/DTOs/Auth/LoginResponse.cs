namespace CivicRegistry.API.DTOs
{
    /// <summary>
    /// DTO cho phản hồi đăng nhập
    /// </summary>
    public class LoginResponse
    {
        /// <summary>
        /// Token JWT
        /// </summary>
        public string Token { get; set; } = string.Empty;

        /// <summary>
        /// Thông tin user
        /// </summary>
        public UserInfo? User { get; set; }

        /// <summary>
        /// Thời gian hết hạn token (Unix timestamp)
        /// </summary>
        public long ExpiresAt { get; set; }
    }

    /// <summary>
    /// Thông tin user trong response
    /// </summary>
    public class UserInfo
    {
        public string Id { get; set; } = string.Empty;
        public string Username { get; set; } = string.Empty;
        public string? FullName { get; set; }
        public string Role { get; set; } = string.Empty;
        public bool IsActive { get; set; }
    }
}

