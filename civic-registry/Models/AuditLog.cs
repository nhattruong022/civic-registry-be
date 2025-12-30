using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace CivicRegistry.API.Models
{
    /// <summary>
    /// Bảng AuditLogs - Nhật ký hệ thống
    /// </summary>
    [BsonCollection("AuditLogs")]
    public class AuditLog
    {
        /// <summary>
        /// ID của log
        /// </summary>
        [BsonId]
        [BsonRepresentation(BsonType.ObjectId)]
        public string Id { get; set; } = ObjectId.GenerateNewId().ToString();

        /// <summary>
        /// ID user thực hiện
        /// </summary>
        [BsonElement("userId")]
        [BsonRequired]
        public string UserId { get; set; } = string.Empty;

        /// <summary>
        /// Hành động
        /// </summary>
        [BsonElement("action")]
        [BsonRequired]
        public string Action { get; set; } = string.Empty;

        /// <summary>
        /// Tên bảng
        /// </summary>
        [BsonElement("tableName")]
        [BsonRequired]
        public string TableName { get; set; } = string.Empty;

        /// <summary>
        /// Giá trị cũ (JSON)
        /// </summary>
        [BsonElement("oldValue")]
        public string? OldValue { get; set; }

        /// <summary>
        /// Giá trị mới (JSON)
        /// </summary>
        [BsonElement("newValue")]
        public string? NewValue { get; set; }

        /// <summary>
        /// Ngày tạo
        /// </summary>
        [BsonElement("createdAt")]
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    }
}

