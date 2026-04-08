using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace DogoFinance.DataAccess.Layer.Models.Entities
{
    [Table("TBL_ACCESS_RIGHT")]
    public partial class TblAccessRight
    {
        [Key]
        public int Id { get; set; }
        public int ModuleId { get; set; }
        [StringLength(100)]
        public string Name { get; set; } = null!;
        [StringLength(100)]
        public string Label { get; set; } = null!;

        [ForeignKey(nameof(ModuleId))]
        [InverseProperty(nameof(TblModule.TblAccessRights))]
        public virtual TblModule Module { get; set; } = null!;
    }
}
