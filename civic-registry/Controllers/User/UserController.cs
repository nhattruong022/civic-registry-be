using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using CivicRegistry.API.DTOs;
using CivicRegistry.API.Services;
using CivicRegistry.API.Types;
using CivicRegistry.API.Middleware;

namespace CivicRegistry.API.Controllers
{
    /// <summary>
    /// Controller quản lý tài khoản cán bộ
    /// </summary>
    [ApiController]
    [Route("api/[controller]")]
    [Authorize] // Yêu cầu authentication cho tất cả các endpoint trong controller này
    public class UserController : ControllerBase
    {
        private readonly UserService _userService;
        private readonly ILogger<UserController> _logger;

        public UserController(UserService userService, ILogger<UserController> logger)
        {
            _userService = userService;
            _logger = logger;
        }

        /// <summary>
        /// Lấy danh sách users có phân trang
        /// </summary>
        /// <remarks>
        /// **Phân quyền:**
        /// - SuperAdmin: xem tất cả users, có thể lọc theo role
        /// - ProvinceAdmin: chỉ xem DistrictAdmin
        /// - DistrictAdmin: chỉ xem WardAdmin
        /// 
        /// **Ví dụ:**
        /// - SuperAdmin: GET /api/users?page=1&pageSize=20
        /// - ProvinceAdmin: GET /api/users?page=1&pageSize=20&role=DistrictAdmin
        /// </remarks>
        /// <param name="page">Số trang (mặc định: 1)</param>
        /// <param name="pageSize">Số item mỗi trang (mặc định: 20, tối đa: 100)</param>
        /// <param name="role">Lọc theo role (optional). SuperAdmin: xem tất cả, ProvinceAdmin: xem DistrictAdmin, DistrictAdmin: xem WardAdmin</param>
        /// <returns>Danh sách users có phân trang</returns>
        [HttpGet]
        [ProducesResponseType(typeof(ApiSuccessResponse<PaginatedResponse<UserInfo>>), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ApiErrorResponse), StatusCodes.Status403Forbidden)]
        [ProducesResponseType(typeof(ApiErrorResponse), StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> GetAllUsers([FromQuery] int page = 1, [FromQuery] int pageSize = 20, [FromQuery] string? role = null)
        {
            try
            {
                // Lấy role của user hiện tại từ HttpContext (được set bởi middleware)
                var currentUserRole = HttpContext.GetCurrentUserRole();
                var currentUser = HttpContext.GetCurrentUser();

                if (currentUser == null)
                {
                    var errorResponse = new ApiErrorResponse("Không tìm thấy thông tin user", null, 401);
                    return Unauthorized(errorResponse);
                }

                PaginatedResponse<UserInfo> result;

                // Phân quyền theo role
                switch (currentUserRole)
                {
                    case "SuperAdmin":
                        // SuperAdmin: xem tất cả users, có thể lọc theo role
                        result = await _userService.GetUsersPaginatedAsync(page, pageSize, role);
                        break;

                    case "ProvinceAdmin":
                        // ProvinceAdmin: chỉ xem DistrictAdmin (và có thể lọc theo role=DistrictAdmin)
                        if (!string.IsNullOrEmpty(role) && role != "DistrictAdmin")
                        {
                            var forbiddenResponse = new ApiErrorResponse("Bạn chỉ có quyền xem DistrictAdmin", null, 403);
                            return StatusCode(403, forbiddenResponse);
                        }
                        result = await _userService.GetUsersPaginatedAsync(page, pageSize, "DistrictAdmin");
                        break;

                    case "DistrictAdmin":
                        // DistrictAdmin: chỉ xem WardAdmin (và có thể lọc theo role=WardAdmin)
                        if (!string.IsNullOrEmpty(role) && role != "WardAdmin")
                        {
                            var forbiddenResponse = new ApiErrorResponse("Bạn chỉ có quyền xem WardAdmin", null, 403);
                            return StatusCode(403, forbiddenResponse);
                        }
                        result = await _userService.GetUsersPaginatedAsync(page, pageSize, "WardAdmin");
                        break;

                    case "WardAdmin":
                    case "Citizen":
                    default:
                        // WardAdmin và Citizen không có quyền xem danh sách users
                        var errorResponse2 = new ApiErrorResponse("Bạn không có quyền truy cập API này", null, 403);
                        return StatusCode(403, errorResponse2);
                }

                var successResponse = new ApiSuccessResponse<PaginatedResponse<UserInfo>>(result, "Lấy danh sách users thành công", 200);
                return Ok(successResponse);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Lỗi khi lấy danh sách users");
                var errorResponse = new ApiErrorResponse("Đã xảy ra lỗi khi lấy danh sách users", ex.Message, 500);
                return StatusCode(500, errorResponse);
            }
        }

        /// <summary>
        /// Tạo user mới
        /// </summary>
        /// <remarks>
        /// **Phân quyền:**
        /// - SuperAdmin: có thể tạo ProvinceAdmin, DistrictAdmin, WardAdmin
        /// - ProvinceAdmin: chỉ có thể tạo DistrictAdmin
        /// - DistrictAdmin: chỉ có thể tạo WardAdmin
        /// 
        /// **Ví dụ request body:**
        /// ```json
        /// {
        ///   "username": "districtadmin1",
        ///   "password": "Password123",
        ///   "role": "DistrictAdmin",
        ///   "provinceId": 1,
        ///   "districtId": 10,
        ///   "wardId": null,
        ///   "isActive": true
        /// }
        /// ```
        /// </remarks>
        /// <param name="request">Thông tin user cần tạo</param>
        /// <returns>Thông tin user đã tạo</returns>
        [HttpPost]
        [ProducesResponseType(typeof(ApiSuccessResponse<UserInfo>), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ApiErrorResponse), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(typeof(ApiErrorResponse), StatusCodes.Status403Forbidden)]
        [ProducesResponseType(typeof(ApiErrorResponse), StatusCodes.Status409Conflict)]
        [ProducesResponseType(typeof(ApiErrorResponse), StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> CreateUser([FromBody] CreateUserRequest request)
        {
            try
            {
                // Kiểm tra quyền tạo user
                var currentUser = HttpContext.GetCurrentUser();
                var currentUserRole = HttpContext.GetCurrentUserRole();

                if (currentUser == null)
                {
                    var errorResponse = new ApiErrorResponse("Không tìm thấy thông tin user", null, 401);
                    return Unauthorized(errorResponse);
                }

                // Phân quyền: SuperAdmin có thể tạo ProvinceAdmin, DistrictAdmin, WardAdmin
                // ProvinceAdmin chỉ có thể tạo DistrictAdmin
                // DistrictAdmin chỉ có thể tạo WardAdmin
                bool canCreate = false;
                switch (currentUserRole)
                {
                    case "SuperAdmin":
                        canCreate = request.Role == "ProvinceAdmin" || request.Role == "DistrictAdmin" || request.Role == "WardAdmin";
                        break;
                    case "ProvinceAdmin":
                        canCreate = request.Role == "DistrictAdmin";
                        break;
                    case "DistrictAdmin":
                        canCreate = request.Role == "WardAdmin";
                        break;
                }

                if (!canCreate)
                {
                    var forbiddenResponse = new ApiErrorResponse($"Bạn không có quyền tạo user với role {request.Role}", null, 403);
                    return StatusCode(403, forbiddenResponse);
                }

                if (!ModelState.IsValid)
                {
                    var errorResponse = new ApiErrorResponse("Dữ liệu không hợp lệ", string.Join(", ", ModelState.Values.SelectMany(v => v.Errors).Select(e => e.ErrorMessage)), 400);
                    return BadRequest(errorResponse);
                }

                var user = await _userService.CreateUserAsync(request);

                if (user == null)
                {
                    // Kiểm tra xem có phải do username đã tồn tại không
                    var existingUser = await _userService.CheckUsernameExistsAsync(request.Username);
                    if (existingUser)
                    {
                        var errorResponse = new ApiErrorResponse("Username đã tồn tại", null, 409);
                        return Conflict(errorResponse);
                    }

                    // Nếu không phải do username, thì là do role không hợp lệ
                    var roleErrorResponse = new ApiErrorResponse("Role không hợp lệ. Role phải là một trong: SuperAdmin, ProvinceAdmin, DistrictAdmin, WardAdmin, Citizen", null, 400);
                    return BadRequest(roleErrorResponse);
                }

                var successResponse = new ApiSuccessResponse<UserInfo>(user, "Tạo user thành công", 200);
                return Ok(successResponse);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Lỗi khi tạo user");
                var errorResponse = new ApiErrorResponse("Đã xảy ra lỗi khi tạo user", ex.Message, 500);
                return StatusCode(500, errorResponse);
            }
        }

        /// <summary>
        /// Cập nhật user
        /// SuperAdmin: cập nhật bất kỳ user nào
        /// ProvinceAdmin: chỉ cập nhật DistrictAdmin
        /// DistrictAdmin: chỉ cập nhật WardAdmin
        /// </summary>
        /// <param name="id">ID của user</param>
        /// <param name="request">Thông tin cập nhật</param>
        /// <returns>Thông tin user đã cập nhật</returns>
        [HttpPut("{id}")]
        [ProducesResponseType(typeof(ApiSuccessResponse<UserInfo>), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ApiErrorResponse), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(typeof(ApiErrorResponse), StatusCodes.Status403Forbidden)]
        [ProducesResponseType(typeof(ApiErrorResponse), StatusCodes.Status404NotFound)]
        [ProducesResponseType(typeof(ApiErrorResponse), StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> UpdateUser(string id, [FromBody] UpdateUserRequest request)
        {
            try
            {
                // Kiểm tra quyền cập nhật user
                var currentUser = HttpContext.GetCurrentUser();
                var currentUserRole = HttpContext.GetCurrentUserRole();

                if (currentUser == null)
                {
                    var errorResponse = new ApiErrorResponse("Không tìm thấy thông tin user", null, 401);
                    return Unauthorized(errorResponse);
                }

                // Kiểm tra user cần cập nhật có tồn tại không và lấy role của user đó
                var targetUser = await _userService.GetUserByIdAsync(id);
                if (targetUser == null)
                {
                    var errorResponse = new ApiErrorResponse("User không tồn tại", null, 404);
                    return NotFound(errorResponse);
                }

                // Phân quyền: SuperAdmin có thể cập nhật bất kỳ user nào
                // ProvinceAdmin chỉ có thể cập nhật DistrictAdmin
                // DistrictAdmin chỉ có thể cập nhật WardAdmin
                bool canUpdate = false;
                switch (currentUserRole)
                {
                    case "SuperAdmin":
                        canUpdate = true; // SuperAdmin có thể cập nhật bất kỳ user nào
                        break;
                    case "ProvinceAdmin":
                        canUpdate = targetUser.Role == "DistrictAdmin";
                        break;
                    case "DistrictAdmin":
                        canUpdate = targetUser.Role == "WardAdmin";
                        break;
                }

                if (!canUpdate)
                {
                    var forbiddenResponse = new ApiErrorResponse("Bạn không có quyền cập nhật user này", null, 403);
                    return StatusCode(403, forbiddenResponse);
                }

                // Kiểm tra nếu có thay đổi role, phải tuân theo quyền tạo user
                if (!string.IsNullOrEmpty(request.Role) && request.Role != targetUser.Role)
                {
                    bool canChangeRole = false;
                    switch (currentUserRole)
                    {
                        case "SuperAdmin":
                            canChangeRole = request.Role == "ProvinceAdmin" || request.Role == "DistrictAdmin" || request.Role == "WardAdmin";
                            break;
                        case "ProvinceAdmin":
                            canChangeRole = request.Role == "DistrictAdmin";
                            break;
                        case "DistrictAdmin":
                            canChangeRole = request.Role == "WardAdmin";
                            break;
                    }

                    if (!canChangeRole)
                    {
                        var forbiddenResponse = new ApiErrorResponse($"Bạn không có quyền thay đổi role thành {request.Role}", null, 403);
                        return StatusCode(403, forbiddenResponse);
                    }
                }

                if (!ModelState.IsValid)
                {
                    var errorResponse = new ApiErrorResponse("Dữ liệu không hợp lệ", string.Join(", ", ModelState.Values.SelectMany(v => v.Errors).Select(e => e.ErrorMessage)), 400);
                    return BadRequest(errorResponse);
                }

                var user = await _userService.UpdateUserAsync(id, request);

                if (user == null)
                {
                    // Kiểm tra xem có phải do user không tồn tại không
                    var existingUser = await _userService.GetUserByIdAsync(id);
                    if (existingUser == null)
                    {
                        var errorResponse = new ApiErrorResponse("User không tồn tại", null, 404);
                        return NotFound(errorResponse);
                    }

                    // Nếu không phải do user không tồn tại, thì là do role không hợp lệ
                    var roleErrorResponse = new ApiErrorResponse("Role không hợp lệ. Role phải là một trong: SuperAdmin, ProvinceAdmin, DistrictAdmin, WardAdmin, Citizen", null, 400);
                    return BadRequest(roleErrorResponse);
                }

                var successResponse = new ApiSuccessResponse<UserInfo>(user, "Cập nhật user thành công", 200);
                return Ok(successResponse);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Lỗi khi cập nhật user");
                var errorResponse = new ApiErrorResponse("Đã xảy ra lỗi khi cập nhật user", ex.Message, 500);
                return StatusCode(500, errorResponse);
            }
        }

        /// <summary>
        /// Xóa user (soft delete)
        /// SuperAdmin: xóa bất kỳ user nào
        /// ProvinceAdmin: chỉ xóa DistrictAdmin
        /// DistrictAdmin: chỉ xóa WardAdmin
        /// </summary>
        /// <param name="id">ID của user</param>
        /// <returns>Thông báo xóa thành công</returns>
        [HttpDelete("{id}")]
        [ProducesResponseType(typeof(ApiSuccessResponse<object>), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ApiErrorResponse), StatusCodes.Status403Forbidden)]
        [ProducesResponseType(typeof(ApiErrorResponse), StatusCodes.Status404NotFound)]
        [ProducesResponseType(typeof(ApiErrorResponse), StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> DeleteUser(string id)
        {
            try
            {
                // Kiểm tra quyền xóa user
                var currentUser = HttpContext.GetCurrentUser();
                var currentUserRole = HttpContext.GetCurrentUserRole();

                if (currentUser == null)
                {
                    var errorResponse = new ApiErrorResponse("Không tìm thấy thông tin user", null, 401);
                    return Unauthorized(errorResponse);
                }

                // Kiểm tra user cần xóa có tồn tại không và lấy role của user đó
                var targetUser = await _userService.GetUserByIdAsync(id);
                if (targetUser == null)
                {
                    var errorResponse = new ApiErrorResponse("User không tồn tại", null, 404);
                    return NotFound(errorResponse);
                }

                // Phân quyền: SuperAdmin có thể xóa bất kỳ user nào
                // ProvinceAdmin chỉ có thể xóa DistrictAdmin
                // DistrictAdmin chỉ có thể xóa WardAdmin
                bool canDelete = false;
                switch (currentUserRole)
                {
                    case "SuperAdmin":
                        canDelete = true; // SuperAdmin có thể xóa bất kỳ user nào
                        break;
                    case "ProvinceAdmin":
                        canDelete = targetUser.Role == "DistrictAdmin";
                        break;
                    case "DistrictAdmin":
                        canDelete = targetUser.Role == "WardAdmin";
                        break;
                }

                if (!canDelete)
                {
                    var forbiddenResponse = new ApiErrorResponse("Bạn không có quyền xóa user này", null, 403);
                    return StatusCode(403, forbiddenResponse);
                }

                var result = await _userService.DeleteUserAsync(id);

                if (!result)
                {
                    var errorResponse = new ApiErrorResponse("User không tồn tại", null, 404);
                    return NotFound(errorResponse);
                }

                var successResponse = new ApiSuccessResponse<object>(null, "Xóa user thành công", 200);
                return Ok(successResponse);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Lỗi khi xóa user");
                var errorResponse = new ApiErrorResponse("Đã xảy ra lỗi khi xóa user", ex.Message, 500);
                return StatusCode(500, errorResponse);
            }
        }

        /// <summary>
        /// Reset password cho user
        /// </summary>
        /// <param name="id">ID của user</param>
        /// <param name="request">Mật khẩu mới</param>
        /// <returns>Thông báo reset password thành công</returns>
        [HttpPut("reset-password/{id}")]
        [ProducesResponseType(typeof(ApiSuccessResponse<object>), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ApiErrorResponse), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(typeof(ApiErrorResponse), StatusCodes.Status404NotFound)]
        [ProducesResponseType(typeof(ApiErrorResponse), StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> ResetPassword(string id, [FromBody] ResetPasswordRequest request)
        {
            try
            {
                if (!ModelState.IsValid)
                {
                    var errorResponse = new ApiErrorResponse("Dữ liệu không hợp lệ", string.Join(", ", ModelState.Values.SelectMany(v => v.Errors).Select(e => e.ErrorMessage)), 400);
                    return BadRequest(errorResponse);
                }

                var result = await _userService.ResetPasswordAsync(id, request);

                if (!result)
                {
                    var errorResponse = new ApiErrorResponse("User không tồn tại", null, 404);
                    return NotFound(errorResponse);
                }

                var successResponse = new ApiSuccessResponse<object>(null, "Reset password thành công", 200);
                return Ok(successResponse);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Lỗi khi reset password");
                var errorResponse = new ApiErrorResponse("Đã xảy ra lỗi khi reset password", ex.Message, 500);
                return StatusCode(500, errorResponse);
            }
        }
    }
}

