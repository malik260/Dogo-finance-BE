using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace DogoFinance.DataAccess.Layer.Models.Entities
{
    [Table("TBL_LEDGER")]
    public partial class TblLedger
    {
        [Key]
        public long Id { get; set; }
        public long TransactionId { get; set; }
        public long WalletId { get; set; }
        public int EntryType { get; set; }
        [Column(TypeName = "decimal(18, 2)")]
        public decimal Amount { get; set; }
        [Column(TypeName = "decimal(18, 2)")]
        public decimal BalanceAfter { get; set; }
        [StringLength(255)]
        public string? Narration { get; set; }
        public DateTime CreatedAt { get; set; }
    }
}
