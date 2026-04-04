using System;

namespace DogoFinance.BusinessLogic.Layer.Models.Request
{
    public class AddCustomerBankRequest
    {
        public int BankId { get; set; }
        public string AccountNumber { get; set; } = null!;
        public string AccountName { get; set; } = null!;
        public bool IsDefault { get; set; }
    }

    public class UpdateCustomerBankRequest
    {
        public long CustomerBankId { get; set; }
        public bool IsDefault { get; set; }
    }
}
