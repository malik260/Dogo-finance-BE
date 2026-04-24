using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace DogoFinance.DataAccess.Layer.Models.Entities
{
    [Table("TBL_PORTFOLIO")]
    public partial class TblPortfolio
    {
        [Key]
        public int PortfolioId { get; set; }
        [StringLength(300)]
        public string Name { get; set; } = null!;
        [StringLength(100)]
        public string Code { get; set; } = null!;
        public int PortfolioTypeId { get; set; }
        [StringLength(100)]
        public string? RiskLevel { get; set; }
        public string? Description { get; set; }
        [Column(TypeName = "decimal(18, 4)")]
        public decimal? ExpectedAnnualReturn { get; set; }
        public bool IsActive { get; set; }
        
        public int LockInPeriodDays { get; set; }
        public int MinHoldingPeriodDays { get; set; }
        [Column(TypeName = "decimal(18, 4)")]
        public decimal ExitFeePercentage { get; set; }
        public int NoticePeriodDays { get; set; }
        [Column(TypeName = "decimal(18, 2)")]
        public decimal ApprovalThresholdAmount { get; set; }

        public DateTime CreatedAt { get; set; }

        [ForeignKey("PortfolioTypeId")]
        public virtual TblPortfolioType PortfolioType { get; set; } = null!;
    }
}
