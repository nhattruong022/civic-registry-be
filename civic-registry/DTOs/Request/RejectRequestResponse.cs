namespace CivicRegistry.API.DTOs
{
    /// <summary>
    /// DTO cho phản hồi từ chối yêu cầu
    /// </summary>
    public class RejectRequestResponse
    {
        /// <summary>
        /// Thông báo
        /// </summary>
        public string Message { get; set; } = "Yêu cầu đã bị từ chối";

        /// <summary>
        /// Thông tin yêu cầu đã bị từ chối
        /// </summary>
        public RequestInfo? Request { get; set; }
    }
}

