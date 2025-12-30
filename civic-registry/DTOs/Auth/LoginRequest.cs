using System.ComponentModel.DataAnnotations;

namespace CivicRegistry.API.DTOs
{
    /// <summary>
    /// DTO cho yêu cầu đăng nhập
    /// </summary>
    public class LoginRequest
    {
        /// <summary>
        /// Tên đăng nhập
        /// </summary>
        [Required(ErrorMessage = "Username là bắt buộc")]
        public string Username { get; set; } = string.Empty;

        /// <summary>
        /// Mật khẩu
        /// </summary>
        [Required(ErrorMessage = "Password là bắt buộc")]
        public string Password { get; set; } = string.Empty;
    }
}

