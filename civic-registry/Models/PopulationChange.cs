using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace CivicRegistry.API.Models
{
    /// <summary>
    /// Bảng PopulationChanges - Biến động dân cư
    /// </summary>
    [BsonCollection("PopulationChanges")]
    public class PopulationChange
    {
        /// <summary>
        /// ID của biến động
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
        /// Loại biến động: Birth, Death, Marriage, Divorce, MoveIn, MoveOut, TempStay, TempLeave
        /// </summary>
        [BsonElement("changeType")]
        [BsonRequired]
        public int ChangeType { get; set; }

        /// <summary>
        /// ID hộ khẩu nguồn
        /// </summary>
        [BsonElement("fromHouseholdId")]
        public string? FromHouseholdId { get; set; }

        /// <summary>
        /// ID hộ khẩu đích
        /// </summary>
        [BsonElement("toHouseholdId")]
        public string? ToHouseholdId { get; set; }

        /// <summary>
        /// Ngày biến động
        /// </summary>
        [BsonElement("changeDate")]
        public DateTime ChangeDate { get; set; } = DateTime.UtcNow;

        /// <summary>
        /// Lý do
        /// </summary>
        [BsonElement("reason")]
        public string? Reason { get; set; }

        /// <summary>
        /// Người tạo
        /// </summary>
        [BsonElement("createdBy")]
        [BsonRequired]
        public string CreatedBy { get; set; } = string.Empty;

        /// <summary>
        /// Người phê duyệt
        /// </summary>
        [BsonElement("approvedBy")]
        public string? ApprovedBy { get; set; }

        /// <summary>
        /// Trạng thái: Pending, Approved, Rejected
        /// </summary>
        [BsonElement("status")]
        public int Status { get; set; } = 0; // 0 = Pending
    }
}

