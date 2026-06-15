using System.ComponentModel.DataAnnotations;

namespace DogoFinance.BusinessLogic.Layer.Models.Request
{
    public class AddNextOfKinRequest
    {
        [Required]
        [StringLength(150)]
        public string FullName { get; set; } = null!;

        [Required]
        public int RelationshipTypeId { get; set; }

        [Required]
        [Phone]
        [StringLength(20)]
        public string PhoneNumber { get; set; } = null!;

        [EmailAddress]
        [StringLength(150)]
        public string? Email { get; set; }

        [StringLength(250)]
        public string? Address { get; set; }
    }
    public class BvnVerificationRequest
    {
        [Required]
        [StringLength(11)]
        public string Bvn { get; set; } = null!;
    }

    public class NinVerificationRequest
    {
        [Required]
        [StringLength(11)]
        public string Nin { get; set; } = null!;
    }

    public class UpdateProfileRequest
    {
        public string? FirstName { get; set; }
        public string? LastName { get; set; }
        public string? Phone { get; set; }
    }

    public class AddressVerificationRequest
    {
        [Required]
        public int DocTypeId { get; set; }
        
        [Required]
        public Microsoft.AspNetCore.Http.IFormFile File { get; set; } = null!;
    }

    public class AdminAddressReviewRequest
    {
        public long VerificationId { get; set; }
        public bool Approved { get; set; }
        public string? AdminNotes { get; set; }
        public string? CorrectedAddress { get; set; }
        public string? CorrectedCity { get; set; }
        public string? CorrectedState { get; set; }
    }

    public class UpdateCorporateProfileRequest
    {
        public string? CompanyName { get; set; }
        public string? RegistrationNumber { get; set; }
        public DateTime? DateOfIncorporation { get; set; }
        public int? NatureOfBusinessId { get; set; }
        public int? CountryId { get; set; }
        public int? StateId { get; set; }
        public string? City { get; set; }
        public string? Address { get; set; }
        public string? EntityType { get; set; }
        public string? OtherEntityType { get; set; }
        public string? Phone { get; set; }
        public string? Tin { get; set; }
        public string? Email { get; set; }
        public string? AnnualTurnover { get; set; }
        public string? SourceOfFunds { get; set; }
        public string? ClientSegmentation { get; set; }
        public string? SignatoryMandate { get; set; }
    }

    public class UpdateCorporateContactRequest
    {
        [Required]
        [StringLength(150)]
        public string FullName { get; set; } = null!;

        [Required]
        [EmailAddress]
        [StringLength(150)]
        public string Email { get; set; } = null!;

        [Required]
        [Phone]
        [StringLength(50)]
        public string PhoneNumber { get; set; } = null!;
    }

    public class CorporateVerificationDto
    {
        public string Name { get; set; } = null!;
        public string Type { get; set; } = null!;
        public string Status { get; set; } = null!; // "verified", "pending", "unverified"
        public string Icon { get; set; } = null!;
        public string Date { get; set; } = "N/A";
        public bool RequiresUpload { get; set; } = true;
    }

    public class UploadCorporateDocumentRequest
    {
        [Required]
        public string DocumentType { get; set; } = null!;

        [Required]
        public Microsoft.AspNetCore.Http.IFormFile File { get; set; } = null!;
    }

    public class AddCorporateSignatoryRequest
    {
        [Required] public string Title { get; set; } = null!;
        [Required] public string Surname { get; set; } = null!;
        [Required] public string FirstName { get; set; } = null!;
        public string? OtherNames { get; set; }
        [Required] public string Designation { get; set; } = null!;
        [Required] public string DateOfBirth { get; set; } = null!;
        [Required] public string ResidentialAddress { get; set; } = null!;
        [Required] public string BusinessEmail { get; set; } = null!;
        [Required] public string PhoneNumber { get; set; } = null!;
        [Required] public string Bvn { get; set; } = null!;
        [Required] public string Nationality { get; set; } = null!;
        [Required] public string Gender { get; set; } = null!;
        [Required] public string SigningClass { get; set; } = null!;
        [Required] public string IdentityType { get; set; } = null!;
        [Required] public string IdNumber { get; set; } = null!;
        [Required] public bool IsPep { get; set; }

        [Required] public Microsoft.AspNetCore.Http.IFormFile PassportPhoto { get; set; } = null!;
        [Required] public Microsoft.AspNetCore.Http.IFormFile SignatureCard { get; set; } = null!;
        [Required] public Microsoft.AspNetCore.Http.IFormFile IdentityDocument { get; set; } = null!;
    }
    public class AddCorporateDirectorRequest
    {
        [Required] public string Title { get; set; } = null!;
        [Required] public string Surname { get; set; } = null!;
        [Required] public string FirstName { get; set; } = null!;
        public string? OtherNames { get; set; }
        [Required] public string Designation { get; set; } = null!;
        [Required] public string DateOfBirth { get; set; } = null!;
        [Required] public string ResidentialAddress { get; set; } = null!;
        [Required] public string BusinessEmail { get; set; } = null!;
        [Required] public string PhoneNumber { get; set; } = null!;
        [Required] public string Bvn { get; set; } = null!;
        [Required] public string Nationality { get; set; } = null!;
        [Required] public string Gender { get; set; } = null!;
        [Required] public string SigningClass { get; set; } = null!;
        [Required] public string IdentityType { get; set; } = null!;
        [Required] public string IdNumber { get; set; } = null!;
        [Required] public bool IsPep { get; set; }

        [Required] public Microsoft.AspNetCore.Http.IFormFile PassportPhoto { get; set; } = null!;
        [Required] public Microsoft.AspNetCore.Http.IFormFile SignatureCard { get; set; } = null!;
        [Required] public Microsoft.AspNetCore.Http.IFormFile IdentityDocument { get; set; } = null!;
    }
}
