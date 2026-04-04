using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace DogoFinance.DataAccess.Layer.Models.Entities
{
    [Table("TBL_BANK")]
    public partial class TblBank
    {
        public TblBank()
        {
            TblCustomerBanks = new HashSet<TblCustomerBank>();
        }

        [Key]
        public int BankId { get; set; }
        [StringLength(100)]
        public string BankName { get; set; } = null!;
        [StringLength(10)]
        public string BankCode { get; set; } = null!;
        [StringLength(200)]
        public string? LogoUrl { get; set; }
        public bool IsActive { get; set; }
        public DateTime CreatedAt { get; set; }

        [InverseProperty(nameof(TblCustomerBank.Bank))]
        public virtual ICollection<TblCustomerBank> TblCustomerBanks { get; set; }
    }
}
