using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace DogoFinance.DataAccess.Layer.Models.Entities
{
    [Table("TBL_ROLE")]
    public partial class TblRole
    {
        public TblRole()
        {
            TblUserRoles = new HashSet<TblUserRole>();
        }

        [Key]
        public int Id { get; set; }
        [StringLength(50)]
        public string Name { get; set; } = null!;

        [InverseProperty(nameof(TblUserRole.Role))]
        public virtual ICollection<TblUserRole> TblUserRoles { get; set; }
    }
}
