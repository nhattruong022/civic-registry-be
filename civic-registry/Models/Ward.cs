using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace CivicRegistry.API.Models
{
    /// <summary>
    /// Bảng Wards - Xã
    /// </summary>
    [BsonCollection("Wards")]
    public class Ward
    {
        /// <summary>
        /// ID của xã
        /// </summary>
        [BsonId]
        [BsonRepresentation(BsonType.ObjectId)]
        public string Id { get; set; } = ObjectId.GenerateNewId().ToString();

        /// <summary>
        /// Tên xã
        /// </summary>
        [BsonElement("name")]
        [BsonRequired]
        public string Name { get; set; } = string.Empty;

        /// <summary>
        /// ID huyện
        /// </summary>
        [BsonElement("districtId")]
        [BsonRequired]
        public string DistrictId { get; set; } = string.Empty;
    }
}

