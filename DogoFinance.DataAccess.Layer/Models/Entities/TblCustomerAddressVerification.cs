using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace DogoFinance.DataAccess.Layer.Models.Entities
{
    [Table("TBL_CUSTOMER_ADDRESS_VERIFICATION")]
    public partial class TblCustomerAddressVerification
    {
        [Key]
        public long Id { get; set; }

        public long CustomerId { get; set; }

        public int DocTypeId { get; set; }

        [StringLength(500)]
        public string? DocumentUrl { get; set; }

        [StringLength(200)]
        public string? CloudinaryPublicId { get; set; }

        [StringLength(250)]
        public string? ExtractedAddress { get; set; }

        [StringLength(100)]
        public string? ExtractedCity { get; set; }

        [StringLength(100)]
        public string? ExtractedState { get; set; }

        [Column(TypeName = "nvarchar(MAX)")]
        public string? ExtractedFullText { get; set; }

        public decimal? ConfidenceScore { get; set; }

        [StringLength(20)]
        public string? Status { get; set; } = "Pending"; // Pending, Review, Approved, Rejected

        [Column(TypeName = "nvarchar(MAX)")]
        public string? AdminNotes { get; set; }

        public DateTime? CreatedAt { get; set; }

        public DateTime? ReviewedAt { get; set; }

        public long? ReviewedBy { get; set; }

        [ForeignKey(nameof(CustomerId))]
        [InverseProperty(nameof(TblCustomer.TblCustomerAddressVerifications))]
        public virtual TblCustomer Customer { get; set; } = null!;

        [ForeignKey(nameof(DocTypeId))]
        [InverseProperty(nameof(TblAddressDocType.TblCustomerAddressVerifications))]
        public virtual TblAddressDocType DocType { get; set; } = null!;
    }
}
