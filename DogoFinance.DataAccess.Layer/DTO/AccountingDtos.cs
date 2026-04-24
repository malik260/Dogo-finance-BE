using System;
using System.Collections.Generic;

namespace DogoFinance.DataAccess.Layer.DTO
{
    public class JournalEntryDto
    {
        public string Reference { get; set; } = null!;
        public string? Narration { get; set; }
        public DateTime TransactionDate { get; set; }
        public List<JournalLineDto> Lines { get; set; } = new();
    }

    public class JournalLineDto
    {
        public string AccountCode { get; set; } = null!;
        public decimal Debit { get; set; }
        public decimal Credit { get; set; }
        public string? Narration { get; set; }
    }

    public class TrialBalanceDto
    {
        public string AccountCode { get; set; } = null!;
        public string AccountName { get; set; } = null!;
        public string AccountType { get; set; } = null!;
        public decimal TotalDebit { get; set; }
        public decimal TotalCredit { get; set; }
        public decimal Balance { get; set; }
    }

    public class ChartOfAccountDto
    {
        public int Id { get; set; }
        public string AccountCode { get; set; } = null!;
        public string AccountName { get; set; } = null!;
        public string AccountType { get; set; } = null!;
        public bool IsLeaf { get; set; }
    }
}
