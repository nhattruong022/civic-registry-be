using System.ComponentModel.DataAnnotations;

namespace CivicRegistry.API.DTOs
{
    /// <summary>
    /// DTO cho yêu cầu tạo user mới
    /// </summary>
    /// <remarks>
    /// **Ví dụ request body:**
    /// ```json
    /// {
    ///   "username": "provinceadmin1",
    ///   "password": "Password123",
    ///   "role": "ProvinceAdmin",
    ///   "provinceId": 1,
    ///   "districtId": null,
    ///   "wardId": null,
    ///   "isActive": true
    /// }
    /// ```
    /// 
    /// **Phân quyền tạo user:**
    /// - SuperAdmin: có thể tạo ProvinceAdmin, DistrictAdmin, WardAdmin
    /// - ProvinceAdmin: chỉ có thể tạo DistrictAdmin
    /// - DistrictAdmin: chỉ có thể tạo WardAdmin
    /// </remarks>
    public class CreateUserRequest
    {
        /// <summary>
        /// Tên đăng nhập (bắt buộc, độ dài 3-50 ký tự, unique)
        /// </summary>
        /// <example>provinceadmin1</example>
        [Required(ErrorMessage = "Username là bắt buộc")]
        [StringLength(50, MinimumLength = 3, ErrorMessage = "Username phải có độ dài từ 3 đến 50 ký tự")]
        public string Username { get; set; } = string.Empty;

        /// <summary>
        /// Mật khẩu (bắt buộc, độ dài 6-100 ký tự)
        /// </summary>
        /// <example>Password123</example>
        [Required(ErrorMessage = "Password là bắt buộc")]
        [StringLength(100, MinimumLength = 6, ErrorMessage = "Password phải có độ dài từ 6 đến 100 ký tự")]
        public string Password { get; set; } = string.Empty;

        /// <summary>
        /// Vai trò: SuperAdmin / ProvinceAdmin / DistrictAdmin / WardAdmin / Citizen (bắt buộc)
        /// </summary>
        /// <example>ProvinceAdmin</example>
        [Required(ErrorMessage = "Role là bắt buộc")]
        public string Role { get; set; } = string.Empty;

        /// <summary>
        /// ID tỉnh (tùy chọn, dùng cho ProvinceAdmin, DistrictAdmin, WardAdmin)
        /// </summary>
        /// <example>1</example>
        public int? ProvinceId { get; set; }

        /// <summary>
        /// ID huyện (tùy chọn, dùng cho DistrictAdmin, WardAdmin)
        /// </summary>
        /// <example>10</example>
        public int? DistrictId { get; set; }

        /// <summary>
        /// ID xã (tùy chọn, dùng cho WardAdmin)
        /// </summary>
        /// <example>100</example>
        public int? WardId { get; set; }

        /// <summary>
        /// Trạng thái hoạt động (mặc định: true)
        /// </summary>
        /// <example>true</example>
        public bool IsActive { get; set; } = true;
    }
}

