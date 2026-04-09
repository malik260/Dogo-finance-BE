using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace DogoFinance.DataAccess.Layer.Models.Entities
{
    [Table("TBL_PORTFOLIO_ALLOCATION_RULE")]
    public partial class TblPortfolioAllocationRule
    {
        [Key]
        public int Id { get; set; }
        public int PortfolioId { get; set; }
        public int AssetClassId { get; set; }
        [Column(TypeName = "decimal(18, 4)")]
        public decimal TargetPercentage { get; set; }
        [Column(TypeName = "decimal(18, 4)")]
        public decimal MinPercentage { get; set; }
        [Column(TypeName = "decimal(18, 4)")]
        public decimal MaxPercentage { get; set; }
        [Column(TypeName = "decimal(18, 4)")]
        public decimal ExpectedReturn { get; set; }

        [ForeignKey("PortfolioId")]
        public virtual TblPortfolio Portfolio { get; set; } = null!;
        [ForeignKey("AssetClassId")]
        public virtual TblAssetClass AssetClass { get; set; } = null!;
    }
}
