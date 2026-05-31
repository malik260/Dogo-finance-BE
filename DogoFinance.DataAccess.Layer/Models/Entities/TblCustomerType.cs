using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace DogoFinance.DataAccess.Layer.Models.Entities
{
    [Table("TBL_CUSTOMER_TYPE")]
    public partial class TblCustomerType
    {
        public TblCustomerType()
        {
            TblCustomers = new HashSet<TblCustomer>();
        }

        [Key]
        public int Id { get; set; }

        [Required]
        [StringLength(50)]
        public string Name { get; set; } = null!;

        [StringLength(200)]
        public string? Description { get; set; }

        public bool IsActive { get; set; }

        [InverseProperty(nameof(TblCustomer.CustomerType))]
        public virtual ICollection<TblCustomer> TblCustomers { get; set; }
    }
}
