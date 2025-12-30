using System.ComponentModel.DataAnnotations;

namespace CivicRegistry.API.DTOs
{
    /// <summary>
    /// DTO cho yêu cầu refresh token
    /// </summary>
    public class RefreshTokenRequest
    {
        /// <summary>
        /// JWT token hiện tại
        /// </summary>
        [Required(ErrorMessage = "Token là bắt buộc")]
        public string Token { get; set; } = string.Empty;
    }
}

