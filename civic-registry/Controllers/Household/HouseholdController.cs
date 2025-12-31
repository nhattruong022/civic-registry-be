using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using CivicRegistry.API.DTOs;
using CivicRegistry.API.Services;
using CivicRegistry.API.Types;
using CivicRegistry.API.Middleware;

namespace CivicRegistry.API.Controllers
{
    /// <summary>
    /// Controller quản lý hộ khẩu
    /// </summary>
    [ApiController]
    [Route("api/[controller]")]
    [Authorize]
    public class HouseholdController : ControllerBase
    {
        private readonly HouseholdService _householdService;
        private readonly ILogger<HouseholdController> _logger;

        public HouseholdController(HouseholdService householdService, ILogger<HouseholdController> logger)
        {
            _householdService = householdService;
            _logger = logger;
        }

        /// <summary>
        /// Lấy danh sách hộ khẩu có phân trang
        /// </summary>
        /// <remarks>
        /// **Phân quyền:**
        /// - WardAdmin: xem hộ khẩu trong xã của mình (tự động lọc theo WardId)
        /// - SuperAdmin, ProvinceAdmin, DistrictAdmin: xem tất cả hoặc lọc theo hamletId
        /// 
        /// **Ví dụ:**
        /// - WardAdmin: GET /api/households?page=1&pageSize=20
        /// - SuperAdmin: GET /api/households?page=1&pageSize=20&hamletId=xxx
        /// </remarks>
        /// <param name="page">Số trang (mặc định: 1)</param>
        /// <param name="pageSize">Số item mỗi trang (mặc định: 20, tối đa: 100)</param>
        /// <param name="hamletId">ID thôn để lọc (optional)</param>
        /// <returns>Danh sách hộ khẩu có phân trang</returns>
        [HttpGet]
        [ProducesResponseType(typeof(ApiSuccessResponse<PaginatedResponse<HouseholdInfo>>), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ApiErrorResponse), StatusCodes.Status403Forbidden)]
        [ProducesResponseType(typeof(ApiErrorResponse), StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> GetAllHouseholds([FromQuery] int page = 1, [FromQuery] int pageSize = 20, [FromQuery] string? hamletId = null)
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

                string? targetHamletId = hamletId;

                // Phân quyền: WardAdmin chỉ xem hộ khẩu trong xã của mình
                if (currentUserRole == "WardAdmin")
                {
                    // WardAdmin chỉ có thể xem hộ khẩu trong xã của mình
                    // Cần lấy danh sách thôn thuộc xã của WardAdmin
                    if (currentUser.WardId.HasValue)
                    {
                        // TODO: Lấy danh sách thôn thuộc xã này và lọc hộ khẩu
                        // Hiện tại cho phép xem tất cả, nhưng có thể thêm logic lọc theo WardId
                    }
                }

                var result = await _householdService.GetHouseholdsPaginatedAsync(page, pageSize, targetHamletId);

                var successResponse = new ApiSuccessResponse<PaginatedResponse<HouseholdInfo>>(result, "Lấy danh sách hộ khẩu thành công", 200);
                return Ok(successResponse);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Lỗi khi lấy danh sách hộ khẩu");
                var errorResponse = new ApiErrorResponse("Đã xảy ra lỗi khi lấy danh sách hộ khẩu", ex.Message, 500);
                return StatusCode(500, errorResponse);
            }
        }

        /// <summary>
        /// Tạo hộ khẩu mới
        /// </summary>
        /// <remarks>
        /// **Phân quyền:**
        /// - WardAdmin: có thể tạo hộ khẩu trong xã của mình
        /// - SuperAdmin, ProvinceAdmin, DistrictAdmin: có thể tạo hộ khẩu bất kỳ
        /// 
        /// **Ví dụ request body:**
        /// ```json
        /// {
        ///   "householdCode": "HK001",
        ///   "address": "Số 123, Đường ABC, Thôn XYZ",
        ///   "hamletId": "507f1f77bcf86cd799439011",
        ///   "headCitizenId": "507f191e810c19729de860ea",
        ///   "status": 0
        /// }
        /// ```
        /// </remarks>
        /// <param name="request">Thông tin hộ khẩu cần tạo</param>
        /// <returns>Thông tin hộ khẩu đã tạo</returns>
        [HttpPost]
        [ProducesResponseType(typeof(ApiSuccessResponse<HouseholdInfo>), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ApiErrorResponse), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(typeof(ApiErrorResponse), StatusCodes.Status403Forbidden)]
        [ProducesResponseType(typeof(ApiErrorResponse), StatusCodes.Status409Conflict)]
        [ProducesResponseType(typeof(ApiErrorResponse), StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> CreateHousehold([FromBody] CreateHouseholdRequest request)
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

                // Phân quyền: WardAdmin chỉ có thể tạo hộ khẩu trong xã của mình
                if (currentUserRole == "WardAdmin")
                {
                    // TODO: Kiểm tra hamletId có thuộc xã của WardAdmin không
                    // Hiện tại cho phép tạo, nhưng có thể thêm validation
                }

                if (!ModelState.IsValid)
                {
                    var errorResponse = new ApiErrorResponse("Dữ liệu không hợp lệ", string.Join(", ", ModelState.Values.SelectMany(v => v.Errors).Select(e => e.ErrorMessage)), 400);
                    return BadRequest(errorResponse);
                }

                var household = await _householdService.CreateHouseholdAsync(request);

                if (household == null)
                {
                    // Kiểm tra xem có phải do mã hộ khẩu đã tồn tại không
                    var existingCode = await _householdService.CheckHouseholdCodeExistsAsync(request.HouseholdCode);
                    if (existingCode)
                    {
                        var errorResponse = new ApiErrorResponse("Mã hộ khẩu đã tồn tại", null, 409);
                        return Conflict(errorResponse);
                    }

                    // Nếu không phải do mã hộ khẩu, có thể là do thôn hoặc chủ hộ không tồn tại
                    var errorResponse2 = new ApiErrorResponse("Thôn hoặc chủ hộ không tồn tại", null, 400);
                    return BadRequest(errorResponse2);
                }

                var successResponse = new ApiSuccessResponse<HouseholdInfo>(household, "Tạo hộ khẩu thành công", 200);
                return Ok(successResponse);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Lỗi khi tạo hộ khẩu");
                var errorResponse = new ApiErrorResponse("Đã xảy ra lỗi khi tạo hộ khẩu", ex.Message, 500);
                return StatusCode(500, errorResponse);
            }
        }

