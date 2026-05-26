using System.ComponentModel.DataAnnotations;

namespace DogoFinance.BusinessLogic.Layer.Models.Request
{
    public class AdminManualFundingReviewRequest
    {
        [Required]
        public long RequestId { get; set; }

        [Required]
        [RegularExpression("^(Approved|Rejected)$", ErrorMessage = "Status must be either 'Approved' or 'Rejected'.")]
        public string Status { get; set; } = null!;

        public string? AdminNotes { get; set; }
    }
}
