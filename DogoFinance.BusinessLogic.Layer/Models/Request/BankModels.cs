using System;

namespace DogoFinance.BusinessLogic.Layer.Models.Request
{
    public class AddCustomerBankRequest
    {
        public int BankId { get; set; }
        public string AccountNumber { get; set; } = null!;
        public string AccountName { get; set; } = null!;
        public bool IsDefault { get; set; }

        public string? CurrencyCode { get; set; }
        public string? SwiftCode { get; set; }
        public string? SortCode { get; set; }
        public string? CorrespondentBank { get; set; }
        public string? Iban { get; set; }
        public string? BeneficiaryAccountName { get; set; }
        public string? BeneficiaryAccountNumber { get; set; }
        public string? BeneficiaryAddress { get; set; }
        public string? FfcDetails { get; set; }
    }

    public class UpdateCustomerBankRequest
    {
        public long CustomerBankId { get; set; }
        public bool IsDefault { get; set; }
    }
}
