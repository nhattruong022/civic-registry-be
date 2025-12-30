using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace CivicRegistry.API.Models
{
    /// <summary>
    /// Bảng Districts - Huyện
    /// </summary>
    [BsonCollection("Districts")]
    public class District
    {
        /// <summary>
        /// ID của huyện
        /// </summary>
        [BsonId]
        [BsonRepresentation(BsonType.ObjectId)]
        public string Id { get; set; } = ObjectId.GenerateNewId().ToString();

        /// <summary>
        /// Tên huyện
        /// </summary>
        [BsonElement("name")]
        [BsonRequired]
        public string Name { get; set; } = string.Empty;

        /// <summary>
        /// ID tỉnh
        /// </summary>
        [BsonElement("provinceId")]
        [BsonRequired]
        public string ProvinceId { get; set; } = string.Empty;
    }
}

