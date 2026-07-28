using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace DogoFinance.DataAccess.Layer.Models.Entities
{
    [Table("TBL_PORTFOLIO_FEE_CONFIG")]
    public partial class TblPortfolioFeeConfig
    {
        [Key]
        public int Id { get; set; }

        public int PortfolioId { get; set; }

        [Required]
        [StringLength(50)]
        public string FeeType { get; set; } = null!; // MANAGEMENT, CUSTODY, SEC_REGULATORY, PERFORMANCE, EXIT

        [Column(TypeName = "decimal(18, 4)")]
        public decimal PercentagePerAnnum { get; set; } // e.g. 1.50 for 1.5%

        [Required]
        [StringLength(50)]
        public string CalculationBasis { get; set; } = "AVERAGE_MONTH_END_NAV"; // AVERAGE_MONTH_END_NAV, END_OF_PERIOD_NAV, INVESTED_CAPITAL

        [Required]
        [StringLength(30)]
        public string BillingFrequency { get; set; } = "QUARTERLY"; // QUARTERLY, MONTHLY, ANNUALLY

        public int ChargeDayOfMonth { get; set; } = 10;

        [Required]
        [StringLength(20)]
        public string TargetAccountCode { get; set; } = null!; // e.g. "4220", "2210"

        public bool IsLiability { get; set; } = false; // True for SEC Fee (Liability), False for Mgmt Fee (Revenue)

        public bool IsWaived { get; set; } = false;

        public bool IsActive { get; set; } = true;

        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

        [ForeignKey(nameof(PortfolioId))]
        public virtual TblPortfolio? Portfolio { get; set; }
    }
}
