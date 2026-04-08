using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace DogoFinance.DataAccess.Layer.Models.Entities
{
    [Table("TBL_PRODUCT")]
    [Index(nameof(Code), Name = "UQ_TBL_PRODUCT_Code", IsUnique = true)]
    [Index(nameof(ProductTypeId), Name = "IX_TBL_PRODUCT_ProductTypeId")]
    public partial class TblProduct
    {
        public TblProduct()
        {
            TblAssetAllocations = new HashSet<TblAssetAllocation>();
        }

        [Key]
        public int ProductId { get; set; }
        [StringLength(150)]
        public string Name { get; set; } = null!;
        [StringLength(50)]
        public string Code { get; set; } = null!;
        public int ProductTypeId { get; set; }
        [StringLength(50)]
        public string? RiskLevel { get; set; }
        public string? Description { get; set; }
        public bool IsActive { get; set; }
        public DateTime CreatedAt { get; set; }
        public int? MinTenorInDays { get; set; }
        public int? MaxTenorInDays { get; set; }

        [ForeignKey(nameof(ProductTypeId))]
        [InverseProperty(nameof(TblProductType.TblProducts))]
        public virtual TblProductType ProductType { get; set; } = null!;
        [InverseProperty(nameof(TblAssetAllocation.Product))]
        public virtual ICollection<TblAssetAllocation> TblAssetAllocations { get; set; }
    }
}
