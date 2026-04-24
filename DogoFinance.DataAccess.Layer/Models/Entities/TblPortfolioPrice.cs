using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace DogoFinance.DataAccess.Layer.Models.Entities
{
    [Table("TBL_PORTFOLIO_PRICE")]
    public partial class TblPortfolioPrice
    {
        [Key]
        public long Id { get; set; }

        public int PortfolioId { get; set; }

        public DateTime PriceDate { get; set; }

        [Column(TypeName = "decimal(18, 6)")]
        public decimal NAV { get; set; }

        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

        [ForeignKey(nameof(PortfolioId))]
        public virtual TblPortfolio Portfolio { get; set; } = null!;
    }
}
