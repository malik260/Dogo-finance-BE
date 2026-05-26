using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace DogoFinance.DataAccess.Layer.Models.Entities
{
    [Table("TBL_COMPANY_PROFILE")]
    public partial class TblCompanyProfile
    {
        [Key]
        public int Id { get; set; }

        [Required]
        [StringLength(250)]
        public string CompanyName { get; set; } = null!;

        [Required]
        [StringLength(500)]
        public string Address { get; set; } = null!;

        [Required]
        [StringLength(50)]
        public string PhoneNumber { get; set; } = null!;

        [Required]
        [StringLength(150)]
        public string Email { get; set; } = null!;

        [Required]
        [StringLength(100)]
        public string RcNumber { get; set; } = null!;

        public DateTime? DateOfIncorporation { get; set; }

        public int? BankId { get; set; }

        [StringLength(50)]
        public string? AccountNumber { get; set; }

        [StringLength(250)]
        public string? XLink { get; set; }

        [StringLength(250)]
        public string? FacebookLink { get; set; }

        public DateTime CreatedAt { get; set; }
        public DateTime? UpdatedAt { get; set; }
    }
}
