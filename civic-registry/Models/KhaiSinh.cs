using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace CivicRegistry.API.Models
{
    /// <summary>
    /// Bảng KhaiSinh - Khai sinh
    /// </summary>
    [BsonCollection("KhaiSinh")]
    public class KhaiSinh
    {
        /// <summary>
        /// ID của bản ghi
        /// </summary>
        [BsonId]
        [BsonRepresentation(BsonType.ObjectId)]
        public string Id { get; set; } = ObjectId.GenerateNewId().ToString();

        /// <summary>
        /// ID nhân khẩu (Foreign Key - MongoDB ObjectId)
        /// </summary>
        [BsonElement("nhanKhauId")]
        [BsonRequired]
        public string NhanKhauId { get; set; } = string.Empty;

        /// <summary>
        /// Nơi sinh
        /// </summary>
        [BsonElement("noiSinh")]
        public string? NoiSinh { get; set; }

        /// <summary>
        /// Ngày đăng ký
        /// </summary>
        [BsonElement("ngayDangKy")]
        public DateTime? NgayDangKy { get; set; }
    }
}

