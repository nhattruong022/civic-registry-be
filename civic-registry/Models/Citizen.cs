using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace CivicRegistry.API.Models
{
    /// <summary>
    /// Bảng Citizens - Nhân khẩu
    /// </summary>
    [BsonCollection("Citizens")]
    public class Citizen
    {
        /// <summary>
        /// ID của nhân khẩu
        /// </summary>
        [BsonId]
        [BsonRepresentation(BsonType.ObjectId)]
        public string Id { get; set; } = ObjectId.GenerateNewId().ToString();

        /// <summary>
        /// Mã công dân
        /// </summary>
        [BsonElement("citizenCode")]
        public string? CitizenCode { get; set; }

        /// <summary>
        /// Họ và tên đầy đủ
        /// </summary>
        [BsonElement("fullName")]
        [BsonRequired]
        public string FullName { get; set; } = string.Empty;

        /// <summary>
        /// Ngày sinh
        /// </summary>
        [BsonElement("dateOfBirth")]
        public DateTime? DateOfBirth { get; set; }

        /// <summary>
        /// Giới tính: 0=Nam, 1=Nữ, 2=Khác
        /// </summary>
        [BsonElement("gender")]
        public int Gender { get; set; }

        /// <summary>
        /// Số CCCD
        /// </summary>
        [BsonElement("cccd")]
        public string? CCCD { get; set; }

        /// <summary>
        /// Dân tộc
        /// </summary>
        [BsonElement("ethnic")]
        public string? Ethnic { get; set; }

        /// <summary>
        /// Tôn giáo
        /// </summary>
        [BsonElement("religion")]
        public string? Religion { get; set; }

        /// <summary>
        /// Nghề nghiệp
        /// </summary>
        [BsonElement("job")]
        public string? Job { get; set; }

        /// <summary>
        /// Trình độ học vấn
        /// </summary>
        [BsonElement("education")]
        public string? Education { get; set; }

        /// <summary>
        /// Tình trạng hôn nhân: 0=Độc thân, 1=Đã kết hôn, 2=Ly dị, 3=Góa
        /// </summary>
        [BsonElement("maritalStatus")]
        public int MaritalStatus { get; set; }

        /// <summary>
        /// ID hộ khẩu
        /// </summary>
        [BsonElement("householdId")]
        [BsonRequired]
        public string HouseholdId { get; set; } = string.Empty;

        /// <summary>
        /// Trạng thái: Living, Transferred, Dead
        /// </summary>
        [BsonElement("status")]
        public string Status { get; set; } = "Living";
    }
}

