using System.ComponentModel.DataAnnotations;

namespace CivicRegistry.API.DTOs
{
    /// <summary>
    /// DTO cho yêu cầu tạo hộ khẩu mới
    /// </summary>
    /// <remarks>
    /// **Ví dụ request body:**
    /// ```json
    /// {
    ///   "householdCode": "HK001",
    ///   "address": "Số 123, Đường ABC, Thôn XYZ",
    ///   "hamletId": "507f1f77bcf86cd799439011",
    ///   "headCitizenId": "507f191e810c19729de860ea",
    ///   "status": 0
    /// }
    /// ```
    /// </remarks>
    public class CreateHouseholdRequest
    {
        /// <summary>
        /// Mã hộ khẩu (bắt buộc, unique)
        /// </summary>
        /// <example>HK001</example>
        [Required(ErrorMessage = "Mã hộ khẩu là bắt buộc")]
        [StringLength(50, ErrorMessage = "Mã hộ khẩu không được vượt quá 50 ký tự")]
        public string HouseholdCode { get; set; } = string.Empty;

        /// <summary>
        /// Địa chỉ (bắt buộc)
        /// </summary>
        /// <example>Số 123, Đường ABC, Thôn XYZ</example>
        [Required(ErrorMessage = "Địa chỉ là bắt buộc")]
        [StringLength(500, ErrorMessage = "Địa chỉ không được vượt quá 500 ký tự")]
        public string Address { get; set; } = string.Empty;

        /// <summary>
        /// ID thôn (bắt buộc)
        /// </summary>
        /// <example>507f1f77bcf86cd799439011</example>
        [Required(ErrorMessage = "ID thôn là bắt buộc")]
        public string HamletId { get; set; } = string.Empty;

        /// <summary>
        /// ID chủ hộ (nhân khẩu) - tùy chọn, có thể thêm sau
        /// </summary>
        /// <example>507f191e810c19729de860ea</example>
        public string? HeadCitizenId { get; set; }

        /// <summary>
        /// Trạng thái: 0=Active, 1=Transferred, 2=Closed (mặc định: 0)
        /// </summary>
        /// <example>0</example>
        public int Status { get; set; } = 0;
    }
}

