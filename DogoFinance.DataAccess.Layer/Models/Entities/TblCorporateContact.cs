using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace DogoFinance.DataAccess.Layer.Models.Entities
{
    [Table("TBL_CORPORATE_CONTACT")]
    public partial class TblCorporateContact
    {
        [Key]
        public long Id { get; set; }

        public long CustomerId { get; set; }

        [StringLength(150)]
        public string? FullName { get; set; }

        [StringLength(150)]
        public string? Email { get; set; }

        [StringLength(50)]
        public string? PhoneNumber { get; set; }

        public bool IsPrimary { get; set; } = true;

        [ForeignKey(nameof(CustomerId))]
        [InverseProperty(nameof(Entities.TblCustomer.TblCorporateContacts))]
        public virtual TblCustomer Customer { get; set; } = null!;
    }
}
