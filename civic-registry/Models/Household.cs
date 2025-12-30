using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace CivicRegistry.API.Models
{
    /// <summary>
    /// Bảng Households - Hộ khẩu
    /// </summary>
    [BsonCollection("Households")]
    public class Household
    {
        /// <summary>
        /// ID của hộ khẩu
        /// </summary>
        [BsonId]
        [BsonRepresentation(BsonType.ObjectId)]
        public string Id { get; set; } = ObjectId.GenerateNewId().ToString();

        /// <summary>
        /// Mã hộ khẩu
        /// </summary>
        [BsonElement("householdCode")]
        [BsonRequired]
        public string HouseholdCode { get; set; } = string.Empty;

        /// <summary>
        /// Địa chỉ
        /// </summary>
        [BsonElement("address")]
        public string? Address { get; set; }

        /// <summary>
        /// ID thôn
        /// </summary>
        [BsonElement("hamletId")]
        [BsonRequired]
        public string HamletId { get; set; } = string.Empty;

        /// <summary>
        /// ID chủ hộ (Citizen)
        /// </summary>
        [BsonElement("headCitizenId")]
        [BsonRequired]
        public string HeadCitizenId { get; set; } = string.Empty;

        /// <summary>
        /// Ngày tạo
        /// </summary>
        [BsonElement("createdDate")]
        public DateTime CreatedDate { get; set; } = DateTime.UtcNow;

        /// <summary>
        /// Trạng thái: 0=Active, 1=Transferred, 2=Closed
        /// </summary>
        [BsonElement("status")]
        public int Status { get; set; } = 0; // 0 = Active
    }
}

