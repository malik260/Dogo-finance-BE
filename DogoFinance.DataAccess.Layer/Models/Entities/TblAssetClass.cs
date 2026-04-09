using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace DogoFinance.DataAccess.Layer.Models.Entities
{
    [Table("TBL_ASSET_CLASS")]
    public partial class TblAssetClass
    {
        [Key]
        public int AssetClassId { get; set; }
        [StringLength(300)]
        public string Name { get; set; } = null!;
        [StringLength(100)]
        public string Code { get; set; } = null!;
        public bool IsShariahCompliant { get; set; }
        public DateTime CreatedAt { get; set; }
    }
}
