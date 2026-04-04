using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace DogoFinance.DataAccess.Layer.Models.Entities
{
    [Table("TBL_PAYMENT")]
    public partial class TblPayment
    {
        public TblPayment()
        {
            TblTransactions = new HashSet<TblTransaction>();
        }

        [Key]
        public long Id { get; set; }
        public long UserId { get; set; }
        [Column(TypeName = "decimal(18, 2)")]
        public decimal Amount { get; set; }
        public int PaymentProvider { get; set; }
        [StringLength(100)]
        public string? ProviderReference { get; set; }
        [StringLength(20)]
        public string Status { get; set; } = null!;
        public DateTime CreatedAt { get; set; }

        [ForeignKey(nameof(UserId))]
        [InverseProperty(nameof(TblUser.TblPayments))]
        public virtual TblUser User { get; set; } = null!;
        [InverseProperty(nameof(TblTransaction.Payment))]
        public virtual ICollection<TblTransaction> TblTransactions { get; set; }
    }
}
