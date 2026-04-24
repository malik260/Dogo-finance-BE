using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace DogoFinance.DataAccess.Layer.Models.Entities
{
    [Table("TBL_ADDRESS_DOC_TYPE")]
    public partial class TblAddressDocType
    {
        public TblAddressDocType()
        {
            TblCustomerAddressVerifications = new HashSet<TblCustomerAddressVerification>();
        }

        [Key]
        public int Id { get; set; }
        
        [StringLength(100)]
        public string Name { get; set; } = null!;
        
        [StringLength(250)]
        public string? Description { get; set; }
        
        public bool IsActive { get; set; }
        
        public DateTime CreatedAt { get; set; }

        [InverseProperty(nameof(TblCustomerAddressVerification.DocType))]
        public virtual ICollection<TblCustomerAddressVerification> TblCustomerAddressVerifications { get; set; }
    }
}
