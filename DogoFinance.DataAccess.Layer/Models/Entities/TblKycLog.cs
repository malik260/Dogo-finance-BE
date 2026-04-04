using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace DogoFinance.DataAccess.Layer.Models.Entities
{
    [Table("TBL_KYC_LOG")]
    public partial class TblKycLog
    {
        [Key]
        public long Id { get; set; }
        public long CustomerId { get; set; }
        [StringLength(10)]
        public string? Type { get; set; }
        [StringLength(20)]
        public string? Status { get; set; }
        public string? Response { get; set; }
        public DateTime? CreatedAt { get; set; }

        [ForeignKey(nameof(CustomerId))]
        [InverseProperty(nameof(TblCustomer.TblKycLogs))]
        public virtual TblCustomer Customer { get; set; } = null!;
    }
}
