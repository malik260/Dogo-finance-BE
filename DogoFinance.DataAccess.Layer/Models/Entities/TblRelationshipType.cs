using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace DogoFinance.DataAccess.Layer.Models.Entities
{
    [Table("TBL_RELATIONSHIP_TYPE")]
    public partial class TblRelationshipType
    {
        public TblRelationshipType()
        {
            TblNextOfKins = new HashSet<TblNextOfKin>();
        }

        [Key]
        public int Id { get; set; }
        [StringLength(50)]
        public string Name { get; set; } = null!;
        [Required]
        public bool? IsActive { get; set; }

        [InverseProperty(nameof(TblNextOfKin.RelationshipType))]
        public virtual ICollection<TblNextOfKin> TblNextOfKins { get; set; }
    }
}
