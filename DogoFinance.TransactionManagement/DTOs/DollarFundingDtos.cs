using System.ComponentModel.DataAnnotations;

namespace DogoFinance.TransactionManagement.DTOs
{
    public class FxRateQuoteResponse
    {
        public decimal NgnAmount { get; set; }
        public decimal BaseNgnPerUsdRate { get; set; }
        public decimal EffectiveRateWithMargin { get; set; }
        public decimal EstimatedUsdAmount { get; set; }
        public string Provider { get; set; } = string.Empty;
        public bool IsFallbackRate { get; set; }
        public DateTime Timestamp { get; set; } = DateTime.UtcNow;
    }

    public class FundDollarWalletFromNairaRequest
    {
        [Required]
        [Range(1, 100000000, ErrorMessage = "Naira amount must be greater than zero.")]
        public decimal NairaAmount { get; set; }
    }

    public class FundDollarWalletViaWireRequest
    {
        [Required]
        [Range(1, 1000000, ErrorMessage = "USD amount must be greater than zero.")]
        public decimal UsdAmount { get; set; }

        [Required]
        public string ProofDocumentUrl { get; set; } = string.Empty;

        [Required]
        public string BankReference { get; set; } = string.Empty;

        public string? Remarks { get; set; }
    }
}
