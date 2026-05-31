using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace DogoFinance.DataAccess.Layer.Models.Entities
{
    [Table("TBL_CUSTOMER_BANK")]
    public partial class TblCustomerBank
    {
        [Key]
        public long CustomerBankId { get; set; }
        public long CustomerId { get; set; }
        public int BankId { get; set; }
        [StringLength(20)]
        public string AccountNumber { get; set; } = null!;
        [StringLength(100)]
        public string AccountName { get; set; } = null!;
        public bool IsDefault { get; set; }
        public DateTime CreatedAt { get; set; }
        public bool IsDeleted { get; set; }

        // Domiciliary / International Routing Fields
        [StringLength(3)]
        public string CurrencyCode { get; set; } = "NGN";
        
        [StringLength(50)]
        public string? SwiftCode { get; set; }
        
        [StringLength(50)]
        public string? SortCode { get; set; }
        
        [StringLength(250)]
        public string? CorrespondentBank { get; set; }
        
        [StringLength(50)]
        public string? Iban { get; set; }
        
        [StringLength(250)]
        public string? BeneficiaryAccountName { get; set; }
        
        [StringLength(50)]
        public string? BeneficiaryAccountNumber { get; set; }
        
        [StringLength(500)]
        public string? BeneficiaryAddress { get; set; }
        
        [StringLength(500)]
        public string? FfcDetails { get; set; }

        [ForeignKey(nameof(BankId))]
        [InverseProperty(nameof(TblBank.TblCustomerBanks))]
        public virtual TblBank Bank { get; set; } = null!;
        [ForeignKey(nameof(CustomerId))]
        [InverseProperty(nameof(TblCustomer.TblCustomerBanks))]
        public virtual TblCustomer Customer { get; set; } = null!;
    }
}
