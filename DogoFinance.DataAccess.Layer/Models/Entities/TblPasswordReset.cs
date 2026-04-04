using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace DogoFinance.DataAccess.Layer.Models.Entities
{
    [Table("TBL_PASSWORD_RESET")]
    public partial class TblPasswordReset
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
        [InverseProperty(nameof(TblUser.TblPasswordResets))]
        public virtual TblUser User { get; set; } = null!;
    }
}
