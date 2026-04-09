using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace DogoFinance.DataAccess.Layer.Models.Entities
{
    [Table("TBL_INSTRUMENT")]
    public partial class TblInstrument
    {
        [Key]
        public int InstrumentId { get; set; }
        [StringLength(300)]
        public string Name { get; set; } = null!;
        [StringLength(100)]
        public string? Code { get; set; }
        public int AssetClassId { get; set; }
        public bool IsShariahCompliant { get; set; }
        public bool IsActive { get; set; }
        public DateTime CreatedAt { get; set; }

        [ForeignKey("AssetClassId")]
        public virtual TblAssetClass AssetClass { get; set; } = null!;
    }
}
