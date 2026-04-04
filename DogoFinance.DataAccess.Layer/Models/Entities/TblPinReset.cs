using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace DogoFinance.DataAccess.Layer.Models.Entities
{
    [Table("TBL_PIN_RESET")]
    public partial class TblPinReset
    {
        [Key]
        public long Id { get; set; }
        public long UserId { get; set; }
        [StringLength(100)]
        public string ResetCode { get; set; } = null!;
        public DateTime ExpiresAt { get; set; }
        public bool IsUsed { get; set; }
        public DateTime CreatedAt { get; set; }

        [ForeignKey(nameof(UserId))]
        public virtual TblUser User { get; set; } = null!;
    }
}
