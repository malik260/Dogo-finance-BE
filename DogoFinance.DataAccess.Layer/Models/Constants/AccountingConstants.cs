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
        public const int PROFIT = 3;
        public const int INVESTMENT = 4;
        public const int LIQUIDATION = 5;
    }

    public static class LiquidationStatus
    {
        public const int PENDING_APPROVAL = 1;
        public const int PENDING_NOTICE = 2;
        public const int COMPLETED = 3;
        public const int REJECTED = 4;
    }
}

