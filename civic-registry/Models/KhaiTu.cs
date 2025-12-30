using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace CivicRegistry.API.Models
{
    /// <summary>
    /// Bảng KhaiTu - Khai tử
    /// </summary>
    [BsonCollection("KhaiTu")]
    public class KhaiTu
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
        /// Ngày mất
        /// </summary>
        [BsonElement("ngayMat")]
        public DateTime? NgayMat { get; set; }

        /// <summary>
        /// Lý do
        /// </summary>
        [BsonElement("lyDo")]
        public string? LyDo { get; set; }
    }
}

