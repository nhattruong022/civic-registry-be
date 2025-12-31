namespace CivicRegistry.API.DTOs
{
    /// <summary>
    /// DTO cho thông tin phân trang
    /// </summary>
    public class PaginationInfo
    {
        /// <summary>
        /// Trang hiện tại (bắt đầu từ 1)
        /// </summary>
        public int Page { get; set; }

        /// <summary>
        /// Số item trên mỗi trang
        /// </summary>
        public int PageSize { get; set; }

        /// <summary>
        /// Tổng số items
        /// </summary>
        public long TotalItems { get; set; }

        /// <summary>
        /// Tổng số trang
        /// </summary>
        public int TotalPages { get; set; }

        /// <summary>
        /// Có trang tiếp theo không
        /// </summary>
        public bool HasNextPage { get; set; }

        /// <summary>
        /// Có trang trước không
        /// </summary>
        public bool HasPrevPage { get; set; }
    }

    /// <summary>
    /// Response có phân trang
    /// </summary>
    /// <typeparam name="T">Kiểu dữ liệu trong danh sách</typeparam>
    public class PaginatedResponse<T>
    {
        /// <summary>
        /// Danh sách dữ liệu
        /// </summary>
        public List<T> Items { get; set; } = new List<T>();

        /// <summary>
        /// Thông tin phân trang
        /// </summary>
        public PaginationInfo Pagination { get; set; } = new PaginationInfo();
    }
}

