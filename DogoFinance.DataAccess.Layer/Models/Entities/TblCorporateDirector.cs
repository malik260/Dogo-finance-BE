using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace DogoFinance.DataAccess.Layer.Models.Entities
{
    [Table("TBL_CORPORATE_DIRECTOR")]
    public partial class TblCorporateDirector
    {
        [Key]
        public int DirectorId { get; set; }

        public long CustomerId { get; set; }

        [StringLength(50)]
        public string Title { get; set; } = null!;

        [StringLength(100)]
        public string Surname { get; set; } = null!;

        [StringLength(100)]
        public string FirstName { get; set; } = null!;

        [StringLength(100)]
        public string? OtherNames { get; set; }

        [StringLength(100)]
        public string Designation { get; set; } = null!;

        [Column(TypeName = "date")]
        public DateTime DateOfBirth { get; set; }

        [StringLength(500)]
        public string ResidentialAddress { get; set; } = null!;

        [StringLength(200)]
        public string BusinessEmail { get; set; } = null!;

        [StringLength(20)]
        public string PhoneNumber { get; set; } = null!;

        [StringLength(20)]
        public string Bvn { get; set; } = null!;

        [StringLength(100)]
        public string Nationality { get; set; } = null!;

        [StringLength(20)]
        public string Gender { get; set; } = null!;

        [StringLength(50)]
        public string SigningClass { get; set; } = null!;

        [StringLength(50)]
        public string IdentityType { get; set; } = null!;

        [StringLength(100)]
        public string IdNumber { get; set; } = null!;

        public bool IsPep { get; set; }

        [StringLength(500)]
        public string PassportPhotoUrl { get; set; } = null!;

        [StringLength(500)]
        public string SignatureCardUrl { get; set; } = null!;

        [StringLength(500)]
        public string IdentityDocumentUrl { get; set; } = null!;

        public bool IsActive { get; set; }

        public DateTime CreatedAt { get; set; }

        public DateTime? UpdatedAt { get; set; }

        public DateTime? DeletedAt { get; set; }

        public bool IsDeleted { get; set; }

        [ForeignKey("CustomerId")]
        [InverseProperty("TblCorporateDirectors")]
        public virtual TblCustomer Customer { get; set; } = null!;
    }
}
