using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace DogoFinance.DataAccess.Layer.Models.Entities
{
    [Table("TBL_PORTFOLIO_TYPE")]
    public partial class TblPortfolioType
    {
        [Key]
        public int PortfolioTypeId { get; set; }
        [StringLength(200)]
        public string Name { get; set; } = null!;
        [StringLength(100)]
        public string Code { get; set; } = null!;
        public bool SupportsAllocation { get; set; }
        public DateTime CreatedAt { get; set; }
    }
}