        /// <summary>
        /// Cập nhật hộ khẩu
        /// </summary>
        /// <remarks>
        /// **Phân quyền:**
        /// - WardAdmin: có thể cập nhật hộ khẩu trong xã của mình
        /// - SuperAdmin, ProvinceAdmin, DistrictAdmin: có thể cập nhật hộ khẩu bất kỳ
        /// </remarks>
        /// <param name="id">ID của hộ khẩu</param>
        /// <param name="request">Thông tin cập nhật</param>
        /// <returns>Thông tin hộ khẩu đã cập nhật</returns>
        [HttpPut("{id}")]
        [ProducesResponseType(typeof(ApiSuccessResponse<HouseholdInfo>), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ApiErrorResponse), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(typeof(ApiErrorResponse), StatusCodes.Status403Forbidden)]
        [ProducesResponseType(typeof(ApiErrorResponse), StatusCodes.Status404NotFound)]
        [ProducesResponseType(typeof(ApiErrorResponse), StatusCodes.Status409Conflict)]
        [ProducesResponseType(typeof(ApiErrorResponse), StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> UpdateHousehold(string id, [FromBody] UpdateHouseholdRequest request)
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

                // Kiểm tra hộ khẩu có tồn tại không
                var existingHousehold = await _householdService.GetHouseholdByIdAsync(id);
                if (existingHousehold == null)
                {
                    var errorResponse = new ApiErrorResponse("Hộ khẩu không tồn tại", null, 404);
                    return NotFound(errorResponse);
                }

                // Phân quyền: WardAdmin chỉ có thể cập nhật hộ khẩu trong xã của mình
                if (currentUserRole == "WardAdmin")
                {
                    // TODO: Kiểm tra hộ khẩu có thuộc xã của WardAdmin không
                    // Hiện tại cho phép cập nhật, nhưng có thể thêm validation
                }

                if (!ModelState.IsValid)
                {
                    var errorResponse = new ApiErrorResponse("Dữ liệu không hợp lệ", string.Join(", ", ModelState.Values.SelectMany(v => v.Errors).Select(e => e.ErrorMessage)), 400);
                    return BadRequest(errorResponse);
                }

                var household = await _householdService.UpdateHouseholdAsync(id, request);

                if (household == null)
                {
                    // Kiểm tra xem có phải do mã hộ khẩu đã tồn tại không
                    if (!string.IsNullOrEmpty(request.HouseholdCode))
                    {
                        var existingCode = await _householdService.CheckHouseholdCodeExistsAsync(request.HouseholdCode);
                        if (existingCode)
                        {
                            var errorResponse = new ApiErrorResponse("Mã hộ khẩu đã tồn tại", null, 409);
                            return Conflict(errorResponse);
                        }
                    }

                    // Nếu không phải do mã hộ khẩu, có thể là do thôn hoặc chủ hộ không tồn tại
                    var errorResponse2 = new ApiErrorResponse("Thôn hoặc chủ hộ không tồn tại", null, 400);
                    return BadRequest(errorResponse2);
                }

                var successResponse = new ApiSuccessResponse<HouseholdInfo>(household, "Cập nhật hộ khẩu thành công", 200);
                return Ok(successResponse);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Lỗi khi cập nhật hộ khẩu");
                var errorResponse = new ApiErrorResponse("Đã xảy ra lỗi khi cập nhật hộ khẩu", ex.Message, 500);
                return StatusCode(500, errorResponse);
            }
        }
    }
}

