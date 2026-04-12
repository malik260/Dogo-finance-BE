using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace DogoFinance.DataAccess.Layer.Models.Entities
{
    [Table("TBL_INVESTMENT_TRANSACTION")]
    public partial class TblInvestmentTransaction
    {
        [Key]
        public long Id { get; set; }
        public long CustomerId { get; set; }
        public int PortfolioId { get; set; }
        public int InstrumentId { get; set; }
        
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
        public virtual TblCustomer Customer { get; set; } = null!;
        
        [ForeignKey("PortfolioId")]
        public virtual TblPortfolio Portfolio { get; set; } = null!;
        
        [ForeignKey("InstrumentId")]
        public virtual TblInstrument Instrument { get; set; } = null!;
    }
}
