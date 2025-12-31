using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using CivicRegistry.API.DTOs;
using CivicRegistry.API.Services;
using CivicRegistry.API.Types;
using CivicRegistry.API.Middleware;

namespace CivicRegistry.API.Controllers
{
    /// <summary>
    /// Controller xử lý thống kê
    /// </summary>
    [ApiController]
    [Route("api/[controller]")]
    [Authorize]
    public class StatisticsController : ControllerBase
    {
        private readonly StatisticsService _statisticsService;
        private readonly ILogger<StatisticsController> _logger;

        public StatisticsController(StatisticsService statisticsService, ILogger<StatisticsController> logger)
        {
            _statisticsService = statisticsService;
            _logger = logger;
        }

        /// <summary>
        /// Lấy thống kê tỉnh
        /// </summary>
        /// <remarks>
        /// **Phân quyền:**
        /// - SuperAdmin: xem thống kê toàn quốc (không cần provinceId)
        /// - ProvinceAdmin: xem thống kê tỉnh của mình (tự động lấy từ user.ProvinceId)
        /// 
        /// **Ví dụ:**
        /// - SuperAdmin: GET /api/statistics/province
        /// - ProvinceAdmin: GET /api/statistics/province?provinceId=1
        /// </remarks>
        /// <param name="provinceId">ID tỉnh (optional, SuperAdmin có thể bỏ qua để xem toàn quốc)</param>
        /// <returns>Thống kê tỉnh</returns>
        [HttpGet("province")]
        [ProducesResponseType(typeof(ApiSuccessResponse<ProvinceStatisticsResponse>), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ApiErrorResponse), StatusCodes.Status403Forbidden)]
        [ProducesResponseType(typeof(ApiErrorResponse), StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> GetProvinceStatistics([FromQuery] int? provinceId = null)
        {
            try
            {
                var currentUser = HttpContext.GetCurrentUser();
                var currentUserRole = HttpContext.GetCurrentUserRole();

                if (currentUser == null)
                {
                    var errorResponse = new ApiErrorResponse("Không tìm thấy thông tin user", null, 401);
                    return Unauthorized(errorResponse);
                }

                int? targetProvinceId = provinceId;

                // Phân quyền
                switch (currentUserRole)
                {
                    case "SuperAdmin":
                        // SuperAdmin có thể xem tất cả hoặc lọc theo provinceId
                        break;

                    case "ProvinceAdmin":
                        // ProvinceAdmin chỉ có thể xem thống kê tỉnh của mình
                        if (currentUser.ProvinceId.HasValue)
                        {
                            targetProvinceId = currentUser.ProvinceId.Value;
                        }
                        else
                        {
                            var errorResponse = new ApiErrorResponse("User không có ProvinceId", null, 400);
                            return BadRequest(errorResponse);
                        }
                        break;

                    default:
                        // Các role khác không có quyền xem thống kê tỉnh
                        var forbiddenResponse = new ApiErrorResponse("Bạn không có quyền xem thống kê tỉnh", null, 403);
                        return StatusCode(403, forbiddenResponse);
                }

                var statistics = await _statisticsService.GetProvinceStatisticsAsync(targetProvinceId);

                var successResponse = new ApiSuccessResponse<ProvinceStatisticsResponse>(statistics, "Lấy thống kê tỉnh thành công", 200);
                return Ok(successResponse);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Lỗi khi lấy thống kê tỉnh");
                var errorResponse = new ApiErrorResponse("Đã xảy ra lỗi khi lấy thống kê tỉnh", ex.Message, 500);
                return StatusCode(500, errorResponse);
            }
        }
    }
}

