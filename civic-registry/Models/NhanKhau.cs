using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace CivicRegistry.API.Models
{
    /// <summary>
    /// Bảng NhanKhau - Nhân khẩu
    /// </summary>
    [BsonCollection("NhanKhau")]
    public class NhanKhau
    {
        /// <summary>
        /// ID của nhân khẩu
        /// </summary>
        [BsonId]
        [BsonRepresentation(BsonType.ObjectId)]
        public string Id { get; set; } = ObjectId.GenerateNewId().ToString();

        /// <summary>
        /// Họ và tên
        /// </summary>
        [BsonElement("hoTen")]
        [BsonRequired]
        public string HoTen { get; set; } = string.Empty;

        /// <summary>
        /// Ngày sinh
        /// </summary>
        [BsonElement("ngaySinh")]
        public DateTime? NgaySinh { get; set; }

        /// <summary>
        /// Giới tính
        /// </summary>
        [BsonElement("gioiTinh")]
        public string? GioiTinh { get; set; }

        /// <summary>
        /// Số CCCD
        /// </summary>
        [BsonElement("cccd")]
        public string? CCCD { get; set; }

        /// <summary>
        /// Quan hệ với chủ hộ
        /// </summary>
        [BsonElement("quanHeChuHo")]
        public string? QuanHeChuHo { get; set; }

        /// <summary>
        /// Trạng thái
        /// </summary>
        [BsonElement("trangThai")]
        public string? TrangThai { get; set; }

        /// <summary>
        /// ID hộ khẩu (Foreign Key - MongoDB ObjectId)
        /// </summary>
        [BsonElement("hoKhauId")]
        public string? HoKhauId { get; set; }

        /// <summary>
        /// Ngày tạo
        /// </summary>
        [BsonElement("ngayTao")]
        public DateTime NgayTao { get; set; } = DateTime.UtcNow;
    }
}

