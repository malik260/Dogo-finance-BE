using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace DogoFinance.DataAccess.Layer.Models.Entities
{
    [Table("TBL_PORTFOLIO_INSTRUMENT")]
    public partial class TblPortfolioInstrument
    {
        [Key]
        public int Id { get; set; }
        public int PortfolioId { get; set; }
        public int InstrumentId { get; set; }
        public int AssetClassId { get; set; }
        [Column(TypeName = "decimal(5, 2)")]
        public decimal TargetWeight { get; set; }

        [ForeignKey("PortfolioId")]
        public virtual TblPortfolio Portfolio { get; set; } = null!;
        [ForeignKey("InstrumentId")]
        public virtual TblInstrument Instrument { get; set; } = null!;
        [ForeignKey("AssetClassId")]
        public virtual TblAssetClass AssetClass { get; set; } = null!;
    }
}
