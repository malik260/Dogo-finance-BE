using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace DogoFinance.DataAccess.Layer.Models.Entities
{
    [Table("TBL_CHART_OF_ACCOUNT")]
    public partial class TblChartOfAccount
    {
        [Key]
        public int Id { get; set; }

        [Required]
        [StringLength(20)]
        public string AccountCode { get; set; } = null!;

        [Required]
        [StringLength(100)]
        public string AccountName { get; set; } = null!;

        [Required]
        [StringLength(50)]
        public string AccountType { get; set; } = null!; // Asset, Liability, Equity, Revenue, Expense

        public bool IsLeaf { get; set; } = true; // Can we post to this account?

        public bool IsActive { get; set; } = true;

        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

        [InverseProperty(nameof(TblJournalLine.Account))]
        public virtual ICollection<TblJournalLine> JournalLines { get; set; } = new List<TblJournalLine>();
    }
}
