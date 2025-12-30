using Microsoft.AspNetCore.Mvc;
using CivicRegistry.API.DTOs;
using CivicRegistry.API.Services;
using CivicRegistry.API.Types;

namespace CivicRegistry.API.Controllers
{
    /// <summary>
    /// Controller quản lý tài khoản cán bộ
    /// </summary>
    [ApiController]
    [Route("api/[controller]")]
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
        /// Lấy danh sách tất cả users
        /// </summary>
        /// <returns>Danh sách users</returns>
        [HttpGet]
        [ProducesResponseType(typeof(ApiSuccessResponse<List<UserInfo>>), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ApiErrorResponse), StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> GetAllUsers()
        {
            try
            {
                var users = await _userService.GetAllUsersAsync();
                var successResponse = new ApiSuccessResponse<List<UserInfo>>(users, "Lấy danh sách users thành công", 200);
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
        /// <param name="request">Thông tin user cần tạo</param>
        /// <returns>Thông tin user đã tạo</returns>
        [HttpPost]
        [ProducesResponseType(typeof(ApiSuccessResponse<UserInfo>), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ApiErrorResponse), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(typeof(ApiErrorResponse), StatusCodes.Status409Conflict)]
        [ProducesResponseType(typeof(ApiErrorResponse), StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> CreateUser([FromBody] CreateUserRequest request)
        {
            try
            {
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
        /// </summary>
        /// <param name="id">ID của user</param>
        /// <param name="request">Thông tin cập nhật</param>
        /// <returns>Thông tin user đã cập nhật</returns>
        [HttpPut("{id}")]
        [ProducesResponseType(typeof(ApiSuccessResponse<UserInfo>), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ApiErrorResponse), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(typeof(ApiErrorResponse), StatusCodes.Status404NotFound)]
        [ProducesResponseType(typeof(ApiErrorResponse), StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> UpdateUser(string id, [FromBody] UpdateUserRequest request)
        {
            try
            {
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
        /// </summary>
        /// <param name="id">ID của user</param>
        /// <returns>Thông báo xóa thành công</returns>
        [HttpDelete("{id}")]
        [ProducesResponseType(typeof(ApiSuccessResponse<object>), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ApiErrorResponse), StatusCodes.Status404NotFound)]
        [ProducesResponseType(typeof(ApiErrorResponse), StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> DeleteUser(string id)
        {
            try
            {
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

