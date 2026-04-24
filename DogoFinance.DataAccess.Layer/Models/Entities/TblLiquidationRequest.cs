using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace DogoFinance.DataAccess.Layer.Models.Entities
{
    [Table("TBL_LIQUIDATION_REQUEST")]
    public partial class TblLiquidationRequest
    {
        [Key]
        public long Id { get; set; }
        public long CustomerId { get; set; }
        public int PortfolioId { get; set; }
        
        [Column(TypeName = "decimal(18, 6)")]
        public decimal UnitsRequested { get; set; }
        
        [Column(TypeName = "decimal(18, 2)")]
        public decimal GrossAmount { get; set; }
        
        [Column(TypeName = "decimal(18, 2)")]
        public decimal ExitFeeApplied { get; set; }
        
        [Column(TypeName = "decimal(18, 2)")]
        public decimal NetPayableAmount { get; set; }
        
        public int Status { get; set; } // Uses LiquidationStatus constants
        
        public DateTime? ExpectedReleaseDate { get; set; }
        
        public string? AdminNotes { get; set; }
        public long? ReviewedByAdminId { get; set; }
        public DateTime? ReviewedAt { get; set; }
        
        public DateTime CreatedAt { get; set; }
        public DateTime? UpdatedAt { get; set; }

        [ForeignKey("CustomerId")]
        public virtual TblCustomer Customer { get; set; } = null!;
        
        [ForeignKey("PortfolioId")]
        public virtual TblPortfolio Portfolio { get; set; } = null!;
        
        [ForeignKey("ReviewedByAdminId")]
        public virtual TblUser? ReviewedByAdmin { get; set; }
    }
}
