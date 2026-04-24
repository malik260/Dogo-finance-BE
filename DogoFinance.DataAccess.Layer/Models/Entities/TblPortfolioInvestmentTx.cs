using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace DogoFinance.DataAccess.Layer.Models.Entities
{
    [Table("TBL_PORTFOLIO_INVESTMENT_TX")]
    public partial class TblPortfolioInvestmentTx
    {
        [Key]
        public long Id { get; set; }
        public long CustomerId { get; set; }
        public int PortfolioId { get; set; }
        
        [Column(TypeName = "decimal(18, 2)")]
        public decimal Amount { get; set; }
        
        [Column(TypeName = "decimal(18, 6)")]
        public decimal Units { get; set; }
        
        [Column(TypeName = "decimal(18, 6)")]
        public decimal NAV { get; set; }
        
        [StringLength(20)]
        public string TransactionType { get; set; } = null!;
        
        public DateTime CreatedAt { get; set; }

        [ForeignKey("CustomerId")]
        public virtual TblCustomer? Customer { get; set; }
        
        [ForeignKey("PortfolioId")]
        public virtual TblPortfolio? Portfolio { get; set; }
    }
}
