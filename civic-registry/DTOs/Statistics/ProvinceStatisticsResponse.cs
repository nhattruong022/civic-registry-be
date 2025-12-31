namespace CivicRegistry.API.DTOs
{
    /// <summary>
    /// DTO cho thống kê tỉnh
    /// </summary>
    public class ProvinceStatisticsResponse
    {
        /// <summary>
        /// Tổng số hộ khẩu
        /// </summary>
        public int TotalHouseholds { get; set; }

        /// <summary>
        /// Tổng số nhân khẩu
        /// </summary>
        public int TotalCitizens { get; set; }

        /// <summary>
        /// Số yêu cầu đang chờ xử lý
        /// </summary>
        public int PendingRequests { get; set; }

        /// <summary>
        /// Số yêu cầu đã duyệt
        /// </summary>
        public int ApprovedRequests { get; set; }

        /// <summary>
        /// Số yêu cầu bị từ chối
        /// </summary>
        public int RejectedRequests { get; set; }

        /// <summary>
        /// Số biến động dân cư trong tháng
        /// </summary>
        public int MonthlyPopulationChanges { get; set; }
    }
}

