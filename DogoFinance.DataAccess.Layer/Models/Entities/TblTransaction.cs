using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace DogoFinance.DataAccess.Layer.Models.Entities
{
    [Table("TBL_TRANSACTION")]
    public partial class TblTransaction
    {
        public TblTransaction()
        {
            InverseReversedTransaction = new HashSet<TblTransaction>();
        }

        [Key]
        public long TransactionId { get; set; }
        [StringLength(100)]
        public string Reference { get; set; } = null!;
        public int TransactionType { get; set; }
        [Column(TypeName = "decimal(18, 2)")]
        public decimal Amount { get; set; }
        public int? Status { get; set; }
        [StringLength(255)]
        public string? Narration { get; set; }
        public long? PaymentId { get; set; }
        public bool IsReversed { get; set; }
        public long? ReversedTransactionId { get; set; }
        public long InitiatedByUserId { get; set; }
        public long? ApprovedByUserId { get; set; }
        public DateTime? ApprovedAt { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime? UpdatedAt { get; set; }

        [ForeignKey(nameof(ApprovedByUserId))]
        [InverseProperty(nameof(TblUser.TblTransactionApprovedByUsers))]
        public virtual TblUser? ApprovedByUser { get; set; }
        [ForeignKey(nameof(InitiatedByUserId))]
        [InverseProperty(nameof(TblUser.TblTransactionInitiatedByUsers))]
        public virtual TblUser InitiatedByUser { get; set; } = null!;
        [ForeignKey(nameof(PaymentId))]
        [InverseProperty(nameof(TblPayment.TblTransactions))]
        public virtual TblPayment? Payment { get; set; }
        [ForeignKey(nameof(ReversedTransactionId))]
        [InverseProperty(nameof(TblTransaction.InverseReversedTransaction))]
        public virtual TblTransaction? ReversedTransaction { get; set; }
        [InverseProperty(nameof(TblTransaction.ReversedTransaction))]
        public virtual ICollection<TblTransaction> InverseReversedTransaction { get; set; }
    }
}
