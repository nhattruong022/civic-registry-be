namespace CivicRegistry.API.DTOs
{
    /// <summary>
    /// DTO cho phản hồi duyệt yêu cầu
    /// </summary>
    public class ApproveRequestResponse
    {
        /// <summary>
        /// Thông báo
        /// </summary>
        public string Message { get; set; } = "Yêu cầu đã được duyệt";

        /// <summary>
        /// Thông tin yêu cầu đã được duyệt
        /// </summary>
        public RequestInfo? Request { get; set; }
    }
}

