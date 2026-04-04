using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace DogoFinance.DataAccess.Layer.Models.Entities
{
    [Table("TBL_NEXT_OF_KIN")]
    public partial class TblNextOfKin
    {
        [Key]
        public long Id { get; set; }
        public long CustomerId { get; set; }
        [StringLength(150)]
        public string FullName { get; set; } = null!;
        public int RelationshipTypeId { get; set; }
        [StringLength(20)]
        public string PhoneNumber { get; set; } = null!;
        [StringLength(150)]
        public string? Email { get; set; }
        [StringLength(250)]
        public string? Address { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime? ModifiedAt { get; set; }

        [ForeignKey(nameof(CustomerId))]
        [InverseProperty(nameof(TblCustomer.TblNextOfKins))]
        public virtual TblCustomer Customer { get; set; } = null!;
        [ForeignKey(nameof(RelationshipTypeId))]
        [InverseProperty(nameof(TblRelationshipType.TblNextOfKins))]
        public virtual TblRelationshipType RelationshipType { get; set; } = null!;
    }
}
