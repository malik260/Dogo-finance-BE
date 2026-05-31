using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace DogoFinance.DataAccess.Layer.Models.Entities
{
    [Table("TBL_CORPORATE_DOCUMENT")]
    public class TblCorporateDocument
    {
        [Key]
        public long DocumentId { get; set; }

        public long CustomerId { get; set; }

        [Required]
        [StringLength(100)]
        public string DocumentType { get; set; } = null!;

        [Required]
        [StringLength(500)]
        public string FilePath { get; set; } = null!;

        [Required]
        [StringLength(50)]
        public string Status { get; set; } = "Pending"; // Pending, Verified, Rejected

        public DateTime UploadedAt { get; set; } = DateTime.UtcNow;

        public DateTime? ReviewedAt { get; set; }

        public long? ReviewedByAdminId { get; set; }

        [StringLength(1000)]
        public string? AdminNotes { get; set; }

        [ForeignKey("CustomerId")]
        public virtual TblCustomer Customer { get; set; } = null!;

        [ForeignKey("ReviewedByAdminId")]
        public virtual TblUser? ReviewedByAdmin { get; set; }
    }
}
