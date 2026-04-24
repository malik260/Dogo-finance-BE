using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace DogoFinance.DataAccess.Layer.Models.Entities
{
    [Table("TBL_SYSTEM_SETTING")]
    public partial class TblSystemSetting
    {
        [Key]
        public int Id { get; set; }
        public int SessionTimeoutInMinutes { get; set; }
        [Column(TypeName = "decimal(18, 2)")]
        public decimal WithdrawalAutoThreshold { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime? UpdatedAt { get; set; }
    }
}
