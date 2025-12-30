using System.ComponentModel.DataAnnotations;

namespace CivicRegistry.API.DTOs
{
    /// <summary>
    /// DTO cho yêu cầu đăng ký
    /// </summary>
    public class RegisterRequest
    {
        /// <summary>
        /// Tên đăng nhập
        /// </summary>
        [Required(ErrorMessage = "Username là bắt buộc")]
        [StringLength(50, MinimumLength = 3, ErrorMessage = "Username phải có độ dài từ 3 đến 50 ký tự")]
        public string Username { get; set; } = string.Empty;

        /// <summary>
        /// Mật khẩu
        /// </summary>
        [Required(ErrorMessage = "Password là bắt buộc")]
        [StringLength(100, MinimumLength = 6, ErrorMessage = "Password phải có độ dài từ 6 đến 100 ký tự")]
        public string Password { get; set; } = string.Empty;

        /// <summary>
        /// Vai trò: SuperAdmin / ProvinceAdmin / DistrictAdmin / WardAdmin / Citizen
        /// </summary>
        [Required(ErrorMessage = "Role là bắt buộc")]
        public string Role { get; set; } = string.Empty;
    }
}

