using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Http;

namespace DogoFinance.BusinessLogic.Layer.Models.Request
{
    public class ManualFundingRequestDto
    {
        [Required]
        [Range(100, double.MaxValue, ErrorMessage = "Amount must be at least 100.")]
        public decimal Amount { get; set; }

        [Required]
        [StringLength(100, ErrorMessage = "Reference cannot exceed 100 characters.")]
        public string Reference { get; set; } = null!;

        public string? ReceiptPath { get; set; }
        
        public IFormFile? ReceiptFile { get; set; }
    }
}
