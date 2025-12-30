using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace CivicRegistry.API.Models
{
    /// <summary>
    /// Bảng Users - Tài khoản hệ thống
    /// </summary>
    [BsonCollection("Users")]
    public class User
    {
        /// <summary>
        /// ID của user
        /// </summary>
        [BsonId]
        [BsonRepresentation(BsonType.ObjectId)]
        public string Id { get; set; } = ObjectId.GenerateNewId().ToString();

        /// <summary>
        /// Tên đăng nhập (unique)
        /// </summary>
        [BsonElement("username")]
        [BsonRequired]
        public string Username { get; set; } = string.Empty;

        /// <summary>
        /// Mật khẩu đã hash
        /// </summary>
        [BsonElement("passwordHash")]
        [BsonRequired]
        public string PasswordHash { get; set; } = string.Empty;

        /// <summary>
        /// Họ và tên đầy đủ
        /// </summary>
        [BsonElement("fullName")]
        public string? FullName { get; set; }

        /// <summary>
        /// Vai trò: Admin / HoTich / ThongKe
        /// </summary>
        [BsonElement("role")]
        [BsonRequired]
        public string Role { get; set; } = string.Empty;

        /// <summary>
        /// Trạng thái hoạt động
        /// </summary>
        [BsonElement("isActive")]
        public bool IsActive { get; set; } = true;
    }
}

