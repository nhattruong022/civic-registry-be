using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace CivicRegistry.API.Models
{
    /// <summary>
    /// Bảng LichSuThayDoi - Nhật ký hệ thống
    /// </summary>
    [BsonCollection("LichSuThayDoi")]
    public class LichSuThayDoi
    {
        /// <summary>
        /// ID của bản ghi
        /// </summary>
        [BsonId]
        [BsonRepresentation(BsonType.ObjectId)]
        public string Id { get; set; } = ObjectId.GenerateNewId().ToString();

        /// <summary>
        /// ID user thực hiện thay đổi (Foreign Key - MongoDB ObjectId)
        /// </summary>
        [BsonElement("userId")]
        public string? UserId { get; set; }

        /// <summary>
        /// Tên bảng bị thay đổi
        /// </summary>
        [BsonElement("bang")]
        public string? Bang { get; set; }

        /// <summary>
        /// Hành động: Create / Update / Delete
        /// </summary>
        [BsonElement("hanhDong")]
        public string? HanhDong { get; set; }

        /// <summary>
        /// Thời gian thay đổi
        /// </summary>
        [BsonElement("thoiGian")]
        public DateTime ThoiGian { get; set; } = DateTime.UtcNow;

        /// <summary>
        /// Nội dung thay đổi
        /// </summary>
        [BsonElement("noiDung")]
        public string? NoiDung { get; set; }
    }
}

