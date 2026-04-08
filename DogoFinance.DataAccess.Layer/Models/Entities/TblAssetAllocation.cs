using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace DogoFinance.DataAccess.Layer.Models.Entities
{
    [Table("TBL_ASSET_ALLOCATION")]
    [Index(nameof(ProductId), Name = "IX_TBL_ASSET_ALLOCATION_ProductId")]
    [Index(nameof(AssetTypeId), Name = "IX_TBL_ASSET_ALLOCATION_AssetTypeId")]
    [Index(nameof(ProductId), nameof(AssetTypeId), Name = "UQ_TBL_ASSET_ALLOCATION_ProductAsset", IsUnique = true)]
    public partial class TblAssetAllocation
    {
        [Key]
        public int Id { get; set; }
        public int ProductId { get; set; }
        public int AssetTypeId { get; set; }
        [Column(TypeName = "decimal(5, 2)")]
        public decimal TargetPercentage { get; set; }
        [Column(TypeName = "decimal(5, 2)")]
        public decimal MinPercentage { get; set; }
        [Column(TypeName = "decimal(5, 2)")]
        public decimal MaxPercentage { get; set; }

        [ForeignKey(nameof(AssetTypeId))]
        [InverseProperty(nameof(TblAssetType.TblAssetAllocations))]
        public virtual TblAssetType AssetType { get; set; } = null!;
        [ForeignKey(nameof(ProductId))]
        [InverseProperty(nameof(TblProduct.TblAssetAllocations))]
        public virtual TblProduct Product { get; set; } = null!;
    }
}
