using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace DogoFinance.DataAccess.Layer.Models.Entities
{
    [Table("TBL_PRODUCT_TYPE")]
    [Index(nameof(Code), Name = "UQ_TBL_PRODUCT_TYPE_Code", IsUnique = true)]
    public partial class TblProductType
    {
        public TblProductType()
        {
            TblProducts = new HashSet<TblProduct>();
        }

        [Key]
        public int ProductTypeId { get; set; }
        [StringLength(100)]
        public string Name { get; set; } = null!;
        [StringLength(50)]
        public string Code { get; set; } = null!;
        public bool SupportsAllocation { get; set; }
        public bool SupportsProfitSharing { get; set; }
        public DateTime CreatedAt { get; set; }

        [InverseProperty(nameof(TblProduct.ProductType))]
        public virtual ICollection<TblProduct> TblProducts { get; set; }
    }
}
