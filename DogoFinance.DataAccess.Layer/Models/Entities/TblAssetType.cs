using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace DogoFinance.DataAccess.Layer.Models.Entities
{
    [Table("TBL_ASSET_TYPE")]
    [Index(nameof(Code), Name = "UQ_TBL_ASSET_TYPE_Code", IsUnique = true)]
    public partial class TblAssetType
    {
        public TblAssetType()
        {
            TblAssetAllocations = new HashSet<TblAssetAllocation>();
        }

        [Key]
        public int AssetTypeId { get; set; }
        [StringLength(150)]
        public string Name { get; set; } = null!;
        [StringLength(50)]
        public string Code { get; set; } = null!;
        public bool IsShariahCompliant { get; set; }
        public DateTime CreatedAt { get; set; }

        [InverseProperty(nameof(TblAssetAllocation.AssetType))]
        public virtual ICollection<TblAssetAllocation> TblAssetAllocations { get; set; }
    }
}
