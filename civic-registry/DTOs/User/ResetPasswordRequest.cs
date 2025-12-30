using System.ComponentModel.DataAnnotations;

namespace CivicRegistry.API.DTOs
{
    /// <summary>
    /// DTO cho yêu cầu reset password
    /// </summary>
    public class ResetPasswordRequest
    {
        /// <summary>
        /// Mật khẩu mới
        /// </summary>
        [Required(ErrorMessage = "NewPassword là bắt buộc")]
        [StringLength(100, MinimumLength = 6, ErrorMessage = "Password phải có độ dài từ 6 đến 100 ký tự")]
        public string NewPassword { get; set; } = string.Empty;
    }
}

