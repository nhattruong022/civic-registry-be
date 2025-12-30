namespace CivicRegistry.API.Types
{
    /// <summary>
    /// Base response cho tất cả các API
    /// </summary>
    public class ApiAbstractResponse
    {
        /// <summary>
        /// Thông báo
        /// </summary>
        public string? Message { get; set; }

        /// <summary>
        /// Trạng thái thành công
        /// </summary>
        public bool Success { get; set; }

        /// <summary>
        /// Mã trả về
        /// </summary>
        public int ReturnCode { get; set; }
    }

    /// <summary>
    /// Response thành công với dữ liệu
    /// </summary>
    /// <typeparam name="T">Kiểu dữ liệu trả về</typeparam>
    public class ApiSuccessResponse<T> : ApiAbstractResponse
    {
        /// <summary>
        /// Dữ liệu kết quả
        /// </summary>
        public T? Result { get; set; }

        public ApiSuccessResponse()
        {
            Success = true;
            ReturnCode = 200;
        }

        public ApiSuccessResponse(T? result, string? message = null, int returnCode = 200)
        {
            Result = result;
            Message = message;
            Success = true;
            ReturnCode = returnCode;
        }
    }

    /// <summary>
    /// Response lỗi
    /// </summary>
    public class ApiErrorResponse : ApiAbstractResponse
    {
        /// <summary>
        /// Chi tiết lỗi
        /// </summary>
        public string? Detail { get; set; }

        public ApiErrorResponse()
        {
            Success = false;
            ReturnCode = 500;
        }

        public ApiErrorResponse(string? message, string? detail = null, int returnCode = 500)
        {
            Message = message;
            Detail = detail;
            Success = false;
            ReturnCode = returnCode;
        }
    }
}

