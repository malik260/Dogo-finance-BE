using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace DogoFinance.DataAccess.Layer.Models.Entities
{
    [Table("TBL_TRANSACTION_APPROVAL")]
    public partial class TblTransactionApproval
    {
        [Key]
        public long Id { get; set; }

        public long TransactionId { get; set; }

        public long ApproverUserId { get; set; }

        [StringLength(50)]
        public string Status { get; set; } = "Pending";

        [StringLength(500)]
        public string? RejectionReason { get; set; }

        public DateTime CreatedAt { get; set; }

        public DateTime? ActedAt { get; set; }

        [ForeignKey(nameof(TransactionId))]
        [InverseProperty(nameof(TblTransaction.TblTransactionApprovals))]
        public virtual TblTransaction Transaction { get; set; } = null!;

        [ForeignKey(nameof(ApproverUserId))]
        [InverseProperty(nameof(TblUser.TblTransactionApprovals))]
        public virtual TblUser ApproverUser { get; set; } = null!;
    }
}
