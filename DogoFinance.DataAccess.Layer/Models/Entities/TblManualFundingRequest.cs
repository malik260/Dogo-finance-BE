using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace DogoFinance.DataAccess.Layer.Models.Entities
{
    [Table("TBL_MANUAL_FUNDING_REQUEST")]
    public partial class TblManualFundingRequest
    {
        [Key]
        public long Id { get; set; }
        public long CustomerId { get; set; }
        
        [Column(TypeName = "decimal(18, 2)")]
        public decimal Amount { get; set; }
        
        [StringLength(50)]
        public string Status { get; set; } = "Pending"; // Pending, Approved, Rejected
        
        [StringLength(100)]
        public string Reference { get; set; } = null!;
        
        [StringLength(250)]
        public string? ReceiptPath { get; set; }
        
        public DateTime InitiatedAt { get; set; } = DateTime.UtcNow;
        public DateTime? ReviewedAt { get; set; }
        public long? ReviewedByAdminId { get; set; }
        
        [StringLength(500)]
        public string? AdminNotes { get; set; }

        [ForeignKey("CustomerId")]
        public virtual TblCustomer Customer { get; set; } = null!;
        
        [ForeignKey("ReviewedByAdminId")]
        public virtual TblUser? Admin { get; set; }
    }
}
