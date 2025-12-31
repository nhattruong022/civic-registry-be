using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using CivicRegistry.API.DTOs;
using CivicRegistry.API.Services;
using CivicRegistry.API.Types;
using CivicRegistry.API.Middleware;

namespace CivicRegistry.API.Controllers
{
    /// <summary>
    /// Controller xử lý yêu cầu công dân
    /// </summary>
    [ApiController]
    [Route("api/[controller]")]
    [Authorize]
    public class RequestController : ControllerBase
    {
        private readonly RequestService _requestService;
        private readonly ILogger<RequestController> _logger;

        public RequestController(RequestService requestService, ILogger<RequestController> logger)
        {
            _requestService = requestService;
            _logger = logger;
        }

        /// <summary>
        /// Duyệt yêu cầu
        /// </summary>
        /// <remarks>
        /// **Phân quyền:**
        /// - ProvinceAdmin: có thể duyệt yêu cầu cấp tỉnh
        /// - DistrictAdmin: có thể duyệt yêu cầu cấp huyện
        /// - WardAdmin: có thể duyệt yêu cầu cấp xã
        /// </remarks>
        /// <param name="id">ID của yêu cầu</param>
        /// <returns>Thông tin yêu cầu đã được duyệt</returns>
        [HttpPut("{id}/approve")]
        [ProducesResponseType(typeof(ApiSuccessResponse<ApproveRequestResponse>), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ApiErrorResponse), StatusCodes.Status403Forbidden)]
        [ProducesResponseType(typeof(ApiErrorResponse), StatusCodes.Status404NotFound)]
        [ProducesResponseType(typeof(ApiErrorResponse), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(typeof(ApiErrorResponse), StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> ApproveRequest(string id)
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

                // Kiểm tra yêu cầu có tồn tại không
                var request = await _requestService.GetRequestByIdAsync(id);
                if (request == null)
                {
                    var errorResponse = new ApiErrorResponse("Yêu cầu không tồn tại", null, 404);
                    return NotFound(errorResponse);
                }

                // Kiểm tra yêu cầu đã được xử lý chưa
                if (request.Status != 0) // 0 = Pending
                {
                    var errorResponse = new ApiErrorResponse("Yêu cầu đã được xử lý", null, 400);
                    return BadRequest(errorResponse);
                }

                // Phân quyền: ProvinceAdmin, DistrictAdmin, WardAdmin có thể duyệt
                if (!HttpContext.HasAnyRole("ProvinceAdmin", "DistrictAdmin", "WardAdmin"))
                {
                    var forbiddenResponse = new ApiErrorResponse("Bạn không có quyền duyệt yêu cầu", null, 403);
                    return StatusCode(403, forbiddenResponse);
                }

                // Duyệt yêu cầu
                var approvedRequest = await _requestService.ApproveRequestAsync(id, currentUser.Id);

                if (approvedRequest == null)
                {
                    var errorResponse = new ApiErrorResponse("Không thể duyệt yêu cầu", null, 500);
                    return StatusCode(500, errorResponse);
                }

                var response = new ApproveRequestResponse
                {
                    Message = "Yêu cầu đã được duyệt thành công",
                    Request = approvedRequest
                };

                var successResponse = new ApiSuccessResponse<ApproveRequestResponse>(response, "Duyệt yêu cầu thành công", 200);
                return Ok(successResponse);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Lỗi khi duyệt yêu cầu");
                var errorResponse = new ApiErrorResponse("Đã xảy ra lỗi khi duyệt yêu cầu", ex.Message, 500);
                return StatusCode(500, errorResponse);
            }
        }

        /// <summary>
        /// Từ chối yêu cầu
        /// </summary>
        /// <remarks>
        /// **Phân quyền:**
        /// - ProvinceAdmin: có thể từ chối yêu cầu cấp tỉnh
        /// - DistrictAdmin: có thể từ chối yêu cầu cấp huyện
        /// - WardAdmin: có thể từ chối yêu cầu cấp xã
        /// </remarks>
        /// <param name="id">ID của yêu cầu</param>
        /// <returns>Thông tin yêu cầu đã bị từ chối</returns>
        [HttpPut("{id}/reject")]
        [ProducesResponseType(typeof(ApiSuccessResponse<RejectRequestResponse>), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ApiErrorResponse), StatusCodes.Status403Forbidden)]
        [ProducesResponseType(typeof(ApiErrorResponse), StatusCodes.Status404NotFound)]
        [ProducesResponseType(typeof(ApiErrorResponse), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(typeof(ApiErrorResponse), StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> RejectRequest(string id)
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

                // Kiểm tra yêu cầu có tồn tại không
                var request = await _requestService.GetRequestByIdAsync(id);
                if (request == null)
                {
                    var errorResponse = new ApiErrorResponse("Yêu cầu không tồn tại", null, 404);
                    return NotFound(errorResponse);
                }

                // Kiểm tra yêu cầu đã được xử lý chưa
                if (request.Status != 0) // 0 = Pending
                {
                    var errorResponse = new ApiErrorResponse("Yêu cầu đã được xử lý", null, 400);
                    return BadRequest(errorResponse);
                }

                // Phân quyền: ProvinceAdmin, DistrictAdmin, WardAdmin có thể từ chối
                if (!HttpContext.HasAnyRole("ProvinceAdmin", "DistrictAdmin", "WardAdmin"))
                {
                    var forbiddenResponse = new ApiErrorResponse("Bạn không có quyền từ chối yêu cầu", null, 403);
                    return StatusCode(403, forbiddenResponse);
                }

                // Từ chối yêu cầu
                var rejectedRequest = await _requestService.RejectRequestAsync(id, currentUser.Id);

                if (rejectedRequest == null)
                {
                    var errorResponse = new ApiErrorResponse("Không thể từ chối yêu cầu", null, 500);
                    return StatusCode(500, errorResponse);
                }

                var response = new RejectRequestResponse
                {
                    Message = "Yêu cầu đã bị từ chối",
                    Request = rejectedRequest
                };

                var successResponse = new ApiSuccessResponse<RejectRequestResponse>(response, "Từ chối yêu cầu thành công", 200);
                return Ok(successResponse);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Lỗi khi từ chối yêu cầu");
                var errorResponse = new ApiErrorResponse("Đã xảy ra lỗi khi từ chối yêu cầu", ex.Message, 500);
                return StatusCode(500, errorResponse);
            }
        }
    }
}

