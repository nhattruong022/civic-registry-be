using System.ComponentModel.DataAnnotations;

namespace CivicRegistry.API.DTOs
{
    /// <summary>
    /// DTO cho yêu cầu cập nhật user
    /// </summary>
    public class UpdateUserRequest
    {
        /// <summary>
        /// Vai trò: SuperAdmin / ProvinceAdmin / DistrictAdmin / WardAdmin / Citizen
        /// </summary>
        [Required(ErrorMessage = "Role là bắt buộc")]
        public string Role { get; set; } = string.Empty;

        /// <summary>
        /// ID tỉnh
        /// </summary>
        public int? ProvinceId { get; set; }

        /// <summary>
        /// ID huyện
        /// </summary>
        public int? DistrictId { get; set; }

        /// <summary>
        /// ID xã
        /// </summary>
        public int? WardId { get; set; }

        /// <summary>
        /// Trạng thái hoạt động
        /// </summary>
        public bool IsActive { get; set; } = true;
    }
}

