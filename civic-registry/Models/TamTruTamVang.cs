using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace CivicRegistry.API.Models
{
    /// <summary>
    /// Bảng TamTruTamVang - Tạm trú tạm vắng
    /// </summary>
    [BsonCollection("TamTruTamVang")]
    public class TamTruTamVang
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
        /// Loại: Tạm trú / Tạm vắng
        /// </summary>
        [BsonElement("loai")]
        [BsonRequired]
        public string Loai { get; set; } = string.Empty;

        /// <summary>
        /// Từ ngày
        /// </summary>
        [BsonElement("tuNgay")]
        public DateTime? TuNgay { get; set; }

        /// <summary>
        /// Đến ngày
        /// </summary>
        [BsonElement("denNgay")]
        public DateTime? DenNgay { get; set; }

        /// <summary>
        /// Lý do
        /// </summary>
        [BsonElement("lyDo")]
        public string? LyDo { get; set; }
    }
}

