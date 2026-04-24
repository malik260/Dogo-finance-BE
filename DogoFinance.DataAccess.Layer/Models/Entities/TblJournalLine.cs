using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace DogoFinance.DataAccess.Layer.Models.Entities
{
    [Table("TBL_JOURNAL_LINE")]
    public partial class TblJournalLine
    {
        [Key]
        public long Id { get; set; }

        public long JournalEntryId { get; set; }

        public int AccountId { get; set; }

        [Column(TypeName = "decimal(18, 2)")]
        public decimal Debit { get; set; }

        [Column(TypeName = "decimal(18, 2)")]
        public decimal Credit { get; set; }

        [StringLength(500)]
        public string? Narration { get; set; }

        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

        [ForeignKey(nameof(JournalEntryId))]
        [InverseProperty(nameof(TblJournalEntry.JournalLines))]
        public virtual TblJournalEntry JournalEntry { get; set; } = null!;

        [ForeignKey(nameof(AccountId))]
        [InverseProperty(nameof(TblChartOfAccount.JournalLines))]
        public virtual TblChartOfAccount Account { get; set; } = null!;
    }
}
