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
            TblCorporateContacts = new HashSet<TblCorporateContact>();
            TblCorporateDocuments = new HashSet<TblCorporateDocument>();
            TblCorporateSignatories = new HashSet<TblCorporateSignatory>();
            TblCorporateDirectors = new HashSet<TblCorporateDirector>();
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
        public DateTime? DateOfBirth { get; set; }
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
        public int? StateId { get; set; }
        public int? Country { get; set; } = null!;
        public int? CountryId { get; set; }
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
        
        public int? CustomerTypeId { get; set; }
        [StringLength(250)]
        public string? BusinessName { get; set; }
        [StringLength(50)]
        public string? RegistrationNumber { get; set; }
        [StringLength(50)]
        public string? TaxIdentificationNumber { get; set; }
        [Column(TypeName = "date")]
        public DateTime? DateOfIncorporation { get; set; }

        [StringLength(250)]
        public string? NatureOfBusiness { get; set; }
        public int? NatureOfBusinessId { get; set; }
        [StringLength(50)]
        public string? EntityType { get; set; }
        [StringLength(100)]
        public string? OtherEntityType { get; set; }
        [StringLength(100)]
        public string? AnnualTurnover { get; set; }
        [StringLength(250)]
        public string? SourceOfFunds { get; set; }
        [StringLength(50)]
        public string? ClientSegmentation { get; set; }

        [StringLength(50)]
        public string? SignatoryMandate { get; set; }

        [Timestamp]
        public byte[] RowVersion { get; set; } = null!;

        [ForeignKey(nameof(UserId))]
        [InverseProperty(nameof(TblUser.TblCustomer))]
        public virtual TblUser User { get; set; } = null!;

        [ForeignKey(nameof(CustomerTypeId))]
        [InverseProperty(nameof(Entities.TblCustomerType.TblCustomers))]
        public virtual TblCustomerType? CustomerType { get; set; }

        [ForeignKey(nameof(CountryId))]
        public virtual TblCountry? TblCountry { get; set; }

        [ForeignKey(nameof(StateId))]
        public virtual TblState? TblState { get; set; }

        [ForeignKey(nameof(NatureOfBusinessId))]
        public virtual TblNatureOfBusiness? TblNatureOfBusiness { get; set; }

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
        [InverseProperty(nameof(TblCorporateContact.Customer))]
        public virtual ICollection<TblCorporateContact> TblCorporateContacts { get; set; }
        public virtual ICollection<TblCorporateDocument> TblCorporateDocuments { get; set; }
        [InverseProperty(nameof(TblCorporateSignatory.Customer))]
        public virtual ICollection<TblCorporateSignatory> TblCorporateSignatories { get; set; }
        [InverseProperty(nameof(TblCorporateDirector.Customer))]
        public virtual ICollection<TblCorporateDirector> TblCorporateDirectors { get; set; }
        [InverseProperty(nameof(TblNotification.Customer))]
        public virtual ICollection<TblNotification> TblNotifications { get; set; }
    }
}
