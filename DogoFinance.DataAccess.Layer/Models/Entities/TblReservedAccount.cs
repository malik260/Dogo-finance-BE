using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace DogoFinance.DataAccess.Layer.Models.Entities
{
    [Table("TBL_RESERVED_ACCOUNT")]
    public class TblReservedAccount
    {
        [Key]
        public int Id { get; set; }
        public long UserId { get; set; }
        public string AccountReference { get; set; } = null!;
        public string AccountNumber { get; set; } = null!;
        public string BankName { get; set; } = null!;
        public string BankCode { get; set; } = null!;
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

        [ForeignKey("UserId")]
        public virtual TblUser User { get; set; } = null!;
    }
}
