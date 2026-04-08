using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace DogoFinance.DataAccess.Layer.Models.Entities
{
    [Table("TBL_MODULE")]
    public partial class TblModule
    {
        public TblModule()
        {
            TblAccessRights = new HashSet<TblAccessRight>();
        }

        [Key]
        public int Id { get; set; }
        [StringLength(100)]
        public string Name { get; set; } = null!;
        [StringLength(50)]
        public string Icon { get; set; } = null!;
        [StringLength(250)]
        public string? Description { get; set; }

        [InverseProperty(nameof(TblAccessRight.Module))]
        public virtual ICollection<TblAccessRight> TblAccessRights { get; set; }
    }
}
