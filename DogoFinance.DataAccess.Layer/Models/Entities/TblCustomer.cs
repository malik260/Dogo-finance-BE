using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace DogoFinance.DataAccess.Layer.Models.Entities
{
    [Table("TBL_CUSTOMER")]
    [Index(nameof(UserId), Name = "IX_TBL_CUSTOMER_UserId", IsUnique = true)]
    public partial class TblCustomer
    {
        public TblCustomer()
        {
            TblKycLogs = new HashSet<TblKycLog>();
            TblNextOfKins = new HashSet<TblNextOfKin>();
            TblWallets = new HashSet<TblWallet>();
            TblCustomerBanks = new HashSet<TblCustomerBank>();
            TblCustomerAddressVerifications = new HashSet<TblCustomerAddressVerification>();
        }

        [Key]
        public long CustomerId { get; set; }
        public long UserId { get; set; }
        [StringLength(100)]
        public string FirstName { get; set; } = null!;
        [StringLength(100)]
        public string LastName { get; set; } = null!;
        [StringLength(150)]
        public string? OtherNames { get; set; }
        [Column(TypeName = "date")]
        public DateTime DateOfBirth { get; set; }
        public int? Gender { get; set; }
        [Column("BVN")]
        [StringLength(11)]
        public string? Bvn { get; set; }
        [Column("BVNVerified")]
        public bool Bvnverified { get; set; }
        [Column("BVNVerifiedAt")]
        public DateTime? BvnverifiedAt { get; set; }
        [Column("NIN")]
        [StringLength(11)]
        public string? Nin { get; set; }
        [Column("NINVerified")]
        public bool Ninverified { get; set; }
        [Column("NINVerifiedAt")]
        public DateTime? NinverifiedAt { get; set; }
        [StringLength(250)]
        public string? Address { get; set; }
        [StringLength(100)]
        public string? City { get; set; }
        [StringLength(100)]
        public string? State { get; set; }
        public int? Country { get; set; } = null!;
        [Column("KYCStatus")]
        public int Kycstatus { get; set; }
        [Column("KYCVerifiedAt")]
        public DateTime? KycverifiedAt { get; set; }
        public DateTime CreatedAt { get; set; }
        public long? CreatedBy { get; set; }
        public DateTime? ModifiedAt { get; set; }
        public long? ModifiedBy { get; set; }
        public string? PhoneNumber { get; set; }
        public bool IsDeleted { get; set; }
        public bool? IsPolitcallyExposed { get; set; }
        [Timestamp]
        public byte[] RowVersion { get; set; } = null!;

        [ForeignKey(nameof(UserId))]
        [InverseProperty(nameof(TblUser.TblCustomer))]
        public virtual TblUser User { get; set; } = null!;
        [InverseProperty(nameof(TblKycLog.Customer))]
        public virtual ICollection<TblKycLog> TblKycLogs { get; set; }
        [InverseProperty(nameof(TblNextOfKin.Customer))]
        public virtual ICollection<TblNextOfKin> TblNextOfKins { get; set; }
        [InverseProperty(nameof(TblWallet.Customer))]
        public virtual ICollection<TblWallet> TblWallets { get; set; }
        [InverseProperty(nameof(TblCustomerBank.Customer))]
        public virtual ICollection<TblCustomerBank> TblCustomerBanks { get; set; }
        [InverseProperty(nameof(TblCustomerAddressVerification.Customer))]
        public virtual ICollection<TblCustomerAddressVerification> TblCustomerAddressVerifications { get; set; }
    }
}
