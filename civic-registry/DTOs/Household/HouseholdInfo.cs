namespace CivicRegistry.API.DTOs
{
    /// <summary>
    /// DTO cho thông tin hộ khẩu
    /// </summary>
    public class HouseholdInfo
    {
        /// <summary>
        /// ID của hộ khẩu
        /// </summary>
        public string Id { get; set; } = string.Empty;

        /// <summary>
        /// Mã hộ khẩu
        /// </summary>
        public string HouseholdCode { get; set; } = string.Empty;

        /// <summary>
        /// Địa chỉ
        /// </summary>
        public string Address { get; set; } = string.Empty;

        /// <summary>
        /// ID thôn
        /// </summary>
        public string? HamletId { get; set; }

        /// <summary>
        /// ID chủ hộ (nhân khẩu)
        /// </summary>
        public string? HeadCitizenId { get; set; }

        /// <summary>
        /// Ngày tạo
        /// </summary>
        public DateTime CreatedDate { get; set; }

        /// <summary>
        /// Trạng thái: 0=Active, 1=Transferred, 2=Closed
        /// </summary>
        public int Status { get; set; }
    }
}

