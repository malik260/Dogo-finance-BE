using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace DogoFinance.DataAccess.Layer.Models.Entities
{
    [Table("TBL_QUARTERLY_FEE_LOG")]
    public partial class TblQuarterlyFeeLog
    {
        [Key]
        public long Id { get; set; }

        public long CustomerId { get; set; }

        public int PortfolioId { get; set; }

        public int FeeConfigId { get; set; }

        public int Year { get; set; }

        public int Quarter { get; set; } // 1, 2, 3, 4

        [Column(TypeName = "decimal(18, 4)")]
        public decimal Month1EndNav { get; set; }

        [Column(TypeName = "decimal(18, 4)")]
        public decimal Month2EndNav { get; set; }

        [Column(TypeName = "decimal(18, 4)")]
        public decimal Month3EndNav { get; set; }

        [Column(TypeName = "decimal(18, 4)")]
        public decimal AverageNav { get; set; }

        [Required]
        [StringLength(50)]
        public string FeeType { get; set; } = null!;

        [Column(TypeName = "decimal(18, 4)")]
        public decimal FeeRateApplied { get; set; }

        [Column(TypeName = "decimal(18, 2)")]
        public decimal CalculatedFeeAmount { get; set; }

        [Required]
        [StringLength(30)]
        public string Status { get; set; } = "PENDING"; // PENDING, DEDUCTED, FAILED, WAIVED, SKIPPED_ALREADY_RUN

        [StringLength(100)]
        public string? JournalReference { get; set; }

        public DateTime ProcessedAt { get; set; } = DateTime.UtcNow;

        [ForeignKey(nameof(CustomerId))]
        public virtual TblCustomer? Customer { get; set; }

        [ForeignKey(nameof(PortfolioId))]
        public virtual TblPortfolio? Portfolio { get; set; }

        [ForeignKey(nameof(FeeConfigId))]
        public virtual TblPortfolioFeeConfig? FeeConfig { get; set; }
    }
}
