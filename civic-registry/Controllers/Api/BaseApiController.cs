using Microsoft.AspNetCore.Mvc;

namespace CivicRegistry.API.Controllers.Api
{
    /// <summary>
    /// Base controller cho tất cả API controllers
    /// </summary>
    [ApiController]
    [Route("api/[controller]")]
    public class BaseApiController : ControllerBase
    {
        /// <summary>
        /// Trả về response thành công
        /// </summary>
        /// <param name="data">Dữ liệu trả về</param>
        /// <param name="message">Thông báo</param>
        /// <returns>Response object</returns>
        protected IActionResult Success(object? data = null, string message = "Success")
        {
            var response = new
            {
                success = true,
                message = message,
                data = data
            };
            return Ok(response);
        }

        /// <summary>
        /// Trả về response lỗi
        /// </summary>
        /// <param name="message">Thông báo lỗi</param>
        /// <param name="errors">Chi tiết lỗi</param>
        /// <returns>Response object</returns>
        protected IActionResult Error(string message = "Error", object? errors = null)
        {
            var response = new
            {
                success = false,
                message = message,
                errors = errors
            };
            return BadRequest(response);
        }
    }
}
