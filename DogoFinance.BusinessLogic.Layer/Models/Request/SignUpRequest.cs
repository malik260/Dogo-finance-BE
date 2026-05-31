using System.ComponentModel.DataAnnotations;

namespace DogoFinance.BusinessLogic.Layer.Models.Request
{
    public class SignUpRequest
    {
        [Required]
        public int CustomerTypeId { get; set; }

        [StringLength(100)]
        public string? FirstName { get; set; }

        [StringLength(100)]
        public string? LastName { get; set; }

        // Corporate specific fields
        [StringLength(250)]
        public string? BusinessName { get; set; }

        [StringLength(100)]
        public string? RegistrationNumber { get; set; }

        [StringLength(50)]
        public string? TaxIdentificationNumber { get; set; }

        public DateTime? DateOfIncorporation { get; set; }

        [Required]
        [EmailAddress]
        [StringLength(150)]
        public string Email { get; set; } = null!;

        [Required]
        [Phone]
        [StringLength(20)]
        public string PhoneNumber { get; set; } = null!;

        [Required]
        [MinLength(8)]
        [StringLength(100)]
        public string Password { get; set; } = null!;

        [Required]
        [Compare(nameof(Password))]
        public string ConfirmPassword { get; set; } = null!;

        public DateTime? DateOfBirth { get; set; }

        public int? GenderId { get; set; }

        public bool IsPoliticallyExposed { get; set; }

        public string? ReferralCode { get; set; }
    }
}
