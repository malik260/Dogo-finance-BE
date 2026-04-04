using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace DogoFinance.DataAccess.Layer.Models.Entities
{
    [Table("TBL_USER")]
    [Index(nameof(Email), Name = "IX_TBL_USER_Email", IsUnique = true)]
    [Index(nameof(PhoneNumber), Name = "IX_TBL_USER_PhoneNumber", IsUnique = true)]
    [Index(nameof(UserName), Name = "IX_TBL_USER_UserName", IsUnique = true)]
    public partial class TblUser
    {
        public TblUser()
        {
            TblPasswordResets = new HashSet<TblPasswordReset>();
            TblPayments = new HashSet<TblPayment>();
            TblTransactionApprovedByUsers = new HashSet<TblTransaction>();
            TblTransactionInitiatedByUsers = new HashSet<TblTransaction>();
            TblUserRoles = new HashSet<TblUserRole>();
        }

        [Key]
        public long UserId { get; set; }
        [StringLength(100)]
        public string UserName { get; set; } = null!;
        [StringLength(150)]
        public string Email { get; set; } = null!;
        [StringLength(20)]
        public string PhoneNumber { get; set; } = null!;
        [StringLength(500)]
        public string PasswordHash { get; set; } = null!;
        [StringLength(200)]
        public string Salt { get; set; } = null!;
        [Required]
        public bool? IsActive { get; set; }
        public bool IsLocked { get; set; }
        public int FailedLoginAttempts { get; set; }
        public DateTime? LastLoginDate { get; set; }
        public DateTime? LastLogoutDate { get; set; }
        public DateTime? LastPasswordChangeDate { get; set; }
        public bool IsSystemUser { get; set; }
        public DateTime CreatedAt { get; set; }
        public long? CreatedBy { get; set; }
        public DateTime? ModifiedAt { get; set; }
        public long? ModifiedBy { get; set; }
        public bool IsDeleted { get; set; }
        [Timestamp]
        public byte[] RowVersion { get; set; } = null!;
        public bool PhoneNumberConfirmed { get; set; }
        [StringLength(500)]
        public string? TransactionPinHash { get; set; }
        [StringLength(200)]
        public string? TransactionPinSalt { get; set; }
        public bool IsPinSet { get; set; }
        public int PinFailedAttempts { get; set; }
        public bool IsPinLocked { get; set; }
        public DateTime? LastPinChangeDate { get; set; }
        public bool Is2faEnabled { get; set; }
        [StringLength(10)]
        public string? VerificationCode { get; set; }
        public DateTime? VerificationExpiry { get; set; }
        [StringLength(500)]
        public string? RefreshToken { get; set; }
        public DateTime? RefreshTokenExpiry { get; set; }

        [InverseProperty("User")]
        public virtual TblCustomer TblCustomer { get; set; } = null!;
        [InverseProperty(nameof(TblPasswordReset.User))]
        public virtual ICollection<TblPasswordReset> TblPasswordResets { get; set; }
        [InverseProperty(nameof(TblPayment.User))]
        public virtual ICollection<TblPayment> TblPayments { get; set; }
        [InverseProperty(nameof(TblTransaction.ApprovedByUser))]
        public virtual ICollection<TblTransaction> TblTransactionApprovedByUsers { get; set; }
        [InverseProperty(nameof(TblTransaction.InitiatedByUser))]
        public virtual ICollection<TblTransaction> TblTransactionInitiatedByUsers { get; set; }
        [InverseProperty(nameof(TblUserRole.User))]
        public virtual ICollection<TblUserRole> TblUserRoles { get; set; }
    }
}
