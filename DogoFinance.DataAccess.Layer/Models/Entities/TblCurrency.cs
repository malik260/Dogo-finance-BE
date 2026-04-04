using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace DogoFinance.DataAccess.Layer.Models.Entities
{
    [Table("TBL_CURRENCY")]
    public partial class TblCurrency
    {
        [Key]
        public int Id { get; set; }
        [StringLength(10)]
        public string Code { get; set; } = null!;
        [StringLength(50)]
        public string Name { get; set; } = null!;
        [StringLength(10)]
        public string Symbol { get; set; } = null!;
        [Required]
        public bool? IsActive { get; set; }
    }
}
