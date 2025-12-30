using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace CivicRegistry.API.Models
{
    /// <summary>
    /// Bảng Hamlets - Thôn
    /// </summary>
    [BsonCollection("Hamlets")]
    public class Hamlet
    {
        /// <summary>
        /// ID của thôn
        /// </summary>
        [BsonId]
        [BsonRepresentation(BsonType.ObjectId)]
        public string Id { get; set; } = ObjectId.GenerateNewId().ToString();

        /// <summary>
        /// Tên thôn
        /// </summary>
        [BsonElement("name")]
        [BsonRequired]
        public string Name { get; set; } = string.Empty;

        /// <summary>
        /// ID xã
        /// </summary>
        [BsonElement("wardId")]
        [BsonRequired]
        public string WardId { get; set; } = string.Empty;
    }
}

