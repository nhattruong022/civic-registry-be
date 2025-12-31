using System.ComponentModel.DataAnnotations;

namespace CivicRegistry.API.DTOs
{
    /// <summary>
    /// DTO cho yêu cầu cập nhật hộ khẩu
    /// </summary>
    public class UpdateHouseholdRequest
    {
        /// <summary>
        /// Mã hộ khẩu
        /// </summary>
        [StringLength(50, ErrorMessage = "Mã hộ khẩu không được vượt quá 50 ký tự")]
        public string? HouseholdCode { get; set; }

        /// <summary>
        /// Địa chỉ
        /// </summary>
        [StringLength(500, ErrorMessage = "Địa chỉ không được vượt quá 500 ký tự")]
        public string? Address { get; set; }

        /// <summary>
        /// ID thôn
        /// </summary>
        public string? HamletId { get; set; }

        /// <summary>
        /// ID chủ hộ (nhân khẩu)
        /// </summary>
        public string? HeadCitizenId { get; set; }

        /// <summary>
        /// Trạng thái: 0=Active, 1=Transferred, 2=Closed
        /// </summary>
        public int? Status { get; set; }
    }
}

