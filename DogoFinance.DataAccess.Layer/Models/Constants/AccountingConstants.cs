namespace DogoFinance.DataAccess.Layer.Models.Constants
{
    public static class EntryType
    {
        public const int CREDIT = 1; // Inflow (Savings/Deposit)
        public const int DEBIT = 2;  // Outflow (Withdrawal)
    }

    public static class TransactionType
    {
        public const int DEPOSIT = 1;
        public const int WITHDRAWAL = 2;
    }
}
