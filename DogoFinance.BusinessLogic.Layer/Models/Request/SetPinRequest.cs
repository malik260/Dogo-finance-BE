using System.ComponentModel.DataAnnotations;

namespace DogoFinance.BusinessLogic.Layer.Models.Request
{
    public class SetPinRequest
    {
        [Required]
        [RegularExpression(@"^\d{6}$", ErrorMessage = "PIN must be exactly 6 digits.")]
        public string Pin { get; set; } = null!;

        [Required]
        [Compare(nameof(Pin), ErrorMessage = "PINs do not match.")]
        public string ConfirmPin { get; set; } = null!;
    }
}
