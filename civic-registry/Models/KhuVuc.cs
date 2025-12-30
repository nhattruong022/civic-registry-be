using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace CivicRegistry.API.Models
{
    /// <summary>
    /// Bảng KhuVuc - Địa bàn quản lý
    /// </summary>
    [BsonCollection("KhuVuc")]
    public class KhuVuc
    {
        /// <summary>
        /// ID của khu vực
        /// </summary>
        [BsonId]
        [BsonRepresentation(BsonType.ObjectId)]
        public string Id { get; set; } = ObjectId.GenerateNewId().ToString();

        /// <summary>
        /// Tên khu vực
        /// </summary>
        [BsonElement("tenKhuVuc")]
        [BsonRequired]
        public string TenKhuVuc { get; set; } = string.Empty;
    }
}

