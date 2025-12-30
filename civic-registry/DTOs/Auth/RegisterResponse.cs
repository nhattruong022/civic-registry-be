namespace CivicRegistry.API.DTOs
{
    /// <summary>
    /// DTO cho phản hồi đăng ký
    /// </summary>
    public class RegisterResponse
    {
        /// <summary>
        /// Thông báo kết quả
        /// </summary>
        public string Message { get; set; } = string.Empty;

        /// <summary>
        /// Thông tin user đã tạo
        /// </summary>
        public UserInfo? User { get; set; }
    }
}

