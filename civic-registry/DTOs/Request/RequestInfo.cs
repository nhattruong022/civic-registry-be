namespace CivicRegistry.API.DTOs
{
    /// <summary>
    /// DTO cho thông tin yêu cầu
    /// </summary>
    public class RequestInfo
    {
        /// <summary>
        /// ID của yêu cầu
        /// </summary>
        public string Id { get; set; } = string.Empty;

        /// <summary>
        /// ID công dân
        /// </summary>
        public string CitizenId { get; set; } = string.Empty;

        /// <summary>
        /// Loại yêu cầu (0-9)
        /// </summary>
        public int RequestType { get; set; }

        /// <summary>
        /// Nội dung yêu cầu
        /// </summary>
        public string? Content { get; set; }

        /// <summary>
        /// Trạng thái: 0 = Pending, 1 = Approved, 2 = Rejected
        /// </summary>
        public int Status { get; set; }

        /// <summary>
        /// Ngày tạo
        /// </summary>
        public DateTime CreatedAt { get; set; }

        /// <summary>
        /// Người xử lý
        /// </summary>
        public string? ProcessedBy { get; set; }

        /// <summary>
        /// Ngày xử lý
        /// </summary>
        public DateTime? ProcessedAt { get; set; }
    }
}

