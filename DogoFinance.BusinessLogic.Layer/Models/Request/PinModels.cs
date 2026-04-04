using System.ComponentModel.DataAnnotations;

namespace DogoFinance.BusinessLogic.Layer.Models.Request
{
    public class ChangePinRequest
    {
        [Required]
        public string OldPin { get; set; } = null!;

        [Required]
        [RegularExpression(@"^\d{6}$")]
        public string NewPin { get; set; } = null!;

        [Compare(nameof(NewPin))]
        public string ConfirmNewPin { get; set; } = null!;
    }

    public class ForgotPinRequest
    {
        [Required]
        [EmailAddress]
        public string Email { get; set; } = null!;
    }

    public class ResetPinRequest
    {
        [Required]
        public string Token { get; set; } = null!;

        [Required]
        [RegularExpression(@"^\d{6}$")]
        public string NewPin { get; set; } = null!;
    }
}
