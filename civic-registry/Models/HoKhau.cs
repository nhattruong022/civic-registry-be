using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace CivicRegistry.API.Models
{
    /// <summary>
    /// Bảng HoKhau - Hộ khẩu
    /// </summary>
    [BsonCollection("HoKhau")]
    public class HoKhau
    {
        /// <summary>
        /// ID của hộ khẩu
        /// </summary>
        [BsonId]
        [BsonRepresentation(BsonType.ObjectId)]
        public string Id { get; set; } = ObjectId.GenerateNewId().ToString();

        /// <summary>
        /// Mã hộ khẩu
        /// </summary>
        [BsonElement("maHoKhau")]
        [BsonRequired]
        public string MaHoKhau { get; set; } = string.Empty;

        /// <summary>
        /// Địa chỉ
        /// </summary>
        [BsonElement("diaChi")]
        public string? DiaChi { get; set; }

        /// <summary>
        /// Khu vực
        /// </summary>
        [BsonElement("khuVuc")]
        public string? KhuVuc { get; set; }

        /// <summary>
        /// Ngày lập
        /// </summary>
        [BsonElement("ngayLap")]
        public DateTime? NgayLap { get; set; }

        /// <summary>
        /// Ghi chú
        /// </summary>
        [BsonElement("ghiChu")]
        public string? GhiChu { get; set; }
    }
}

