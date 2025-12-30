using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace CivicRegistry.API.Models
{
    /// <summary>
    /// Bảng Provinces - Tỉnh
    /// </summary>
    [BsonCollection("Provinces")]
    public class Province
    {
        /// <summary>
        /// ID của tỉnh
        /// </summary>
        [BsonId]
        [BsonRepresentation(BsonType.ObjectId)]
        public string Id { get; set; } = ObjectId.GenerateNewId().ToString();

        /// <summary>
        /// Tên tỉnh
        /// </summary>
        [BsonElement("name")]
        [BsonRequired]
        public string Name { get; set; } = string.Empty;

        /// <summary>
        /// Mã tỉnh
        /// </summary>
        [BsonElement("code")]
        public string? Code { get; set; }
    }
}

