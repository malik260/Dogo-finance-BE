using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace DogoFinance.DataAccess.Layer.Models.Entities
{
    [Table("TBL_CUSTOMER_PORTFOLIO")]
    public partial class TblCustomerPortfolio
    {
        [Key]
        public long Id { get; set; }
        public long CustomerId { get; set; }
        public int PortfolioId { get; set; }
        [Column(TypeName = "decimal(18, 4)")]
        public decimal TotalInvested { get; set; }
        public DateTime CreatedAt { get; set; }

        [ForeignKey("CustomerId")]
        public virtual TblCustomer Customer { get; set; } = null!;
        [ForeignKey("PortfolioId")]
        public virtual TblPortfolio Portfolio { get; set; } = null!;
    }
}
