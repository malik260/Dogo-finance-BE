using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace DogoFinance.DataAccess.Layer.Models.Entities
{
    [Table("TBL_WALLET")]
    public partial class TblWallet
    {
        [Key]
        public long WalletId { get; set; }
        public long CustomerId { get; set; }
        [StringLength(10)]
        public string WalletNumber { get; set; } = null!;
        public int Currency { get; set; }
        [Column(TypeName = "decimal(18, 2)")]
        public decimal Balance { get; set; }
        [Required]
        public bool? IsActive { get; set; }
        public DateTime? CreatedAt { get; set; }
        [Timestamp]
        public byte[] RowVersion { get; set; } = null!;

        [ForeignKey(nameof(CustomerId))]
        [InverseProperty(nameof(TblCustomer.TblWallets))]
        public virtual TblCustomer Customer { get; set; } = null!;
    }
}
