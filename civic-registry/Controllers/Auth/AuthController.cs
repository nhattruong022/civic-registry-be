using Microsoft.AspNetCore.Mvc;
using CivicRegistry.API.DTOs;
using CivicRegistry.API.Services;
using CivicRegistry.API.Types;

namespace CivicRegistry.API.Controllers
{
    /// <summary>
    /// Controller xử lý authentication
    /// </summary>
    [ApiController]
    [Route("api/[controller]")]
    public class AuthController : ControllerBase
    {
        private readonly AuthService _authService;
        private readonly ILogger<AuthController> _logger;

        public AuthController(AuthService authService, ILogger<AuthController> logger)
        {
            _authService = authService;
            _logger = logger;
        }

        /// <summary>
        /// Đăng nhập
        /// </summary>
        /// <param name="request">Thông tin đăng nhập</param>
        /// <returns>Token và thông tin user</returns>
        [HttpPost("login")]
        [ProducesResponseType(typeof(ApiSuccessResponse<LoginResponse>), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ApiErrorResponse), StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(typeof(ApiErrorResponse), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(typeof(ApiErrorResponse), StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> Login([FromBody] LoginRequest request)
        {
            try
            {
                if (!ModelState.IsValid)
                {
                    var errorResponse = new ApiErrorResponse("Dữ liệu không hợp lệ", string.Join(", ", ModelState.Values.SelectMany(v => v.Errors).Select(e => e.ErrorMessage)), 400);
                    return BadRequest(errorResponse);
                }

                var response = await _authService.LoginAsync(request);

                if (response == null)
                {
                    var errorResponse = new ApiErrorResponse("Tên đăng nhập hoặc mật khẩu không đúng", null, 401);
                    return Unauthorized(errorResponse);
                }

                var successResponse = new ApiSuccessResponse<LoginResponse>(response, "Đăng nhập thành công", 200);
                return Ok(successResponse);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Lỗi khi đăng nhập");
                var errorResponse = new ApiErrorResponse("Đã xảy ra lỗi khi xử lý yêu cầu đăng nhập", ex.Message, 500);
                return StatusCode(500, errorResponse);
            }
        }

        /// <summary>
        /// Đăng ký tài khoản mới
        /// </summary>
        /// <param name="request">Thông tin đăng ký</param>
        /// <returns>Thông tin user đã tạo</returns>
        [HttpPost("register")]
        [ProducesResponseType(typeof(ApiSuccessResponse<RegisterResponse>), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ApiErrorResponse), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(typeof(ApiErrorResponse), StatusCodes.Status409Conflict)]
        [ProducesResponseType(typeof(ApiErrorResponse), StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> Register([FromBody] RegisterRequest request)
        {
            try
            {
                if (!ModelState.IsValid)
                {
                    var errorResponse = new ApiErrorResponse("Dữ liệu không hợp lệ", string.Join(", ", ModelState.Values.SelectMany(v => v.Errors).Select(e => e.ErrorMessage)), 400);
                    return BadRequest(errorResponse);
                }

                var response = await _authService.RegisterAsync(request);

                if (response == null)
                {
                    // Kiểm tra xem có phải do username đã tồn tại không
                    var existingUser = await _authService.CheckUsernameExistsAsync(request.Username);
                    if (existingUser)
                    {
                        var errorResponse = new ApiErrorResponse("Username đã tồn tại", null, 409);
                        return Conflict(errorResponse);
                    }
                    
                    // Nếu không phải do username, thì là do role không hợp lệ
                    var roleErrorResponse = new ApiErrorResponse("Role không hợp lệ. Role phải là một trong: SuperAdmin, ProvinceAdmin, DistrictAdmin, WardAdmin, Citizen", null, 400);
                    return BadRequest(roleErrorResponse);
                }

                var successResponse = new ApiSuccessResponse<RegisterResponse>(response, "Đăng ký thành công", 200);
                return Ok(successResponse);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Lỗi khi đăng ký");
                var errorResponse = new ApiErrorResponse("Đã xảy ra lỗi khi xử lý yêu cầu đăng ký", ex.Message, 500);
                return StatusCode(500, errorResponse);
            }
        }

        /// <summary>
        /// Refresh JWT token
        /// </summary>
        /// <remarks>
        /// Gửi token hiện tại trong Authorization header với format: Bearer {token}
        /// 
        /// **Cách sử dụng:**
        /// 1. Click nút "Authorize" (khóa) ở góc trên bên phải
        /// 2. Nhập token với format: Bearer {your-token} hoặc chỉ {your-token}
        /// 3. Click "Authorize" và "Close"
        /// 4. Gọi API refreshToken
        /// </remarks>
        /// <returns>Token mới và thông tin user</returns>
        [HttpPost("refreshToken")]
        [ProducesResponseType(typeof(ApiSuccessResponse<LoginResponse>), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ApiErrorResponse), StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(typeof(ApiErrorResponse), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(typeof(ApiErrorResponse), StatusCodes.Status500InternalServerError)]
        [Microsoft.AspNetCore.Authorization.AllowAnonymous]
        public async Task<IActionResult> RefreshToken()
        {
            try
            {
                // Lấy token từ Authorization header
                var authHeader = Request.Headers["Authorization"].FirstOrDefault();
                if (string.IsNullOrEmpty(authHeader))
                {
                    var errorResponse = new ApiErrorResponse("Token không được cung cấp. Vui lòng gửi token trong Authorization header", null, 400);
                    return BadRequest(errorResponse);
                }

                // Lấy token (hỗ trợ cả "Bearer {token}" và chỉ "{token}")
                string token;
                if (authHeader.StartsWith("Bearer ", StringComparison.OrdinalIgnoreCase))
                {
                    token = authHeader.Substring("Bearer ".Length).Trim();
                }
                else
                {
                    token = authHeader.Trim();
                }

                if (string.IsNullOrEmpty(token))
                {
                    var errorResponse = new ApiErrorResponse("Token không được cung cấp", null, 400);
                    return BadRequest(errorResponse);
                }

                var request = new RefreshTokenRequest { Token = token };
                var response = await _authService.RefreshTokenAsync(request);

                if (response == null)
                {
                    var errorResponse = new ApiErrorResponse("Token không hợp lệ hoặc đã hết hạn", null, 401);
                    return Unauthorized(errorResponse);
                }

                var successResponse = new ApiSuccessResponse<LoginResponse>(response, "Refresh token thành công", 200);
                return Ok(successResponse);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Lỗi khi refresh token");
                var errorResponse = new ApiErrorResponse("Đã xảy ra lỗi khi xử lý yêu cầu refresh token", ex.Message, 500);
                return StatusCode(500, errorResponse);
            }
        }

        /// <summary>
        /// Đăng xuất
        /// </summary>
        /// <remarks>
        /// Đăng xuất user. Token sẽ không còn hợp lệ sau khi logout (client nên xóa token).
        /// 
        /// **Cách sử dụng:**
        /// 1. Gửi token hiện tại trong Authorization header với format: Bearer {token}
        /// 2. Sau khi logout thành công, client nên xóa token khỏi storage
        /// </remarks>
        /// <returns>Thông báo logout thành công</returns>
        [HttpPost("logout")]
        [ProducesResponseType(typeof(ApiSuccessResponse<LogoutResponse>), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ApiErrorResponse), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(typeof(ApiErrorResponse), StatusCodes.Status500InternalServerError)]
        [Microsoft.AspNetCore.Authorization.AllowAnonymous]
        public async Task<IActionResult> Logout()
        {
            try
            {
                // Lấy token từ Authorization header (optional - có thể không cần token để logout)
                string? token = null;
                var authHeader = Request.Headers["Authorization"].FirstOrDefault();
                if (!string.IsNullOrEmpty(authHeader))
                {
                    if (authHeader.StartsWith("Bearer ", StringComparison.OrdinalIgnoreCase))
                    {
                        token = authHeader.Substring("Bearer ".Length).Trim();
                    }
                    else
                    {
                        token = authHeader.Trim();
                    }
                }

                var response = await _authService.LogoutAsync(token);

                var successResponse = new ApiSuccessResponse<LogoutResponse>(response, "Đăng xuất thành công", 200);
                return Ok(successResponse);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Lỗi khi đăng xuất");
                var errorResponse = new ApiErrorResponse("Đã xảy ra lỗi khi xử lý yêu cầu đăng xuất", ex.Message, 500);
                return StatusCode(500, errorResponse);
            }
        }

        /// <summary>
        /// Kiểm tra token và lấy thông tin user
        /// </summary>
        /// <remarks>
        /// Kiểm tra token từ Authorization header và trả về thông tin user hiện tại.
        /// 
        /// **Cách sử dụng:**
        /// 1. Click nút "Authorize" (khóa) ở góc trên bên phải
        /// 2. Nhập token với format: Bearer {your-token} hoặc chỉ {your-token}
        /// 3. Click "Authorize" và "Close"
        /// 4. Gọi API checkToken
        /// </remarks>
        /// <returns>Thông tin user từ token</returns>
        [HttpPost("checkToken")]
        [ProducesResponseType(typeof(ApiSuccessResponse<UserInfo>), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ApiErrorResponse), StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(typeof(ApiErrorResponse), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(typeof(ApiErrorResponse), StatusCodes.Status500InternalServerError)]
        [Microsoft.AspNetCore.Authorization.AllowAnonymous]
        public async Task<IActionResult> CheckToken()
        {
            try
            {
                // Lấy token từ Authorization header
                var authHeader = Request.Headers["Authorization"].FirstOrDefault();
                if (string.IsNullOrEmpty(authHeader))
                {
                    var errorResponse = new ApiErrorResponse("Token không được cung cấp. Vui lòng gửi token trong Authorization header", null, 400);
                    return BadRequest(errorResponse);
                }

                // Lấy token (hỗ trợ cả "Bearer {token}" và chỉ "{token}")
                string token;
                if (authHeader.StartsWith("Bearer ", StringComparison.OrdinalIgnoreCase))
                {
                    token = authHeader.Substring("Bearer ".Length).Trim();
                }
                else
                {
                    token = authHeader.Trim();
                }

                if (string.IsNullOrEmpty(token))
                {
                    var errorResponse = new ApiErrorResponse("Token không được cung cấp", null, 400);
                    return BadRequest(errorResponse);
                }

                var userInfo = await _authService.CheckTokenAsync(token);

                if (userInfo == null)
                {
                    var errorResponse = new ApiErrorResponse("Token không hợp lệ, đã hết hạn hoặc user không tồn tại", null, 401);
                    return Unauthorized(errorResponse);
                }

                var successResponse = new ApiSuccessResponse<UserInfo>(userInfo, "Token hợp lệ", 200);
                return Ok(successResponse);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Lỗi khi kiểm tra token");
                var errorResponse = new ApiErrorResponse("Đã xảy ra lỗi khi xử lý yêu cầu kiểm tra token", ex.Message, 500);
                return StatusCode(500, errorResponse);
            }
        }
    }
}

