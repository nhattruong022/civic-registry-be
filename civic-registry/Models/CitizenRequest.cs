using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace CivicRegistry.API.Models
{
    /// <summary>
    /// Bảng CitizenRequests - Yêu cầu người dân
    /// </summary>
    [BsonCollection("CitizenRequests")]
    public class CitizenRequest
    {
        /// <summary>
        /// ID của yêu cầu
        /// </summary>
        [BsonId]
        [BsonRepresentation(BsonType.ObjectId)]
        public string Id { get; set; } = ObjectId.GenerateNewId().ToString();

        /// <summary>
        /// ID công dân
        /// </summary>
        [BsonElement("citizenId")]
        [BsonRequired]
        public string CitizenId { get; set; } = string.Empty;

        /// <summary>
        /// Loại yêu cầu
        /// </summary>
        [BsonElement("requestType")]
        [BsonRequired]
        public int RequestType { get; set; }

        /// <summary>
        /// Nội dung yêu cầu
        /// </summary>
        [BsonElement("content")]
        public string? Content { get; set; }

        /// <summary>
        /// Trạng thái: Pending, Approved, Rejected
        /// </summary>
        [BsonElement("status")]
        public int Status { get; set; } = 0; // 0 = Pending

        /// <summary>
        /// Ngày tạo
        /// </summary>
        [BsonElement("createdAt")]
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

        /// <summary>
        /// Người xử lý
        /// </summary>
        [BsonElement("processedBy")]
        public string? ProcessedBy { get; set; }

        /// <summary>
        /// Ngày xử lý
        /// </summary>
        [BsonElement("processedAt")]
        public DateTime? ProcessedAt { get; set; }
    }
}

