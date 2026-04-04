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
}
