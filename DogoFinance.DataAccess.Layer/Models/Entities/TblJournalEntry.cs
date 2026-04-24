using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace DogoFinance.DataAccess.Layer.Models.Entities
{
    [Table("TBL_JOURNAL_ENTRY")]
    public partial class TblJournalEntry
    {
        [Key]
        public long Id { get; set; }

        [Required]
        [StringLength(100)]
        public string Reference { get; set; } = null!;

        [StringLength(500)]
        public string? Narration { get; set; }

        public DateTime TransactionDate { get; set; }

        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

        public long? CreatedByUserId { get; set; }

        [InverseProperty(nameof(TblJournalLine.JournalEntry))]
        public virtual ICollection<TblJournalLine> JournalLines { get; set; } = new List<TblJournalLine>();
    }
}
