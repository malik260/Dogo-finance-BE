using System.ComponentModel.DataAnnotations;

namespace DogoFinance.BusinessLogic.Layer.Models.Request
{
    public class WithdrawalRequest
    {
        [Required]
        public long CustomerId { get; set; }
        
        [Required]
        [Range(100, 10000000)]
        public decimal Amount { get; set; }

        [Required]
        public string BankCode { get; set; } = null!;

        [Required]
        public string AccountNumber { get; set; } = null!;

        [Required]
        [RegularExpression(@"^\d{6}$")]
        public string Pin { get; set; } = null!;

        public string? Narration { get; set; }
        public string? Otp { get; set; }
    }
}
