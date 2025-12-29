using System;
using System.ComponentModel.DataAnnotations;

namespace CivicRegistry.API.Models
{
    /// <summary>
    /// Base model cho tất cả các entities
    /// </summary>
    public abstract class BaseModel
    {
        /// <summary>
        /// ID của entity
        /// </summary>
        [Key]
        public int Id { get; set; }

        /// <summary>
        /// Ngày tạo
        /// </summary>
        public DateTime CreatedAt { get; set; }

        /// <summary>
        /// Ngày cập nhật
        /// </summary>
        public DateTime? UpdatedAt { get; set; }

        /// <summary>
        /// Người tạo
        /// </summary>
        [StringLength(100)]
        public string? CreatedBy { get; set; }

        /// <summary>
        /// Người cập nhật
        /// </summary>
        [StringLength(100)]
        public string? UpdatedBy { get; set; }

        /// <summary>
        /// Đánh dấu đã xóa (soft delete)
        /// </summary>
        public bool IsDeleted { get; set; }

        public BaseModel()
        {
            CreatedAt = DateTime.UtcNow;
            IsDeleted = false;
        }
    }
}

